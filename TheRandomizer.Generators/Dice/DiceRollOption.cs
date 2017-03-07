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
        /// <summary>If true, a variable must be included</summary>
        public bool VariableIsRequired { get; set; } = true;
    }
}
