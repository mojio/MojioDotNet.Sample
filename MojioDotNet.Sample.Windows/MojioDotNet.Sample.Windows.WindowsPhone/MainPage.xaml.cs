using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            this.DataContext = new HomeViewModel(_manager);
            if (!_manager.IsAuthenticated)
            {
                timer.Interval = new TimeSpan(0, 0, 0);
                timer.Tick += timer_Tick;
                timer.Start();
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