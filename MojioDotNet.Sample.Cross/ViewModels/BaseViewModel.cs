using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Mojio;
using MojioDotNet.Sample.Cross.Models;
using MojioDotNet.Sample.Cross.ObservableEvents;

namespace MojioDotNet.Sample.Cross.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IObserver<AuthenticationEvent>, IObserver<User>, IObserver<List<ComposedVehicle>>
    {
        public MojioManager Manager { get; private set; }

        public BaseViewModel(MojioManager manager)
        {
            Manager = manager;
            Manager.Subscribe(this as IObserver<AuthenticationEvent>);
            Manager.Subscribe(this as IObserver<User>);
            Manager.Subscribe(this as IObserver<List<ComposedVehicle>>);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public abstract void OnNext(User value);
        public abstract void OnNext(AuthenticationEvent value);
        public abstract void OnNext(List<ComposedVehicle> value);
    }
}
