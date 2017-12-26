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
using System.IO;
using TheRandomizer.WinApp.Utility;
using TheRandomizer.WinApp.Commands;
using TheRandomizer.WinApp.Views;
using System.Collections;

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
            LayoutRoot.DataContext = this;
        }
        #endregion

        #region Dependency Properties
        public static DependencyProperty GeneratorProperty = DependencyProperty.Register("Generator",
                                                                                         typeof(GeneratorWrapper),
                                                                                         typeof(GeneratorTabItem),
                                                                                         new PropertyMetadata(null));
        #endregion

        #region Properties
        public mshtml.IHTMLDocument2 Document
        {
            get
            {
                return webBrowser?.Document as mshtml.IHTMLDocument2;
            }
        }

        internal GeneratorWrapper Generator
        {
            get
            {
                var value = GetValue(GeneratorProperty);
                if (value == null) return null;
                return (GeneratorWrapper)value;
            }
            set
            {
                SetValue(GeneratorProperty, value);
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
            e.CanExecute = Generator != null && !string.IsNullOrEmpty(Generator.Results);
        }
        
        private void Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            Generator?.Cancel();
        }
        
        private void Clear(object sender, ExecutedRoutedEventArgs e)
        {
            Generator.Results = "<Html></Html>";
        }

        private void Copy(object sender, ExecutedRoutedEventArgs e)
        {
            Document?.execCommand("SelectAll");
            Document?.execCommand("Copy");
        }
        
        private void Generate(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Generator?.Generate();
            }
            catch (Exception ex)
            {
                Generator.Results = $"An error occured during generation:<br />{FormatException(ex)}";
            }
        }

        private string FormatException(Exception ex)
        {
            var message = new StringBuilder();
            message.AppendLine($"{ex.Message}<br />");
            foreach (DictionaryEntry data in ex.Data)
            {
                message.AppendLine($"{data.Key} = {data.Value}<br />");
            }
            return message.ToString();
        }

        private void Print(object sender, ExecutedRoutedEventArgs e)
        {
            //Document?.execCommand("Print", true, null);
        }

        private void CanPrint(object sender, CanExecuteRoutedEventArgs e)
        {   
            //if (Document?.queryCommandSupported("Print") == true && Document?.queryCommandEnabled("Print") == true)
            //{
            //    e.CanExecute = true;
            //}
            e.CanExecute = false;
        }

        private void Save(object sender, ExecutedRoutedEventArgs e)
        {
            Document?.execCommand("SaveAs", true);
        }
        
        private void SelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            Document?.execCommand("SelectAll");
        }

        private void SelectNone(object sender, ExecutedRoutedEventArgs e)
        {
            Document?.execCommand("Unselect");
        }
        #endregion
    }
}
