using Mojio;
using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Windows.Commands;
using System.Collections.Generic;
using System.Reflection;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;

namespace MojioDotNet.Sample.Windows.ViewModels
{
    public class HomeViewModel : WindowsBaseViewModel
    {
        private string _headerText;

        public HomeViewModel(MojioManager manager)
            : base(manager)
        {
            Register(typeof (HomeViewModel).GetTypeInfo());
            HeaderVisibility = Visibility.Visible;

            var geo = new Geolocator();
            geo.ReportInterval = 5000;
            geo.PositionChanged += geo_PositionChanged;
        }

        private Geoposition _devicePosition;

        public Geoposition DevicePosition
        {
            get { return _devicePosition; }
            set
            {
                _devicePosition = value;
                OnPropertyChanged();
                if (this.ComposedVehicles != null)
                {
                    foreach (var v in this.ComposedVehicles)
                    {
                        v.DevicePosition = new Location()
                        {
                            Lat = value.Coordinate.Latitude,
                            Lng = value.Coordinate.Longitude
                        };
                    }
                }
            }
        }

        private void geo_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            base.SetProperty("DevicePosition", args.Position);
        }

        private SelectVehicleCommand _selectVehicleCommand;

        public SelectVehicleCommand SelectVehicleCommand
        {
            get
            {
                if (_selectVehicleCommand == null) _selectVehicleCommand = new SelectVehicleCommand(Manager);
                return _selectVehicleCommand;
            }
        }

        private ComposedVehicle _selectedVehicle;

        public ComposedVehicle SelectedVehicle
        {
            get { return _selectedVehicle; }
            set
            {
                _selectedVehicle = value;
                if (_selectedVehicle != null)
                {
                    _selectedVehicle.OnPropertyChanged("State");
                    _selectedVehicle.OnPropertyChanged("DiagnosticsCodes");
                    _selectedVehicle.OnPropertyChanged("Vehicle");
                    _selectedVehicle.OnPropertyChanged("EventHistory");
                }
                OnPropertyChanged();
                OnPropertyChanged("DetailTabsVisible");
            }
        }

        public List<ComposedVehicle> ComposedVehicles
        {
            get { return Manager.ComposedVehicles; }
            set
            {
                if (Manager.ComposedVehicles != null)
                {
                    HeaderVisibility = Visibility.Collapsed;
                }
                foreach (var c in Manager.ComposedVehicles)
                {
                    c.OnPropertyChanged("State");
                    c.OnPropertyChanged("DiagnosticsCodes");
                    c.OnPropertyChanged("Vehicle");
                    c.OnPropertyChanged("EventHistory");
                    c.DevicePosition = new Location()
                    {
                        Lat = this.DevicePosition.Coordinate.Latitude,
                        Lng = this.DevicePosition.Coordinate.Longitude
                    };
                }

                if (_selectedVehicle != null) SelectedVehicle = _selectedVehicle;
                OnPropertyChanged();
            }
        }

        private Visibility _headerVisibility;

        public Visibility HeaderVisibility
        {
            get { return _headerVisibility; }
            set
            {
                _headerVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility DetailTabsVisible
        {
            get { return (SelectedVehicle == null) ? Visibility.Collapsed : Visibility.Visible; }
        }

        public Visibility IsAuthenticVisibility
        {
            get { return (AuthenticationVisible) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility IsNotAuthenticVisibility
        {
            get { return (AuthenticationVisible) ? Visibility.Collapsed : Visibility.Visible; }
        }

        public string DetailsHeaderText
        {
            get { return "vehicle details"; }
        }

        public string HeaderText
        {
            get
            {
                if (User != null)
                {
                    return string.Format("Welcome {0}, give me a few more seconds...", User.UserName);
                }
                else
                {
                    return "Give me a sec, I need to download your vehicle data";
                }
            }
        }

        public bool AuthenticationVisible
        {
            get { return Manager.IsAuthenticated; }
            set
            {
                OnPropertyChanged();
                OnPropertyChanged("IsNotAuthenticVisibility");
                OnPropertyChanged("IsAuthenticVisibility");
            }
        }

        private User _user = null;

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
                OnPropertyChanged("HeaderText");
            }
        }
    }
}