using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TheRandomizer.WinApp.Commands
{
    class DialogCommands
    {
        public static ICommand Cancel
        {
            get
            {
                return new DelegateCommand<Window>(
                    w =>
                    {
                        w.DialogResult = false;
                        w.Close();
                    });
            }
        }

        public static ICommand Ok
        {
            get
            {
                return new DelegateCommand<Window>(
                    w=>
                    {
                        w.DialogResult = true;
                        w.Close();
                    });
            }
        }
    }
}
