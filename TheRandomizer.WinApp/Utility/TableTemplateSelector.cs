using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TheRandomizer.Generators.Table;

namespace TheRandomizer.WinApp.Utility
{
    class TableTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LoopTableTemplate { get; set; }
        public DataTemplate RandomTableTemplate { get; set; }
        public DataTemplate SelectTableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var table = item as BaseTable;
            if (table != null)
            {
                if (table.GetType() == typeof(LoopTable)) return LoopTableTemplate;
                if (table.GetType() == typeof(RandomTable)) return RandomTableTemplate;
                if (table.GetType() == typeof(SelectTable)) return SelectTableTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
