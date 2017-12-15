using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TheRandomizer.Generators;
using TheRandomizer.Generators.Table;
using TheRandomizer.WinApp.Commands;

namespace TheRandomizer.WinApp.Utility
{
    static class GeneratorEditorStaticMethods
    {        
        public static Func<object> NewDiceFunction
        {
            get
            {
                return () => new Generators.Dice.RollFunction();
            }
        }

        public static ICommand NewLoopTable
        {
            get
            {
                return new DelegateCommand<TableGenerator>(g => g.Tables.Add(new LoopTable() { Name = "Loop Table" }));
            }
        }

        public static ICommand NewRandomTable
        {
            get
            {
                return new DelegateCommand<TableGenerator>(g => g.Tables.Add(new RandomTable() { Name = "Random Table" } ));
            }
        }

        public static ICommand NewSelectTable
        {
            get
            {
                return new DelegateCommand<TableGenerator>(g => g.Tables.Add(new SelectTable() { Name = "Select Table" } ));
            }
        }
    }
}
