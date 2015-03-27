using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Droid.Views;
using Xamarin.Auth;


namespace MojioDotNet.Sample.Droid
{
    [Activity(Label = "MojioDotNet.Sample", Icon = "@drawable/icon", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new MojioDotNet.Sample.App());


        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            return base.OnPrepareOptionsMenu(menu);
        }

        private async Task<string> GetUsernameAsync(IDictionary<string, string> accountProperties)
        {
            return null;
        }

        private void auth_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {

        }
    }
}