using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TheRandomizer.WebApp.HelperClasses
{
    internal class Settings
    {
        public static T GetSetting<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T GetSetting<T>(string key, T defaultValue)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return GetSetting<T>(key);
            }
            else
            {
                return defaultValue;
            }
        }

        public static AuthenticationElement GetAuthentication(string name)
        {
            var section = (AuthenticationSection)ConfigurationManager.GetSection("authenticationSettings");
            return section.AuthenticationItems[name];
        }
    }
}