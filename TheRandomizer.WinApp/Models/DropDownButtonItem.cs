using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TheRandomizer.WinApp.Models
{
    class DropDownButtonItem
    {
        public DropDownButtonItem(string text, string toolTip, ICommand command)
        {
            Text = text;
            ToolTip = toolTip;
            Command = command;
        }

        public string Text { get; set; }
        public string ToolTip { get; set; }
        public ICommand Command { get; set; }
    }
}
