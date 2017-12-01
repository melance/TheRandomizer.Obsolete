using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TheRandomizer.Generators;

namespace TheRandomizer.WinApp.Utility
{
    class GeneratorEditorTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ListDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var info = item as BaseGenerator;
            if (info != null)
            {
                switch (info.GeneratorType)
                {
                    case GeneratorType.List: return ListDataTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
