using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Mojio.Client;
using MojioDotNet.Sample.Cross;

namespace MojioDotNet.Sample.Windows.Commands
{
    public abstract class MojioCommand : ICommand
    {
        protected readonly MojioManager _manager;

        public MojioCommand(MojioManager manager)
        {
            _manager = manager;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;
    }
}
