using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TheRandomizer.WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ChangeAppStyle();
            base.OnStartup(e);
        }

        public static void ChangeAppStyle()
        {
            var theme = ThemeManager.GetAppTheme(WinApp.Properties.Settings.Default.Theme);
            var accent = ThemeManager.GetAccent(WinApp.Properties.Settings.Default.Accent);
            if (theme != null && accent != null)
                ThemeManager.ChangeAppStyle(Current, accent, theme);
        }
    }
}
