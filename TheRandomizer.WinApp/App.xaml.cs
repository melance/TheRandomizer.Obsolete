using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using NDesk.Options;
using TheRandomizer.Generators;

namespace TheRandomizer.WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const int SPLASH_DISPLAY_MILLISECONDS = 5000;

        static bool _showGUI = true;
        static int? _maxLength = null;
        static int _repeat = 1;
        static Dictionary<string, string> _parameters;

        private enum Mode
        {
            Standard,
            Editor,
            Test
        }
                
        private static Mode SetMode(string value)
        {
            var result = Mode.Standard;
            Enum.TryParse(value, true, out result);
            return result;
        }

        private static void PrintHelp(OptionSet p)
        {
            _showGUI = false;
            string help;
            using (var writer = new StringWriter())
            {
                p.WriteOptionDescriptions(writer);
                help = writer.ToString();
            }
            MessageBox.Show(help);
        }

        private static void GetMaxLength(string value)
        {
            if (int.TryParse(value, out int parsed))
            {
                _maxLength = parsed;
            }
        }

        private static void GetRepeat(string value)
        {
            if (!int.TryParse(value, out _repeat))
            {
                _repeat = 1;
            }
        }

        private static void GetParameter(string value)
        {
            if (value.Contains("="))
            {
                var parts = value.Split('=');
                if (_parameters == null) _parameters = new Dictionary<string, string>();
                _parameters.Add(parts[0], string.Join("=", parts.Skip(1)));
            }
        }

        private static void RunGenerator(string filePath)
        {
            _showGUI = false;
            filePath = Environment.ExpandEnvironmentVariables(filePath);
            if (!Path.IsPathRooted(filePath)) filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            if (!File.Exists(filePath)) throw new ArgumentException($"Could not find the file {filePath}.");
            var generator = BaseGenerator.Deserialize(File.ReadAllText(filePath));
            var tempFile = Path.GetTempFileName();
            var result = string.Empty;
            tempFile = Path.ChangeExtension(tempFile, "html");
            result = Utility.GeneratorWrapper.FormatResults(generator.Generate(_repeat, _maxLength), generator.CSS);
            File.WriteAllText(tempFile, result);
            Process.Start(tempFile);
        }

        public static void LoadCustomAccents()
        {
            if (Directory.Exists(@".\Accents\"))
            {
                foreach (var fileName in Directory.GetFiles(@".\Accents\", "*.xaml", SearchOption.TopDirectoryOnly))
                {
                    var uri = new Uri($"pack://application:,,,/{fileName}", UriKind.RelativeOrAbsolute);
                    ThemeManager.AddAccent(Path.GetFileNameWithoutExtension(fileName), uri);
                }
            }
        }

        public static void ChangeAppStyle()
        {
            var theme = ThemeManager.GetAppTheme(Utility.Settings.Theme);
            var accent = ThemeManager.GetAccent(Utility.Settings.Accent);
            if (theme != null && accent != null)
                ThemeManager.ChangeAppStyle(Current, accent, theme);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var help = false;
            var generatorPath = string.Empty;
            var selectedMode = Mode.Standard;
            var settingsPath = "settings.json";

            Boolean.TryParse(ConfigurationManager.AppSettings["PortableMode"], out bool portableMode);

            if (!portableMode)
            {
                settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                            Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName),
                                            settingsPath);
            }

            Utility.Settings.Load(settingsPath);

            var p = new OptionSet() {
                            { "m=|mode=", "Set the application mode, 'Standard' or 'Editor'", v => selectedMode = SetMode(v) },
                            { "ml=|maxlength=", "If used in conjunction with 'generate', provides the max length to the generator.", GetMaxLength },
                            { "r=|repeat=", "If used in conjunction with 'generate', provides the number of times to run the generator.", GetRepeat },
                            { "p=|parameter=", "If used in conjunction with 'generate', adds a parameter. The parameter takes the form Name=Value.", GetParameter },
                            { "g=|generate=", "Runs the provided generator and opens the results in your default web browser.", v => generatorPath = v },
                            { "?|help", "Shows this help.", v => help = true }};

            p.Parse(e.Args);

            if (!string.IsNullOrWhiteSpace(generatorPath)) RunGenerator(generatorPath);
            if (help) PrintHelp(p);
            
            if (_showGUI)
            {
                LoadCustomAccents();
                ChangeAppStyle();
                switch (selectedMode)
                {
#if DEBUG
                    case Mode.Test:
                        MainWindow = new Views.Test();
                        break;
#endif
                    case Mode.Editor:
                        MainWindow = new Views.GeneratorEditor();
                        break;
                    default:
                        MainWindow = new MainWindow();
                        break;
                }
                if (Utility.Settings.ShowSplash)
                {
                    var timer = new Stopwatch();
                    var splash = new Views.SplashScreen();

                    timer.Start();

                    splash.Show();

                    if (selectedMode != Mode.Editor)
                        Models.GeneratorInfoCollection.LoadGeneratorList(null);

                    timer.Stop();

                    var remainingSplashTime = SPLASH_DISPLAY_MILLISECONDS - timer.ElapsedMilliseconds;
                    if (remainingSplashTime > 0)
                        System.Threading.Thread.Sleep((int)remainingSplashTime);
                    MainWindow.Show();
                    splash.Close();
                }
                else
                {
                    MainWindow.Show();
                }
                if (MainWindow.GetType() == typeof(MainWindow)) ((MainWindow)MainWindow).LoadGenerators();
                //            else
                //            {
                //                Current.Shutdown();
            }
        }
    }
}
