using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheRandomizer.Generators.Parameter
{
    /// <summary>
    /// This is the class used to provide parameter information to the generators as well as to any clients
    /// using this library
    /// </summary>
    public class Configuration 
    {
        #region Enumerators
        /// <summary>
        /// Enumeration of the available types for a parameter
        /// </summary>
        public enum ParameterType
        {
            Text,
            Integer,
            Decimal,
            List,
            Date,
            MultiSelect
        }
        #endregion

        #region Members
        private string _displayName = null;
        #endregion

        #region Public Properties
        /// <summary>The name of the parameter, used to reference it</summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>The value of the parameter</summary>
        [XmlAttribute("value")]
        public string Value { get; set; }

        /// <summary>The friendly name to display to the user</summary>
        [XmlAttribute("display")]
        public string DisplayName { 
            get 
            {
                return string.IsNullOrWhiteSpace(_displayName) ? Name : _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        /// <summary>The type of this parameter</summary>
        [XmlAttribute("type")]
        public ParameterType Type { get; set; }
        
        /// <summary>A list of possible options for List and Multiselect parameter types</summary>
        [XmlElement("option")]
        public OptionList Options { get; set; } = new OptionList();

        [XmlElement("optionList")]
        [Display(Name = "Options")]
        public string OptionList
        {
            get
            {
                return Options.ToString();
            }
            set
            {
                if (value != null)
                {
                    Options = (OptionList)TypeDescriptor.GetConverter(this.Options.GetType()).ConvertFromString(value);
                }
                else
                {
                    Options = new OptionList();
                }
            }
        }

        #endregion
    }
}
