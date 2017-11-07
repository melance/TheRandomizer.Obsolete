using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TheRandomizer.WinApp.Commands
{
    public class GeneratorTabItemCommands
    {
        public static RoutedUICommand SelectNone
        {
            get
            {
                var gestures = new InputGestureCollection();
                gestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
                return new RoutedUICommand("Select None", "SelectNone", typeof(GeneratorTabItemCommands), gestures);
            }
        }
                
        public static RoutedUICommand Generate
        {
            get
            {
                var gestures = new InputGestureCollection();
                gestures.Add(new KeyGesture(Key.F5));
                return new RoutedUICommand("Generate", "Generate", typeof(GeneratorTabItemCommands), gestures);
            }
        }

        public static RoutedUICommand Cancel
        {
            get
            {
                return new RoutedUICommand("Cancel", "Cancel", typeof(GeneratorTabItemCommands));
            }
        }
    }
}
