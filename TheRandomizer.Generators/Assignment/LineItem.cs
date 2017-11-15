using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheRandomizer.Generators.Assignment
{
    public class LineItem
    {
        private int _weight = 1;

        [XmlAttribute("name")]
        [Required]
        public string Name { get; set; }
        [XmlText()]
        public string Expression { get; set; }
        [XmlAttribute("next")]
        public string Next { get; set; }
        [XmlAttribute("weight")]
        public int Weight { 
            get
            {
                return _weight;
            }
            set
            {
                if (value > 0)
                {
                    _weight = value;
                }
            }
        }
        [XmlAttribute("variable")]
        public string Variable { get; set; }
        [XmlAttribute("repeat")]
        public string Repeat { get; set; } 
    }
}
