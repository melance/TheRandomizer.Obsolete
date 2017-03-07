using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheRandomizer.Generators.Phonotactics
{
    public class Pattern
    {
        /// <summary>The pattern of definition keys and constants</summary>
        [XmlText]
        public string Value { get; set; }
        /// <summary>Weight of this pattern when selecting what pattern to use</summary>
        [XmlAttribute("weight")]
        public Int32 Weight { get; set; } = 1;
    }
}
