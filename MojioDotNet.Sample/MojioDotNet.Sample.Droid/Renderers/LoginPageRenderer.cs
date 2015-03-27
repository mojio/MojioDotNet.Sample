using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MojioDotNet.Sample.Droid.Renderers;
using MojioDotNet.Sample.Droid.Views;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]

namespace MojioDotNet.Sample.Droid.Renderers
{
    public class LoginPageRenderer : PageRenderer
    {
        private bool done = false;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {

            if (!done)
            {
                
                // this is a ViewGroup - so should be able to load an AXML file and FindView<>
                var activity = this.Context as Activity;

                var auth = new OAuth2Authenticator(
                    clientId: App.Instance.MojioManager.Configuration.ApplicationId,
                    scope: "",
                    authorizeUrl: App.Instance.MojioManager.Configuration.AuthorizeUri,
                    redirectUrl: App.Instance.MojioManager.Configuration.RedirectUri
                    );
                
                auth.AllowCancel = true;
                auth.ClearCookiesBeforeLogin = true;

                auth.Title = "Login to your Mojio Account";

                auth.Completed += (sender, eventArgs) =>
                {
                    if (eventArgs.IsAuthenticated)
                    {
                        //App.Instance.SuccessfulLoginAction.Invoke();
                        //// Use eventArgs.Account to do wonderful things
                        //App.Instance.SaveToken(eventArgs.Account.Properties["token"]);
                    }
                    else
                    {
                        // The user cancelled
                    }
                };
                auth.BrowsingCompleted += auth_BrowsingCompleted;

                activity.StartActivity(auth.GetUI(activity));
                done = true;
            }

            base.OnElementChanged(e);

        }

        void auth_BrowsingCompleted(object sender, EventArgs e)
        {
            
        }
    }
}