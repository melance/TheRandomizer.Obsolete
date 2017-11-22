using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Dice
{
    internal class DiceRollOption
    {
        /// <summary>What option does this reference</summary>
        public DiceRollOptions Option { get; set; }
        /// <summary>The value of the option.</summary>
        public Int32 Variable { get; set; }
        /// <summary>If true, the option has a variable</summary>
        public bool HasVariable { get; set; } = true;
        /// <summary>The default value of the variable</summary>
        public int? DefaultValue { get; set; } = null;
        /// <summary>If true, the option has a variable</summary>
        public bool VariableIsRequired { get; set; } = false;
    }
}
