using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Windows.ViewModels;
using System.Collections.Generic;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MojioDotNet.Sample.Windows
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private MojioManager _manager;
        private HomeViewModel viewModel;
        private bool _isDirty = false;
        private bool _onOriginalState = false;

        public SettingsPage()
        {
            this.InitializeComponent();

            settings = ApplicationData.Current.RoamingSettings;

            if (settings.Values.ContainsKey("sandbox"))
            {
                var v = settings.Values["sandbox"];
                if (v != null)
                {
                    bool value = false;
                    bool.TryParse(v.ToString(), out value);
                    SandboxSwitch.IsOn = value;
                    _onOriginalState = value;
                }
            }

            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (_isDirty)
            {
                rootFrame.Navigate(typeof (MainPage), true);
                e.Handled = true;
            }
            else
            {
                if (rootFrame != null && rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                    e.Handled = true;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = e.Parameter as object[];
            _manager = args[0] as MojioManager;

            viewModel = new HomeViewModel(_manager);
            this.DataContext = viewModel;

            base.OnNavigatedTo(e);
        }

        private ApplicationDataContainer settings;

        private void SanboxSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            var newValue = (sender as ToggleSwitch).IsOn;
            if (settings.Values.ContainsKey("sandbox")) settings.Values.Remove("sandbox");
            settings.Values.Add("sandbox", newValue);

            _isDirty = (newValue != _onOriginalState);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _manager.ComposedVehicles = new List<ComposedVehicle>();
            _manager.IsAuthenticated = false;
            settings.Values.Clear();
            _isDirty = true;
            SandboxSwitch.IsOn = false;
        }
    }
}