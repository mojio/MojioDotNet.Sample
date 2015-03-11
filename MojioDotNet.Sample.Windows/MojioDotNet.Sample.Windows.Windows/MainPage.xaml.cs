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
            _manager = new MojioManager(new ApplicationConfiguration());
            this.DataContext = new HomeViewModel(_manager);

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
    }
}
