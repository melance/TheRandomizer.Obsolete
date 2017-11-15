using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Configuration;

namespace TheRandomizer.WinApp.Utility
{
    public class ConfigurationBindingExtension : Binding
    {
        #region Constants
        private const string MORE_GENERATORS_URL_KEY = "MoreGeneratorsURL";
        #endregion

        #region Constructors
        public ConfigurationBindingExtension() : base() { Initialize(); }

        public ConfigurationBindingExtension(string path) : base(path) { Initialize(); }
        #endregion

        #region Properties
        public string GetMoreGeneratorsURL { get { return GetItem<string>(MORE_GENERATORS_URL_KEY); } }
        #endregion

        #region Private Methods
        private void Initialize()
        {
            Source = this;
            Mode = BindingMode.OneWay;
        }

        private T GetItem<T>(string key)
        {
            return GetItem<T>(key, default(T));
        }

        private T GetItem<T>(string key, T defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            else
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
        #endregion
    }
}
