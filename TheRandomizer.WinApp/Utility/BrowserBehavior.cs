using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace TheRandomizer.WinApp.Utility
{
    class BrowserBehavior
    {
        #region Dependency Properties
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached("Html",
                                                                                      typeof(string),
                                                                                      typeof(BrowserBehavior),
                                                                                      new FrameworkPropertyMetadata(OnHtmlChanged));
        #endregion

        #region Members
        private static Commands.DelegateCommand<string> _openURL = new Commands.DelegateCommand<string>(OpenURLAction);
        #endregion

        #region Static Properties
        public static Commands.DelegateCommand<string> OpenURL { get { return _openURL; } }
        #endregion

        #region Static Methods
        public static void OpenURLAction(string url)
        {
            if (url != null)
            {
                var info = new ProcessStartInfo(url);
                Process.Start(info);
            }
        }

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser browser)
        {
            return (string)browser.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser browser, string html)
        {
            browser.SetValue(HtmlProperty, html);
        }

        public static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = dependencyObject as WebBrowser;
            if (browser != null)
                browser.NavigateToString(e.NewValue as string ?? "No Results");
        }
        #endregion

    }
}
