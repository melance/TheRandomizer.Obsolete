using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TheRandomizer.WinApp.Commands;

namespace TheRandomizer.WinApp.Controls
{
    /// <summary>
    /// Interaction logic for TokenEditor.xaml
    /// </summary>
    public partial class TokenEditor : UserControl
    {
        public static DependencyProperty TagsProperty = DependencyProperty.Register("Tags", typeof(ObservableCollection<string>), typeof(TokenEditor));

        public TokenEditor()
        {
            InitializeComponent();
            Tags = new ObservableCollection<string>() { "Hello", "World" };
            DataContext = this;
        }

        internal DelegateCommand<TagControl> DeleteTag
        {
            get
            {
                return new DelegateCommand<TagControl>(DeleteTagCommand);
            }
        }

        private void DeleteTagCommand(TagControl source)
        {

        }

        public ObservableCollection<string> Tags
        {
            get
            {
                return GetValue(TagProperty) as ObservableCollection<string>;
            }
            set
            {
                SetValue(TagProperty, value);
            }
        }

        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
