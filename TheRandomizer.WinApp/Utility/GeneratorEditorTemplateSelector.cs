using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TheRandomizer.Generators;
using TheRandomizer.WinApp.ViewModels;

namespace TheRandomizer.WinApp.Utility
{
    class GeneratorEditorTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AssignmentDataTemplate { get; set; }
        public DataTemplate DiceDataTemplate { get; set; }
        public DataTemplate ListDataTemplate { get; set; }
        public DataTemplate LuaDataTemplate { get; set; }
        public DataTemplate PhonotacticTemplate { get; set; }
        public DataTemplate TableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var generator = item as BaseGenerator;
            if (generator != null)
            {
                switch (generator.GeneratorType)
                {
                    case GeneratorType.Assignment: return AssignmentDataTemplate;
                    case GeneratorType.Dice: return DiceDataTemplate;
                    case GeneratorType.List: return ListDataTemplate;
                    case GeneratorType.Lua: return LuaDataTemplate;
                    case GeneratorType.Phonotactics: return PhonotacticTemplate;
                    case GeneratorType.Table: return TableTemplate;
                }
            }
            return base.SelectTemplate(generator, container);
        }
    }
}
