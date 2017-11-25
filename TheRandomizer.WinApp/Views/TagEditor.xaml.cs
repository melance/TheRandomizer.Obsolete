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
using System.Collections.ObjectModel;
using TheRandomizer.WinApp.Utility;

namespace TheRandomizer.WinApp.Views
{
    /// <summary>
    /// Interaction logic for TagEditor.xaml
    /// </summary>
    public partial class TagEditor : MetroWindow
    {
        public TagEditor()
        {
            InitializeComponent();
        }

        private ObservableCollection<string> Tags
        {
            get
            {
                if (DataContext == null || DataContext.GetType() != typeof(GeneratorWrapper)) return null;
                return ((GeneratorWrapper)DataContext).Tags;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox != null)
            {
                var item = txtBox.DataContext as string;
                if (item != null)
                {
                    var container = lstTags.ItemContainerGenerator.ContainerFromItem(item);
                    if (container != null)
                    {
                        var index = lstTags.ItemContainerGenerator.IndexFromContainer(container);
                        if (txtBox.Text != Tags[index])
                        {
                            Tags[index] = txtBox.Text;
                        }
                    }
                }
            }
        }
    }
}
