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
using MojioDotNet.Sample.Cross;
using Xamarin.Auth;

namespace MojioDotNet.Sample.Droid.Views
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        public LoginActivity()
        {
            auth = new OAuth2Authenticator(MojioManager.Configuration.ApplicationId,
                MojioManager.Configuration.SecretKey, null,
                MojioManager.Configuration.AuthorizeUri, MojioManager.Configuration.RedirectUri,
                MojioManager.Configuration.AuthorizeUri);

            auth.Completed += auth_Completed;           
        }

        private OAuth2Authenticator auth = null;
        public Intent GetUI()
        {
            return auth.GetUI(this);
        }

        public MojioManager MojioManager { get; set; }


        private void auth_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {

        }
    }
}