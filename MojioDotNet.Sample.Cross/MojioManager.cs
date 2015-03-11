﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Mojio;
using Mojio.Client;
using Mojio.Events;
using MojioDotNet.Sample.Cross.Extensions;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Cross.ObservableEvents;

namespace MojioDotNet.Sample.Cross
{
    public class MojioManager : IObservable<AuthenticationEvent>, IObservable<User>, IObservable<List<ComposedVehicle>>
    {
        public ApplicationConfiguration Configuration
        {
            get { return _configuration; }
        }

        private readonly ApplicationConfiguration _configuration;


        public MojioManager(ApplicationConfiguration configuration)
        {
            _configuration = configuration;
            IsAuthenticated = false;

            _configuration.AuthorizeUri = _client.getAuthorizeUri(_configuration.ApplicationId,
                _configuration.RedirectUri.ToString(), _configuration.Live);


        }


        private MojioClient _client = new MojioClient("https://develop.api.moj.io/v1");

        public ApplicationConfiguration ApplicationConfiguration { get; set; }

        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set
            {
                _isAuthenticated = value;
                Push(new AuthenticationEvent() {IsAuthenticated = value});

                if (_isAuthenticated)
                {
                    Task.Factory.StartNew(() => { Connect(); });
                }
            }
        }

        private bool tokenSet = false;
        public User User { get; set; }

        private async Task Connect()
        {
            if (!tokenSet)
            {
                tokenSet = true;
                try
                {
                    await _client.TokenAsync(_configuration.ApplicationId, OAuthToken.AccessToken);
                    
                }
                catch (Exception)
                {

                    throw;
                }

            }
            try
            {

                User = await _client.GetCurrentUserAsync();
                Push(User);

                var composed = new List<ComposedVehicle>();

                var userVehicles = await _client.UserVehiclesAsync(User.Id);
                if (userVehicles.StatusCode == HttpStatusCode.OK)
                {
                    foreach (var vehicle in userVehicles.Data.Data)
                    {
                        var c = new ComposedVehicle()
                        {
                            Vehicle = vehicle,
                        };

                        composed.Add(c);
                    }
                }
                ComposedVehicles = composed;
                Push(ComposedVehicles);

                foreach (var v in ComposedVehicles)
                {
                    try
                    {
                        v.VehicleDetails = (await _client.GetVehicleDetailsAsync(v.Vehicle.Id)).Data;
                    }
                    catch (Exception)
                    {
                    }
                }
                Push(ComposedVehicles);


                SetupObservers();
            }
            catch (Exception e)
            {

                throw;
            }
        }


