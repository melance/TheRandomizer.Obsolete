using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheRandomizer.Generators.Dice
{
    public class RollFunction
    {
        /// <summary>The name to display to the user when selecting this roll function</summary>
        [XmlAttribute("displayName")]
        public string DisplayName { get; set; }
        /// <summary>The name of the function</summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        /// <summary>The contents of the function</summary>
        [XmlText]
        public string Function { get; set; }
    }
}
