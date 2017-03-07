using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheRandomizer.Generators.Assignment
{
    public class LineItem
    {
        private Int32 _weight = 1;

        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlText()]
        public string Expression { get; set; }
        [XmlAttribute("next")]
        public string Next { get; set; }
        [XmlAttribute("weight")]
        public Int32 Weight { 
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
    }
}
