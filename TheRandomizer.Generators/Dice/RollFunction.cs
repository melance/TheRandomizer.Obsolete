using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Display")]
        public string DisplayName { get; set; }
        /// <summary>The name of the function</summary>
        [XmlAttribute("name")]
        [Required]
        public string Name { get; set; }
        /// <summary>The contents of the function</summary>
        [XmlText]
        [Required]
        public string Function { get; set; }
    }
}
