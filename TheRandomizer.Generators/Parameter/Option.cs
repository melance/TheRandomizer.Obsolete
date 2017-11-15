using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Utility;

namespace TheRandomizer.Generators.Parameter
{
    public class Option : ObservableBase
    {
        #region Public Properties
        /// <summary>The friendly display name of the parameter option to show the user</summary>
        [XmlAttribute("display")]
        public string DisplayName { get { return GetProperty(Value); } set { SetProperty(value); } }

        /// <summary>The value to return if this option is selected</summary>
        [XmlText()]
        public string Value { get { return GetProperty<string>(); } set { SetProperty(value); } }
        #endregion
    }
}
