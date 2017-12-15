using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.WinApp.Utility
{
    class CommandLineOptions
    {
        public CommandLineOptions(string[] arguments)
        {
            if (arguments.Contains("-e"))
            {

            }
        }

        public bool OpenEditorOnly { get; set; }
    }
}
