using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Utility;

namespace TheRandomizer.Generators.Assignment
{
    public class Import : ObservableBase
    {
        public static implicit operator string(Import value)
        {
            return value.Value;
        }

        public static implicit operator Import(string value)
        {
            return new Import { Value = value };
        }
        
        [XmlText]
        public string Value { get { return GetProperty<string>(); } set { SetProperty(value); } }
    }
}
