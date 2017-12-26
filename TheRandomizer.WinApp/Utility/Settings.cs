using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.WinApp.Utility
{
    static class Settings
    {
        public static string GeneratorPath
        {
            get
            {
                var value = Properties.Settings.Default.GeneratorDirectory;
                value = Environment.ExpandEnvironmentVariables(value);
                return value;
            }
        }
    }
}
