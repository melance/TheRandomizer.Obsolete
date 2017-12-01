using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheRandomizer.WinApp.Commands;


namespace TheRandomizer.WinApp.Controls
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {

        public class ColorView
        {
            public ColorView(string name,
                             string value,
                             Color color)
            {
                Name = name;
                Value = value;
                Color = color;
            }

            public string Name { get; set; }
            public string Value { get; set; }
            public Color Color { get; set; }
        }

        public Settings()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public ICommand ResetSettings
        {
            get
            {
                return new DelegateCommand(ResetSettingsAction);
            }
        }

        public ICommand SelectFolder
        {
            get
            {
                return new DelegateCommand(SelectFolderAction);
            }
        }

        public ICommand SaveSettings
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Properties.Settings.Default.Save();
                    var main = (Application.Current.MainWindow as MainWindow);
                    if (main != null)
                    {
                        main.flySettings.IsOpen = false;
                    }
                });
            }
        }

        public ICommand Cancel
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Properties.Settings.Default.Reload();
                    var main = (Application.Current.MainWindow as MainWindow);
                    if (main != null)
                    {
                        main.flySettings.IsOpen = false;
                    }
                });
            }
        }

        public List<ColorView> Themes
        {
            get
            {
                var value = new List<ColorView>();
                foreach (var theme in ThemeManager.AppThemes)
                {
                    var color = ((SolidColorBrush)theme.Resources["WindowBackgroundBrush"])?.Color;
                    value.Add(new ColorView(theme.Name.Replace("Base", ""), theme.Name, color ?? Colors.Gray));
                }
                return value.OrderBy(cv => cv.Name).ToList();
            }
        }

        public List<ColorView> Accents
        {
            get
            {
                const string ACCENT_BASE_COLOR = "AccentBaseColor";
                const string ACCENT_COLOR = "AccentColor";

                var value = new List<ColorView>();
                foreach (var accent in ThemeManager.Accents)
                {
                    Color color = Colors.Black;
                    if (accent.Resources[ACCENT_BASE_COLOR] != null)
                    {
                        color = (Color)accent.Resources[ACCENT_BASE_COLOR];
                    }
                    else if (accent.Resources[ACCENT_COLOR] != null)
                    {
                        color = (Color)accent.Resources[ACCENT_COLOR];
                    }
                    
                    value.Add(new ColorView(accent.Name, accent.Name, color));
                }
                return value.OrderBy(cv => cv.Name).ToList();
            }
        }

        private async void ResetSettingsAction()
        {
            var main = Application.Current.MainWindow as MetroWindow;
            var result = await main?.ShowMessageAsync("Confirm", "Are you sure you want to reset all settings?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
                Properties.Settings.Default.Reset();
        }

        private void SelectFolderAction()
        {             
            Properties.Settings.Default.GeneratorDirectory = Utility.Dialogs.OpenFolderDialog(Models.GeneratorInfoCollection.GeneratorPath);
        }        
    }
}
