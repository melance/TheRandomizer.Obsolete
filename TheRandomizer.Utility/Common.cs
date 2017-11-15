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
        /// <summary>
        /// Returns the name of the calling method
        /// </summary>
        /// <param name="caller">This should always be excluded from the call so that the name of the caller is returned.</param>
        /// <returns>The <see cref="CallerMemberNameAttribute"/> value</returns>
        public static string GetCaller([CallerMemberName] string caller = null)
        {
            return caller;
        }
    }
}
