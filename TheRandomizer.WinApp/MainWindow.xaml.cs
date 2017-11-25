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
using System.Windows.Shapes;
using TheRandomizer.WinApp.ViewModels;

namespace TheRandomizer.WinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void mnuConvert_Click(object sender, RoutedEventArgs e)
        {
            var convert = new Views.ConvertGenerator();
            convert.Owner = this;
            convert.ShowDialog();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel)?.LoadGenerators();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
