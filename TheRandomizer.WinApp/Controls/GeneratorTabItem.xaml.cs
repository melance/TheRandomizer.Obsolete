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
using TheRandomizer.WinApp.ViewModels;
using TheRandomizer.Generators;

namespace TheRandomizer.WinApp.Controls
{
    /// <summary>
    /// Interaction logic for GeneratorTabItem.xaml
    /// </summary>
    public partial class GeneratorTabItem : UserControl
    {
        #region Constructor
        public GeneratorTabItem()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        public mshtml.IHTMLDocument2 Document
        {
            get
            {
                return webBrowser.Document as mshtml.IHTMLDocument2;
            }
        }

        public GeneratorTabItemViewModel Generator
        {
            get
            {
                return DataContext as GeneratorTabItemViewModel;
            }
        }
        #endregion

        #region Methods
        private void HasGenerator(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Generator != null;
        }

        private void HasResults(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(((GeneratorTabItemViewModel)DataContext).Results); ;
        }
        
        private void Cancel(object sender, RoutedEventArgs e)
        {
            Generator?.Cancel();
        }
        
        private void Clear(object sender, RoutedEventArgs e)
        {
            Document.clear();
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            Document.execCommand("Copy");
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            Generator?.Generate();
        }
        
        private void Print(object sender, RoutedEventArgs e)
        {
            Document.execCommand("Print", true);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Document.execCommand("Save", true);
        }
        
        private void SelectAll(object sender, RoutedEventArgs e)
        {
            Document.execCommand("SelectAll");
        }

        private void SelectNone(object sender, RoutedEventArgs e)
        {
            Document.execCommand("Unselect");
        }
        #endregion
    }
}
