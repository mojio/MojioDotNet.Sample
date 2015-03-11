using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Mojio;
using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Cross.ObservableEvents;
using MojioDotNet.Sample.Cross.ViewModels;

namespace MojioDotNet.Sample.Windows.ViewModels
{
    public abstract class WindowsBaseViewModel : Cross.ViewModels.BaseViewModel
    {
        protected WindowsBaseViewModel(MojioManager manager) : base(manager)
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Tick += timer_Tick;
            timer.Start();

        }


        private DispatcherTimer timer;
        private readonly CoreDispatcher _dispatcher;

        
        private async Task SetProperty(string propertyName, object value)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var property = (from p in typeInfo.DeclaredProperties where p.Name == propertyName  select p).FirstOrDefault();
                if (property != null)
                {
                    property.SetMethod.Invoke(this, new object[] {value});
                }
            });
        }

        private TypeInfo typeInfo = typeof(WindowsBaseViewModel).GetTypeInfo();
        protected void Register(TypeInfo type)
        {
            typeInfo = type;
        }

  
        private async void timer_Tick(object sender, object et)
        {
            timer.Stop();
            lock (_notificationLock)
            {
                if (notifications.Count > 0)
                {
                    foreach (var e in notifications)
                    {
                        if (_dispatcher != null && _dispatcher.HasThreadAccess)
                        {
                            SetProperty(e.Key, e.Value);//.Wait();
                        }
                        //else
                        //{
                        //    notifications.Add(e.Key, e.Value);
                        //}
                    }
                }
                notifications.Clear();
            }
            timer.Start();
        }

        private static object _notificationLock = new object();
        Dictionary<string, object> notifications = new Dictionary<string, object>();

        public override async void OnNext(User value)
        {

            if (_dispatcher != null && _dispatcher.HasThreadAccess)
            {
                SetProperty("User", value);
            }
            else
            {
                lock (_notificationLock)
                {
                    notifications.Add("User", value);
                }
            }
        }

        public override void OnNext(AuthenticationEvent value)
        {

            if (_dispatcher != null && _dispatcher.HasThreadAccess)
            {
                SetProperty("AuthenticationVisible", value.IsAuthenticated);
            }
            else
            {
                lock (_notificationLock)
                {
                    notifications.Add("AuthenticationVisible", value.IsAuthenticated);
                }
            }
        }


        public override void OnNext(List<ComposedVehicle> value)
        {

            if (_dispatcher != null && _dispatcher.HasThreadAccess)
            {
                SetProperty("ComposedVehicles", value);
            }
            else
            {
                lock (_notificationLock)
                {
                    notifications.Add("ComposedVehicles", value);
                }
            }
        }

    }
}
