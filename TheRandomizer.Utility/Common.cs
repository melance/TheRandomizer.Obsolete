using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Utility
{
    public sealed class Common
    {
        public static string GetCaller([CallerMemberName] string caller = null)
        {
            return caller;
        }
    }
}
