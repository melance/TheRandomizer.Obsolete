using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
        private const int SPLASH_DISPLAY_MILLISECONDS = 5000;

        protected override void OnStartup(StartupEventArgs e)
        {
            ChangeAppStyle();
            var timer = new Stopwatch();
            var splash = new Views.SplashScreen();

            timer.Start();

            splash.Show();

            base.OnStartup(e);
            var mainWindow = new MainWindow();

            timer.Stop();

            var remainingSplashTime = SPLASH_DISPLAY_MILLISECONDS - timer.ElapsedMilliseconds;
            if (remainingSplashTime > 0)
                System.Threading.Thread.Sleep((int)remainingSplashTime);

            splash.Close();
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
