using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Windows.ViewModels;
using System;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MojioDotNet.Sample.Windows
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MojioManager _manager;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_manager != null && _manager.IsAuthenticated)
            {
                base.OnNavigatedTo(e);
                return;
            }

            var config = new ApplicationConfiguration();
            var settings = ApplicationData.Current.RoamingSettings;

            if (settings.Values.ContainsKey("sandbox"))
            {
                var v = settings.Values["sandbox"];
                if (v != null)
                {
                    bool value = false;
                    if (bool.TryParse(v.ToString(), out value))
                    {
                        config.Live = !value;
                    }
                }
            }
            _manager = new MojioManager(config);

            var model = new HomeViewModel(_manager);
            this.DataContext = model;

            if (settings.Values.ContainsKey(App.MojioStorageKey))
            {
                var value = settings.Values[App.MojioStorageKey];
                if (value != null)
                {
                    var str = value as string;
                    if (!string.IsNullOrEmpty(str))
                    {
                        var t = _manager.HandleTokenResponse(str);
                        App.RegisterBackgroundTask(str);
                    }
                }
            }
            if (!_manager.IsAuthenticated)
            {
                timer.Interval = new TimeSpan(0, 0, 0);
                timer.Tick += timer_Tick;
                timer.Start();
            }
            base.OnNavigatedTo(e);
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
                Frame.Navigate(typeof(VehicleDetailsPage), new object[] { _manager, item });
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage), new object[] { _manager });
        }
    }
}