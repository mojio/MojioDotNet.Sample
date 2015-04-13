using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Windows.ViewModels;
using Windows.Devices.Geolocation;
using Windows.Phone.UI.Input;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MojioDotNet.Sample.Windows
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VehicleDetailsPage : Page
    {
        private readonly MojioManager _manager;
        private HomeViewModel viewModel;

        public VehicleDetailsPage()
        {
            this.InitializeComponent();

            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = e.Parameter as object[];
            var manager = args[0] as MojioManager;
            var selectedVehicle = args[1] as ComposedVehicle;

            viewModel = new HomeViewModel(manager);
            this.DataContext = viewModel;

            viewModel.SelectedVehicle = selectedVehicle;

            viewModel.PropertyChanged += model_PropertyChanged;

            base.OnNavigatedTo(e);
        }

        private string lastLocation = "";

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedVehicle" || e.PropertyName == "DevicePosition")
            {
                var model = this.DataContext as HomeViewModel;
                if (model.SelectedVehicle != null && model.SelectedVehicle.Vehicle != null &&
                    model.SelectedVehicle.Vehicle.LastLocation != null)
                {
                    var thisLocation = string.Format("{0},{1},{2}",
                        (model.SelectedVehicle.Vehicle.LastAltitude != null &&
                         model.SelectedVehicle.Vehicle.LastAltitude.HasValue)
                            ? model.SelectedVehicle.Vehicle.LastAltitude.Value
                            : -1,
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

                        if (model.DevicePosition != null)
                        {
                            var myPosition = new Geopoint(new BasicGeoposition()
                            {
                                Latitude = model.DevicePosition.Coordinate.Latitude,
                                Longitude = model.DevicePosition.Coordinate.Longitude,
                            });
                            this.VehicleMap.MapElements.Add(new MapIcon()
                            {
                                Location = myPosition,
                                Title = "My Location",
                                Visible = true
                            });
                        }

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
    }
}