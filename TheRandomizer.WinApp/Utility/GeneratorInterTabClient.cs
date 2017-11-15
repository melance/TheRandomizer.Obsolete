using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dragablz;

namespace TheRandomizer.WinApp.Utility
{
    public class InterTabClient : IInterTabClient
    {
        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var host = new Views.GeneratorTabHost();
            host.tabContainer.InterTabController = new InterTabController() { InterTabClient = this };
            return new NewTabHost<Window>(host, host.tabContainer);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            if (window.GetType() == typeof(MainWindow))
            {
                return TabEmptiedResponse.DoNothing;
            }
            else
            {
                return TabEmptiedResponse.CloseWindowOrLayoutBranch;
            }
        }
    }
}
