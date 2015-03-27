using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Droid.Views;
using MojioDotNet.Sample.Views;
using Xamarin.Forms;
using Xamarin.Auth;

namespace MojioDotNet.Sample
{
	public class App : Application
	{
        public static App Instance { get; private set; }
	    public MojioManager  MojioManager { get; set; }

	    public App()
	    {
	        Instance = this;
            MojioManager = new MojioManager(new ApplicationConfiguration());
        }

	    protected override void OnStart ()
		{
			// Handle when your app starts
            MainPage = new MainPage();
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
