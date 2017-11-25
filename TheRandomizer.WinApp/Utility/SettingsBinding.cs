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
        private bool _generatorsPathChanged = false;
        
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
            Properties.Settings.Default.SettingsSaving += SettingsSaved;
            Source = Properties.Settings.Default;
            Mode = BindingMode.TwoWay;
        }

        private void SettingsSaved(object sender, CancelEventArgs e)
        {
            if (_generatorsPathChanged)
            {
                var main = Application.Current.MainWindow.DataContext as ViewModels.MainWindowViewModel;
                main?.LoadGenerators();
            }
            App.ChangeAppStyle();
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "GeneratorDirectory":
                    _generatorsPathChanged = true;
                    break;
            }
        }
    }
}
