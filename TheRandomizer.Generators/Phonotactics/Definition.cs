using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Utility;

namespace TheRandomizer.Generators.Phonotactics
{
    public class Definition
    {
        private List<string> _valueList;

        /// <summary>The key use to identify this definition</summary>
        [XmlAttribute("key")]
        public string Key { get; set; }
        /// <summary>The delimiter used to separate individual items in the <see cref="Value"/> property</summary>
        /// <remarks>Defaults to ","</remarks>
        [XmlAttribute("delimiter")]
        public string Delimiter { get; set; } = ",";
        /// <summary>The delimited string of individual items</summary>
        [XmlText]
        public string Value { get; set; }

        [XmlIgnore]
        public List<string> ValueList
        {
            get
            {
                if (_valueList == null)
                {
                    _valueList = new List<string>(Value.Split(new string[] { Delimiter }, StringSplitOptions.None));                    
                }

                return _valueList;
            }
        }
    }
}
