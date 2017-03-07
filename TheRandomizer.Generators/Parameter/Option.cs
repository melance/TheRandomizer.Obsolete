using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheRandomizer.Generators.Parameter
{
    public class Option
    {
        #region Members
        private string _displayName = "";
        #endregion

        #region Public Properties
        /// <summary>The friendly display name of the parameter option to show the user</summary>
        [XmlAttribute("display")]
        public string DisplayName 
        {
            get
            {
                return _displayName == "" ? Value : _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        /// <summary>The value to return if this option is selected</summary>
        [XmlText()]
        public string Value { get; set; }
        #endregion
    }
}
