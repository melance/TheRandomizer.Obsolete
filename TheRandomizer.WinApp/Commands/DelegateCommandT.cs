using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TheRandomizer.WinApp.Commands
{
    class DelegateCommand<T> : ICommand
    {
#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        private readonly Action<T> _action;
        private readonly T _parameter;

        public DelegateCommand(Action<T> action)
        {
            _action = action;
            _parameter = default(T);
        }

        public DelegateCommand(Action<T> action, T parameter)
        {
            _action = action;
            _parameter = parameter;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_parameter != null && parameter is T)
            {
                _action((T)parameter);
            }
            else
            {
                _action(_parameter);
            }
        }
    }
}
