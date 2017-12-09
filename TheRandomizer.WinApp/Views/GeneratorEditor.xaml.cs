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
using MahApps.Metro.Controls.Dialogs;
using TheRandomizer.WinApp.ViewModels;
using TheRandomizer.Generators;

namespace TheRandomizer.WinApp.Views
{
    /// <summary>
    /// Interaction logic for GeneratorEditor.xaml
    /// </summary>
    public partial class GeneratorEditor : MetroWindow
    {
        bool verifyClose = true;

        public GeneratorEditor()
        {
            InitializeComponent();
            DataContext = new GeneratorEditorViewModel(DialogCoordinator.Instance);
        }        

        public GeneratorEditor(BaseGenerator generator)
        {
            InitializeComponent();
            DataContext = new GeneratorEditorViewModel(generator, DialogCoordinator.Instance);
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (verifyClose)
            {
                if ((DataContext as GeneratorEditorViewModel)?.IsDirty == true)
                {
                    e.Cancel = true;
                    var cancel = await (DataContext as GeneratorEditorViewModel)?.CancelClosing();
                    if (cancel == false)
                    {
                        verifyClose = false;
                        Close();
                    }
                    else verifyClose = true;
                }
            }
        }
    }
}
