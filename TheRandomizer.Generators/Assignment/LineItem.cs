using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Utility;

namespace TheRandomizer.Generators.Assignment
{
    public class LineItem : ObservableBase
    {
        [XmlAttribute("name")]
        [Required]
        public string Name { get { return GetProperty<string>(); } set { SetProperty(value); } }
        [XmlText()]
        public string Expression { get { return GetProperty<string>(); } set { SetProperty(value); } }
        [XmlAttribute("next")]
        public string Next { get { return GetProperty<string>(); } set { SetProperty(value); } }
        [XmlAttribute("weight")]
        public int Weight { get { return GetProperty<int>(); } set { value = (value <= 0 ? 1 : value); SetProperty(value); } }
        [XmlAttribute("variable")]
        public string Variable { get { return GetProperty<string>(); } set { SetProperty(value); } }
        [XmlAttribute("repeat")]
        public string Repeat { get { return GetProperty<string>(); } set { SetProperty(value); } }
    }
}
