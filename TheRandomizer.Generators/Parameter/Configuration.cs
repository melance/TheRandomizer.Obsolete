using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Utility;

namespace TheRandomizer.Generators.Parameter
{
    /// <summary>
    /// This is the class used to provide parameter information to the generators as well as to any clients
    /// using this library
    /// </summary>
    public class Configuration : ObservableBase
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
            Date
        }
        #endregion

        #region Constructors
        public Configuration() : base()
        {
            Options = new Parameter.OptionList();
            Options.CollectionChanged += OptionsChanged;
        }
        #endregion

        #region Members

        #endregion

        #region Public Properties

        /// <summary>The name of the parameter, used to reference it</summary>
        [XmlAttribute("name")]
        public string Name { get { return GetProperty<string>(); } set { SetProperty(value); } }

        /// <summary>The value of the parameter</summary>
        [XmlAttribute("value")]
        public string Value { get { return GetProperty<string>(); } set { SetProperty(value); } }

        /// <summary>The friendly name to display to the user</summary>
        [XmlAttribute("display")]
        public string DisplayName { get { return GetProperty(Name); } set { SetProperty(value); } }

        /// <summary>The type of this parameter</summary>
        [XmlAttribute("type")]
        public ParameterType Type { get { return GetProperty<ParameterType>(); } set { SetProperty(value); } }
        
        /// <summary>A list of possible options for List and Multiselect parameter types</summary>
        [XmlElement("option")]
        public OptionList Options { get { if (GetProperty<OptionList>() == null) SetProperty(new OptionList()); return GetProperty<OptionList>(); } set { SetProperty(value); } } 

        [XmlElement("optionList")]
        [Display(Name = "Options")]
        public string OptionList
        {
            get
            {
                return GetProperty<string>();
            }
            set
            {
                if (value != null)
                {
                    SetProperty((OptionList)TypeDescriptor.GetConverter(Options.GetType()).ConvertFromString(value), "Options");
                }
                else
                {
                    SetProperty(new OptionList(), "Options");
                }
            }
        }

        #endregion

        private void OptionsChanged(object sender, EventArgs e)
        {
            if (Options != null && Options.Count > 0)
            {
                Value = Options[0].Value;
            }
        }
    }
}
