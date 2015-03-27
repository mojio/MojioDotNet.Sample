using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Bing.Maps;
using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Windows.Extensions;
using MojioDotNet.Sample.Windows.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MojioDotNet.Sample.Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private readonly MojioManager _manager;

        public MainPage()
        {
            this.InitializeComponent();
            _manager = new MojioManager(new ApplicationConfiguration());
            var model = new HomeViewModel(_manager);
            this.DataContext = model;

            model.PropertyChanged += model_PropertyChanged;
        }
        private string lastLocation = "";

        void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Debug.WriteLine(e.PropertyName);
            if (e.PropertyName == "SelectedVehicle")
            {
                var model = this.DataContext as HomeViewModel;
                if (model.SelectedVehicle != null && model.SelectedVehicle.Vehicle != null &&
                    model.SelectedVehicle.Vehicle.LastLocation != null)
                {
                    var v = model.SelectedVehicle;
                    var thisLocation = string.Format("{0},{1},{2}", (model.SelectedVehicle.Vehicle.LastAltitude!=null && model.SelectedVehicle.Vehicle.LastAltitude.HasValue)?model.SelectedVehicle.Vehicle.LastAltitude.Value:-1,
                        model.SelectedVehicle.Vehicle.LastLocation.Lng, model.SelectedVehicle.Vehicle.LastLocation.Lat);
                    if (string.IsNullOrEmpty(lastLocation) || thisLocation != lastLocation)
                    {
                        lastLocation = thisLocation;
                        var location = new Location()
                        {                            
                            Longitude = model.SelectedVehicle.Vehicle.LastLocation.Lng,
                            Latitude = model.SelectedVehicle.Vehicle.LastLocation.Lat,
                        };

                        var map = VehicleMap;

                        var pin = new Pushpin()
                        {
                            Text = v.Vehicle.Name,
                            DataContext = v,
                            Width=100,
                            Height=100,
                        };
                        MapLayer.SetPosition(pin, location);
                        map.Children.Clear();
                        map.Children.Add(pin);
                        map.Center = location;
                        map.ZoomLevel = 17;

                        //this.VehicleMap.MapElements.Clear();
                        //this.VehicleMap.MapElements.Add(new MapIcon()
                        //{
                        //    Location = location,
                        //    Title = model.SelectedVehicle.Vehicle.Name,
                        //    Visible = true,
                        //});

                        //this.VehicleMap.ZoomLevel = 17;
                        //this.VehicleMap.Center = location;
                    }
                }
            }
        }

        private Bing.Maps.Map VehicleMap
        {
            get { return this.pageRoot.Frame.FindChild<Bing.Maps.Map>("VehicleMap"); }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {

                var result =
                    await
                        WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None,
                            _manager.Configuration.AuthorizeUri,
                            _manager.Configuration.RedirectUri);

                if (result.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    //		ResponseData	"http://localhost/_done#access_token=b90aa72d-45cd-4371-a560-14c295b224ef&token_type=bearer&expires_in=2629740"	string
                    var tokenBits = result.ResponseData;
                    _manager.HandleTokenResponse(tokenBits);
                }
                else
                {
                    _manager.IsAuthenticated = false;
                }

            }
            catch (Exception)
            {

                throw;
            }

            base.OnNavigatedTo(e);
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                var vm = (this.DataContext as HomeViewModel);
                vm.SelectedVehicle = item as ComposedVehicle;
                vm.SelectVehicleCommand.Execute(item);                
            }

        }
    }
}
