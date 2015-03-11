using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Mojio;
using MojioDotNet.Sample.Cross;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Cross.ObservableEvents;

namespace MojioDotNet.Sample.Windows.ViewModels
{
    public class HomeViewModel : WindowsBaseViewModel
    {
        private string _headerText;
        public HomeViewModel(MojioManager manager)
            : base(manager)
        {
            Register(typeof(HomeViewModel).GetTypeInfo());
            HeaderVisibility = Visibility.Visible;
        }


        public List<ComposedVehicle> ComposedVehicles
        {
            get { return Manager.ComposedVehicles; }
            set
            {
                if (Manager.ComposedVehicles != null)
                {
                    HeaderVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged();
                foreach (var c in Manager.ComposedVehicles)
                {
                    c.OnPropertyChanged("State");
                }
            }
        }

        private Visibility _headerVisibility;
        public Visibility HeaderVisibility
        {
            get { return _headerVisibility; }
            set
            {
                _headerVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility IsAuthenticVisibility
        {
            get { return (AuthenticationVisible) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility IsNotAuthenticVisibility
        {
            get { return (AuthenticationVisible) ? Visibility.Collapsed : Visibility.Visible; }
        }

        public string HeaderText
        {
            get
            {
                if (User != null)
                {
                    return string.Format("Welcome {0}, give me a few more seconds...", User.UserName);
                }
                else
                {
                    return "Give me a sec, I need to download your vehicle data";
                }
            }
        }
        public bool AuthenticationVisible
        {
            get { return Manager.IsAuthenticated; }
            set
            {
                OnPropertyChanged();
                OnPropertyChanged("IsNotAuthenticVisibility");
                OnPropertyChanged("IsAuthenticVisibility");

            }
        }

        private User _user = null;

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
                OnPropertyChanged("HeaderText");
            }
        }



    }
}