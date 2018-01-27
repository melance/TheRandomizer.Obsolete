using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.ComponentModel;

namespace TheRandomizer.WinApp.Utility
{
    public static class Settings 
    {
        static Dictionary<string, object> _settings = new Dictionary<string, object>();
        static string _filePath;
        static bool _generatorPathChanged = false;
        
        public static string GeneratorPath
        {
            get
            {
                var value = GeneratorDirectory;
                value = Environment.ExpandEnvironmentVariables(value);
                return value;
            }
        }

        public static void Load(string filePath)
        {
            _filePath = filePath;
            if (File.Exists(filePath))
            {
                _settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(filePath));
                _generatorPathChanged = false;
            }
        }

        public static void Save()
        {
            if (string.IsNullOrWhiteSpace(_filePath)) throw new ArgumentNullException("FilePath");
            SaveSettings(_filePath);
        }

        public static void SaveSettings(string filePath)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(_settings));
            App.ChangeAppStyle();
            if (_generatorPathChanged)
            {
                var main = System.Windows.Application.Current.MainWindow.DataContext as ViewModels.MainWindowViewModel;
                main?.LoadGenerators();
            }
            _generatorPathChanged = false;
        }

        public static void Reload()
        {
            if (!string.IsNullOrWhiteSpace(_filePath))
            {
                Load(_filePath);
                _generatorPathChanged = false;
            }
        }

        public static void Reset()
        {
            _settings.Clear();
        }

        public static string Accent
        {
            get
            {
                return GetSetting<string>();
            }
            set
            {
                SetSetting(value);
            }
        }

        public static bool CheckUpdates
        {
            get
            {
                return GetSetting<bool>();
            }
            set
            {
                SetSetting(value);
            }
        }

        public static string EditorMRU
        {
            get
            {
                return GetSetting<string>();
            }
            set
            {
                SetSetting(value);
            }
        }

        public static string GeneratorDirectory
        {
            get
            {
                return GetSetting<string>();
            }
            set
            {
                SetSetting(value);
                _generatorPathChanged = true;
            }
        }

        public static bool IncludeBeta
        {
            get
            {
                return GetSetting<bool>();
            }
            set
            {
                SetSetting(value);
            }
        }

        public static bool IncludeGeneratorSubFolders
        {
            get
            {
                return GetSetting<bool>();
            }
            set
            {
                SetSetting(value);
            }
        }

        public static bool ShowSplash
        {
            get
            {
                return GetSetting<bool>();
            }
            set
            {
                SetSetting(value);
            }
        }

        public static string Theme
        {
            get
            {
                return GetSetting<string>();
            }
            set
            {
                SetSetting(value);
            }
        }        

        private static T GetAppSetting<T>(string key)
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                var value = ConfigurationManager.AppSettings[key];
                var converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromString(value);
            }
            else
            {
                return (T)(typeof(T).IsValueType ? Activator.CreateInstance(typeof(T)) : null);
            }
        }

        private static T GetSetting<T>([CallerMemberName] string propertyName = "")
        {
            object defaultValue = GetAppSetting<T>(propertyName);
            return GetSetting<T>((T)defaultValue, propertyName);
        }

        private static T GetSetting<T>(T defaultValue, [CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");
            if (_settings.ContainsKey(propertyName))
            {
                return (T)_settings[propertyName];
            }
            return defaultValue;
        }

        private static void SetSetting<T>(T value, [CallerMemberName] string propertyName = "")
        {
            _settings[propertyName] = value;
            OnStaticPropertyChanged(propertyName);
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void OnStaticPropertyChanged([CallerMemberName] string propertyName = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}
