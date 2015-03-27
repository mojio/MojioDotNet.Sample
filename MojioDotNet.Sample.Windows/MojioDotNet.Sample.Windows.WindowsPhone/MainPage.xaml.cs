using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
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
            this.NavigationCacheMode = NavigationCacheMode.Required;

            _manager = new MojioManager(new ApplicationConfiguration());
            var model = new HomeViewModel(_manager);
            this.DataContext = model;
            if (!_manager.IsAuthenticated)
            {
                timer.Interval = new TimeSpan(0, 0, 0);
                timer.Tick += timer_Tick;
                timer.Start();
            }

            model.PropertyChanged += model_PropertyChanged;
        }

        private string lastLocation = "";
        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           // Debug.WriteLine(e.PropertyName);
            if (e.PropertyName == "SelectedVehicle")
            {
                var model = this.DataContext as HomeViewModel;
                if (model.SelectedVehicle != null && model.SelectedVehicle.Vehicle != null &&
                    model.SelectedVehicle.Vehicle.LastLocation != null)
                {
                    var thisLocation = string.Format("{0},{1},{2}", (model.SelectedVehicle.Vehicle.LastAltitude != null && model.SelectedVehicle.Vehicle.LastAltitude.HasValue) ? model.SelectedVehicle.Vehicle.LastAltitude.Value : -1,
                        model.SelectedVehicle.Vehicle.LastLocation.Lng, model.SelectedVehicle.Vehicle.LastLocation.Lat);
                    if (string.IsNullOrEmpty(lastLocation) || thisLocation != lastLocation)
                    {
                        lastLocation = thisLocation;
                        var location = new Geopoint(new BasicGeoposition()
                        {
                            Longitude = model.SelectedVehicle.Vehicle.LastLocation.Lng,
                            Latitude = model.SelectedVehicle.Vehicle.LastLocation.Lat,
                        });

                        this.VehicleMap.MapElements.Clear();
                        this.VehicleMap.MapElements.Add(new MapIcon()
                        {
                            Location = location,
                            Title = model.SelectedVehicle.Vehicle.Name,
                            Visible = true,
                        });

                        this.VehicleMap.ZoomLevel = 17;
                        this.VehicleMap.Center = location;
                    }
                }
            }
        }


        private DispatcherTimer timer = new DispatcherTimer();

        private void timer_Tick(object sender, object e)
        {
            timer.Stop();
            WebAuthenticationBroker.AuthenticateAndContinue(_manager.Configuration.AuthorizeUri,
                _manager.Configuration.RedirectUri,
                null, WebAuthenticationOptions.None);
        }


        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                var vm = (this.DataContext as HomeViewModel);
                vm.SelectedVehicle = item as ComposedVehicle;
                vm.SelectVehicleCommand.Execute(item);
                this.pivot.SelectedIndex = 1;
            }
        }
    }
}