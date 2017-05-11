using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TheRandomizer.WebApp.HelperClasses
{
    public static class GenericExtensions
    {
        public static string GetDisplayName(this object extended)
        {
            var attributes = (DisplayNameAttribute[])extended.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), false);

            if (attributes != null && attributes.Count() > 0)
            {
                return attributes[0].DisplayName;
            }

            return extended.GetType().Name;
        }
    }
}