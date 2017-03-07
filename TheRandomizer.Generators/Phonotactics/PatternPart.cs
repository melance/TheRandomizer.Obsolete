using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Phonotactics
{
    internal class PatternPart
    {
        /// <summary>The definition key for this pattern part</summary>
        public char Key { get; set; }
        /// <summary>If this is true, this pattern part can be skipped</summary>
        public bool Optional { get; set; }
    }
}
