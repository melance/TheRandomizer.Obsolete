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

namespace TheRandomizer.WinApp.Controls
{
    /// <summary>
    /// Interaction logic for Tag.xaml
    /// </summary>
    public partial class TagControl : UserControl
    {
        public TagControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TagControl));
        public static DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(TagControl));
        
        public string Text
        {
            get
            {
                return GetValue(TextProperty) as string;
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public ICommand Command
        {
            get
            {
                return GetValue(CommandProperty) as ICommand;
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Command?.CanExecute(null) == true) Command?.Execute(this);
        }
    }
}
