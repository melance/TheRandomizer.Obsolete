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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheRandomizer.Generators.Table;

namespace TheRandomizer.WinApp.Controls
{
    /// <summary>
    /// Interaction logic for GeneratorEditorLua.xaml
    /// </summary>
    public partial class GeneratorEditorTable : UserControl
    {
        public GeneratorEditorTable()
        {
            InitializeComponent();
        }

        private T FindAncestor<T>(DependencyObject reference) where T:DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(reference);

            while (parent != null && parent.GetType() != typeof(T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return (T)parent;
        }        
    }
}
