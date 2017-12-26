using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace TheRandomizer.WinApp.ViewModels
{
    sealed class AboutViewModel 
    {
        public class DetailListItem
        {
            public DetailListItem() { }
            public DetailListItem(string name, string description, string url, string urlText)
            {
                Name = name;
                Description = description;
                Url = url;
                UrlText = urlText;
            }

            public string Name { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string UrlText { get; set; }
        }

        private static Assembly TheRandomizer { get { return Assembly.GetExecutingAssembly(); } }
                
        public static string Title
        {
            get
            {
                return ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(TheRandomizer, typeof(AssemblyTitleAttribute)))?.Title;
            }
        }

        public static string Copyright
        {
            get
            {
                return ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(TheRandomizer, typeof(AssemblyCopyrightAttribute)))?.Copyright;
            }
        }

        public static string Version
        {
            get
            {
                return TheRandomizer.GetName().Version.ToString(2);
            }
        }

        public static string Description
        {
            get
            {
                return ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(TheRandomizer, typeof(AssemblyDescriptionAttribute)))?.Description;
            }
        }

        public static string Author
        {
            get
            {
                return "Lance Boudreaux";
            }
        }

        public static Uri LicenseURL
        {
            get
            {
                return new Uri("https://creativecommons.org/licenses/by-nc/4.0/");
            }
        }

        public static string License
        {
            get
            {
                return "Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)";
            }
        }        

        public static List<DetailListItem> Contributors
        {
            get
            {
                return new List<DetailListItem> { new DetailListItem("Lance Boudreaux", "Developer", "https://github.com/melance", "melance on GitHub"),
                                                  new DetailListItem("Hubert Tomaszewski","Icon Design","https://rndmnm.deviantart.com/", "rndmnm on Deviant Art") };
            }
        }

        public static List<DetailListItem> ThirdPartyComponents
        {
            get
            {
                return new List<DetailListItem> { new DetailListItem("NLua","Lua Generator", "https://github.com/NLua/NLua", "NLua on GitHub"),
                                                  new DetailListItem("NCalc","Calculations","https://github.com/sheetsync/NCalc","NCalc on GitHub"),
                                                  new DetailListItem("MahApps.Metro", "UI Styles", "https://mahapps.com/", "MahApps Home"),
                                                  new DetailListItem("Dragablz", "Tab Control", "https://dragablz.net/", "Dragablz Home"),
                                                  new DetailListItem("NLog", "Logging", "https://github.com/NLog/NLog", "NLog on GitHub"),
                                                  new DetailListItem("MathConverter","Binding Converter","https://github.com/hexinnovation/MathConverter","Math Converter on GitHub"),
                                                  new DetailListItem("Octokit","GitHub API", "https://github.com/octokit/octokit.net","Octokit on GitHub")};
            }
        }
    }
}
