using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MahApps.Metro;
using System.Windows;

namespace TheRandomizer.WinApp.Utility
{
    public class SettingBindingExtension : Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path) : base(path)
        {
            Initialize();
        }

        ~SettingBindingExtension()
        {
            Properties.Settings.Default.PropertyChanged -= PropertyChanged;
        }

        private void Initialize()
        {
            Properties.Settings.Default.PropertyChanged += PropertyChanged;
            Source = Properties.Settings.Default;
            Mode = BindingMode.TwoWay;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Properties.Settings.Default.Save();
            switch (e.PropertyName)
            {
                case "Theme":
                case "Accent":
                    App.ChangeAppStyle();
                    break;
                case "GeneratorDirectory":
                    var main = Application.Current.MainWindow.DataContext as ViewModels.MainWindowViewModel;
                    main?.LoadGenerators();
                    break;
            }
        }
    }
}
