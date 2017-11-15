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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using TheRandomizer.WinApp.Utility;

namespace TheRandomizer.WinApp.Views
{
    /// <summary>
    /// Interaction logic for GeneratorTabHost.xaml
    /// </summary>
    public partial class GeneratorTabHost : MetroWindow
    {
        public GeneratorTabHost()
        {
            InitializeComponent();
        }

        private InterTabClient _interTabClient;

        public InterTabClient InterTabClientInstance
        {
            get
            {
                if (_interTabClient == null) _interTabClient = new InterTabClient();
                return _interTabClient;
            }
            set
            {
                _interTabClient = value;
            }
        }

        private void MetroWindow_Deactivated(object sender, EventArgs e)
        {

        }
    }
}
