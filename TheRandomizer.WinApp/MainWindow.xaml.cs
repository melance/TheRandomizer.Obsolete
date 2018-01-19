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
using TheRandomizer.WinApp.Models;
using TheRandomizer.WinApp.ViewModels;

namespace TheRandomizer.WinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {        
        private MainWindowViewModel Model
        {
            get
            {
                return (MainWindowViewModel)DataContext;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainWindowViewModel(DialogCoordinator.Instance);           
            DataContext = viewModel;
        }

        public void LoadGenerators()
        {
            Model.Generators = Models.GeneratorInfoCollection.GeneratorList;
        }

        private DateTime _lastKeyPress = DateTime.Now;
        private string _searchText;
                        
        private void lstGenerators_KeyUp(object sender, KeyEventArgs e)
        {            
            var diff = DateTime.Now - _lastKeyPress;
            char key;

            if (diff.TotalSeconds >= 2) _searchText = string.Empty;
            _lastKeyPress = DateTime.Now;

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Model.SelectGenerator.Execute(((GeneratorInfo)lstGenerators.SelectedItem).FilePath);
                e.Handled = true;
            }
            else
            {
                if (char.TryParse(e.Key.ToString(), out key))
                {
                    GeneratorInfo item;
                    _searchText += key;
                    item = Model.Generators.FirstOrDefault(gi => gi.Name.StartsWith(_searchText, StringComparison.InvariantCultureIgnoreCase));
                    if (item != null)
                    {
                        lstGenerators.SelectedValue = item;
                        lstGenerators.ScrollIntoView(lstGenerators.SelectedItem);
                    }
                    e.Handled = true;
                }
            }
        }
    }
}