        private async Task SetupObservers()
        {
            _client.ObserveHandler += _client_ObserveHandler;

            foreach (var v in this.ComposedVehicles)
            {
                if (v.Vehicle.MojioId.HasValue)
                {
                    var o = new Observer(typeof(Event), typeof(Vehicle))
                    {
                        Transports = Transport.SignalR,
                        ParentId = v.Vehicle.Id,
                        Status = ObserveStatus.Approved,                        
                        //Parent = v.Vehicle.Id.ToString()
                    };
                    try
                    {
                        //lets not block for this, let it ride
                        await _client.CreateAsync(o).ContinueWith(r =>
                        {
                            if (!r.IsFaulted && r.Result != null && r.Result.Data != null)
                            {
                                _client.Observe(r.Result.Data).ContinueWith(t =>
                                {
                                    if (t.IsFaulted)
                                    {
                                        Debug.WriteLine("Not watching for events for " + v.Vehicle.Name);

                                    }
                                    else
                                    {
                                        Debug.WriteLine("Watching for events for " + v.Vehicle.Name);                                        
                                    }
                                }).Wait();
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        //throw;
                    }
                }
                else
                {

                }
            }
        }
        void _client_ObserveHandler(GuidEntity entity)
        {
            if (entity is Event)
            {
                var evt = entity as Event;
                var vehicle = VehicleById(evt.VehicleId);
                bool updated = false;
                
                if (evt.Location != null)
                {
                    vehicle.Vehicle.LastLocation = evt.Location;
                    updated = true;
                }
                if (evt.Accelerometer != null)
                {
                    vehicle.Vehicle.LastAccelerometer = evt.Accelerometer;
                    updated = true;
                }
                if (evt.BatteryVoltage != null)
                {
                    vehicle.Vehicle.LastBatteryVoltage = evt.BatteryVoltage;
                    updated = true;
                }
                //if (updated && OnVehicleUpdated != null) OnVehicleUpdated(vehicle);
                //if (vehicle.Vehicle.Id == SelectedVehicle.Vehicle.Id) OnPropertyChanged("SelectedVehicle");

                Push(ComposedVehicles);


            }
        }

        public ComposedVehicle VehicleById(Guid id)
        {
            return (from v in ComposedVehicles where v.Vehicle.Id == id select v).FirstOrDefault();
        }
        

        public void HandleTokenResponse(string tokenBits)
        {
            if (!tokenBits.Contains("?"))
            {
                tokenBits = tokenBits.Replace("#", "?");
            }
            else
            {
                tokenBits = tokenBits.Replace("#", "");
            }

            System.Uri responseUri = new Uri(tokenBits);

            var queryString = responseUri.ParseQueryString();
            var token = new oAuthToken()
            {
                AccessToken = queryString["access_token"],
                ExpiresIn = queryString["expires_in"],
                TokenType = queryString["token_type"]
            };

            IsAuthenticated = true;
            OAuthToken = token;
        }

        private oAuthToken _oAuthToken;

        public oAuthToken OAuthToken
        {
            get { return _oAuthToken; }
            set
            {
                _oAuthToken = value;                
            }
        }

        private void Push<T>(T evt)
        {
            List<IObserver<T>> lst = null;
            if (typeof (T).GetTypeInfo() == typeof (AuthenticationEvent).GetTypeInfo())
            {
                lst = _authObservers as List<IObserver<T>>;
            }
            else if (typeof (T).GetTypeInfo() == typeof (User).GetTypeInfo())
            {
                lst = _userObservers as List<IObserver<T>>;
            }
            else if (typeof(T).GetTypeInfo() == typeof(List<ComposedVehicle>).GetTypeInfo())
            {
                lst = _vehicleObservers as List<IObserver<T>>;
            }
            if (lst != null)
            {
                foreach (var o in lst)
                {
                    o.OnNext(evt);
                }
            }
        }

        private List<IObserver<AuthenticationEvent>> _authObservers = new List<IObserver<AuthenticationEvent>>();
        public IDisposable Subscribe(IObserver<AuthenticationEvent> observer)
        {
            if (!_authObservers.Contains(observer))
            {
                _authObservers.Add(observer);
            }
            return new Unsubscriber<AuthenticationEvent>(_authObservers, observer);
        }



        private List<IObserver<User>> _userObservers = new List<IObserver<User>>();
        public IDisposable Subscribe(IObserver<User> observer)
        {
            if (!_userObservers.Contains(observer))
            {
                _userObservers.Add(observer);
            }
            return new Unsubscriber<User>(_userObservers, observer);
        }

        public List<ComposedVehicle> ComposedVehicles
        {
            get { return _composedVehicles; }
            set { _composedVehicles = value; }
        }

        private List<IObserver<List<ComposedVehicle>>> _vehicleObservers = new List<IObserver<List<ComposedVehicle>>>();
        private List<ComposedVehicle> _composedVehicles;

        public IDisposable Subscribe(IObserver<List<ComposedVehicle>> observer)
        {
            if (!_vehicleObservers.Contains(observer))
            {
                _vehicleObservers.Add(observer);
            }
            return new Unsubscriber<List<ComposedVehicle>>(_vehicleObservers, observer);
        }

    }
}