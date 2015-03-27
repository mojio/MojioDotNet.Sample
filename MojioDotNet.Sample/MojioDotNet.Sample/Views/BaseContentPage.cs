using System;
using System.Collections.Generic;
using System.Text;
using MojioDotNet.Sample.Droid.Views;
using Xamarin.Forms;

namespace MojioDotNet.Sample.Views
{
    public class BaseContentPage : ContentPage
    {
        protected override void OnAppearing()
        {
            if (!App.Instance.MojioManager.IsAuthenticated)
            {
                Navigation.PushModalAsync(new LoginPage());
            }
            base.OnAppearing();

        }
    }
}
