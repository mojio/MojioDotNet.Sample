using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
using Xamarin.Forms;
using Xamarin.Auth;

namespace MojioDotNet.Sample
{
	public class App : Application
	{
        private readonly MojioManager _manager = new MojioManager(new ApplicationConfiguration());

	    public App()
	    {
            // The root page of your application
            //MainPage = new ContentPage
            //{
            //    Content = new StackLayout
            //    {
            //        VerticalOptions = LayoutOptions.Center,
            //        Children = {
            //            new Label {
            //                XAlign = TextAlignment.Center,
            //                Text = "Welcome to Xamarin Forms!"
            //            }
            //        }
            //    }
            //};
            //var auth = new OAuth2Authenticator(
            //    clientId: _manager.ApplicationConfiguration.ApplicationId,
            //    scope: "",
            //    authorizeUrl: _manager.ApplicationConfiguration.AuthorizeUri,
            //    redirectUrl: _manager.ApplicationConfiguration.RedirectUri                
            //    );

            //auth.Completed += (sender, eventArgs) =>
            //{

            //    if (eventArgs.IsAuthenticated)
            //    {
            //        // Use eventArgs.Account to do wonderful things
            //    }
            //};

            //MainPage = auth.GetUI(this);

	    }

	    protected override void OnStart ()
		{
			// Handle when your app starts


            
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
