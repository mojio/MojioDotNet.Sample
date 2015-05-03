using Mojio;
using Mojio.Client;
using Mojio.Events;
using MojioDotNet.Sample.Cross.Extensions;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Cross.ObservableEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

            _client = new MojioClient(_configuration.Host);

            _configuration.AuthorizeUri = _client.getAuthorizeUri(_configuration.ApplicationId,
                _configuration.RedirectUri.ToString(), _configuration.Live);
        }

        private MojioClient _client = null;

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

                        try
                        {
                            var details = (await _client.GetVehicleDetailsAsync(vehicle.Id)).Data;
                            c.VehicleDetails = details;


                            var service = await _client.GetVehicleServiceScheduleAsync(vehicle.Id);

                            Dictionary<string, ComposedVehicleService> storedList =
                                new Dictionary<string, ComposedVehicleService>();

                            if (service != null && service.Data != null && service.Data.Data != null)
                            {
                                c.VehicleService = new List<ComposedVehicleService>();

                                foreach (var s in service.Data.Data)
                                {
                                    string key = string.Format("{0}{1}{2}", s.IntervalType, s.Units, s.Value);
                                    var odo = false;
                                    var time = false;

                                    if (!string.IsNullOrEmpty(s.Units))
                                    {
                                        if (timeUnits.Contains(s.Units))
                                        {
                                            time = true;
                                            odo = false;
                                        }
                                        else
                                        {
                                            time = false;
                                            odo = true;
                                        }
                                    }

                                    if (odo || time)
                                    {
                                        bool add = false;
                                        if (odo)
                                        {

                                            s.Value = FixOdo(s.Value);

                                            if (s.IntervalType == "At")
                                            {
                                                if (vehicle.EstimatedOdometer.HasValue &&
                                                    vehicle.EstimatedOdometer.Value <= s.Value)
                                                {
                                                    add = true;
                                                }
                                            }
                                            else
                                            {
                                                add = true;
                                            }

                                        }
                                        else
                                        {
                                            if (s.IntervalType == "At" && c.VehicleDetails != null &&
                                                c.VehicleDetails.Year > 0)
                                            {
                                                if (s.Units == "Months")
                                                {
                                                    if (c.EstimatedVehicleAgeInMonths < s.Value) add = true;
                                                }
                                                else if (s.Units == "Year")
                                                {
                                                    var months = s.Value*12;
                                                    if (c.EstimatedVehicleAgeInMonths < months) add = true;
                                                }
                                            }
                                            else
                                            {
                                                add = true;

                                            }
                                        }

                                        if (add)
                                        {
                                            if (storedList.ContainsKey(key))
                                            {
                                                var cvs = storedList[key];
                                                cvs.SubServices.Add(s);
                                            }
                                            else
                                            {

                                                var cvs = new ComposedVehicleService()
                                                {
                                                    VehicleService = s,
                                                    CurrentOdometer = vehicle.EstimatedOdometer,
                                                    IsTimeBased = time,
                                                };
                                                c.VehicleService.Add(cvs);
                                                storedList.Add(key, cvs);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception)
                        {
                        }
                        if (c.VehicleService != null && c.VehicleService.Count > 0)
                        {
                            c.VehicleService =
                                c.VehicleService.OrderBy(z => z.VehicleService.IntervalType)
                                    .ThenBy(z => z.VehicleService.Units)
                                    .ThenBy(z => z.VehicleService.Value)
                                    .ToList();
                        }
                        composed.Add(c);
                    }
                }

                ComposedVehicles = composed;
                Push(ComposedVehicles);

            }
            catch (Exception e)
            {
            }
        }

        private double FixOdo(double value)
        {
            string val = Math.Round(value,0).ToString();
            if (val.Length <= 3) return value;

            switch (val.Length)
            {
                case 4: //1,000
                    val = string.Format("{0}000", val[0]);
                    break;
                case 5: //10,000
                    val = string.Format("{0}{1}000", val[0], val[1]);
                    break;
                case 6: //100,000
                    val = string.Format("{0}{1}{2}000", val[0], val[1], val[2]);
                    break;
            }
            double newVal = value;
            double.TryParse(val, out newVal);
            return newVal;
        }
        List<string> timeUnits = new List<string>()
        {
            "Years", "Months", "Hours", "Weeks", "Minutes", "Seconds"
        };

        private async Task UpdateExtended()
        {
            foreach (var v in ComposedVehicles)
            {
                try
                {
                    var details = (await _client.GetVehicleDetailsAsync(v.Vehicle.Id)).Data;
                    v.VehicleDetails = details;
                    Debug.WriteLine(string.Format("got vehicle details:{0}", v.Vehicle.Id));
                }
                catch (Exception e)
                {
                }
                try
                {
                    var vehicleServiceSchedules = (await _client.GetVehicleServiceScheduleAsync(v.Vehicle.Id)).Data.Data;
                    //v.VehicleServiceSchedules = vehicleServiceSchedules;
                    Debug.WriteLine(string.Format("got vehicle service schedules:{0}", v.Vehicle.Id));
                }
                catch (Exception e)
                {
                }
            }
            SetupObservers();
            Push(ComposedVehicles);
        }

        private async Task SetupObservers()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(1000).Wait();

                    try
                    {
                        var lst = new List<ComposedVehicle>();
                        var newstate = _client.UserVehiclesAsync(this.User.Id, 1).Result;
                        if (newstate != null && newstate.Data != null && newstate.Data.Data != null)
                        {
                            foreach (var v in newstate.Data.Data)
                            {
                                var vc = new ComposedVehicle() {Vehicle = v};
                                var existing = (from x in this.ComposedVehicles where x.Vehicle.Id == v.Id select x).FirstOrDefault();
                                if (existing != null) vc.VehicleDetails = existing.VehicleDetails;                                
                                lst.Add(vc);                               
                            }
                            Push(lst);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        private IEnumerable<Event> DeDupEvents(ComposedVehicle vehicle, IEnumerable<Event> incoming)
        {
            List<Event> noDups = new List<Event>();
            if (incoming != null)
            {
                foreach (var e in incoming)
                {
                    var existing = (from evt in vehicle.EventHistory where evt.Id == e.Id select evt).FirstOrDefault();

                    if (existing == null)
                    {
                        if (e.EventType != EventType.TripStatus && e.EventType != EventType.OffStatus &&
                            e.EventType != EventType.Diagnostic && e.EventType != EventType.DeviceDiagnostic &&
                            e.EventType != EventType.HeartBeat && e.EventType != EventType.MojioOff &&
                            e.EventType != EventType.MojioOn && e.EventType != EventType.MojioWake &&
                            e.EventType != EventType.MojioIdle
                            )
                        {
                            var composedEvent = new ComposedEvent() {Event = e};
                            noDups.Add(e);
                            vehicle.EventHistory.Add(composedEvent);
                        }
                    }
                }
            }
            return noDups;
        }

        private void HandleEvent(ComposedVehicle vehicle, Event entity)
        {
            var evt = entity;

            if (evt.Location != null)
            {
                vehicle.Vehicle.LastLocation = evt.Location;
            }
            if (evt.Accelerometer != null)
            {
                vehicle.Vehicle.LastAccelerometer = evt.Accelerometer;
            }
            if (evt.BatteryVoltage != null)
            {
                vehicle.Vehicle.LastBatteryVoltage = evt.BatteryVoltage;
            }
            //if (updated && OnVehicleUpdated != null) OnVehicleUpdated(vehicle);
            //if (vehicle.Vehicle.Id == SelectedVehicle.Vehicle.Id) OnPropertyChanged("SelectedVehicle");

            Debug.WriteLine(string.Format("{2} {0}:{1}", vehicle.Vehicle.Name, entity.EventType, entity.Time));
        }

        public ComposedVehicle VehicleById(Guid id)
        {
            return (from v in ComposedVehicles where v.Vehicle.Id == id select v).FirstOrDefault();
        }

        public oAuthToken HandleTokenResponse(string tokenBits)
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
            return token;
        }

        private oAuthToken _oAuthToken;

        public oAuthToken OAuthToken
        {
            get { return _oAuthToken; }
            set { _oAuthToken = value; }
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
            else if (typeof (T).GetTypeInfo() == typeof (List<ComposedVehicle>).GetTypeInfo())
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