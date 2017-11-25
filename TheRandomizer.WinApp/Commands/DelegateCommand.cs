using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TheRandomizer.WinApp.Commands
{
    class DelegateCommand : ICommand
    {
#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        private readonly Action _action;
        private readonly Func<bool> _canExecute;

        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public DelegateCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();
            else
                return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
