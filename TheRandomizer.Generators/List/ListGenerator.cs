using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Utility;
using TheRandomizer.Generators.Attributes;
using System.ComponentModel.DataAnnotations;
using TheRandomizer.Generators.Parameter;

namespace TheRandomizer.Generators.List
{
    [XmlType("List")]
    [GeneratorDisplay(Generators.GeneratorType.List, "A simple generator that selects an item from a list of choices.")]
    public class ListGenerator : BaseGenerator
    {
        /// <summary>
        /// The newline delimited string of items to choose from
        /// </summary>
        [XmlElement("items")]
        [Required]
        public string Items { get { return GetProperty<string>(); } set { SetProperty(value); } }

        /// <summary>
        /// If true, maintaines whitespace at the beginning and end of the result
        /// </summary>
        [XmlElement("keepWhitespace")]
        [Display(Name = "Keep Whitespace")]
        public bool KeepWhitespace { get { return GetProperty(false); } set { SetProperty(value); } }

        [XmlIgnore]
        public override bool? SupportsMaxLength { get { return null; } set { } }

        [XmlIgnore]
        public override ConfigurationList Parameters { get; set; } = new ConfigurationList();

        /// <summary>
        /// Generates content by selecting a single item from the list
        /// </summary>
        protected override string GenerateInternal(int? maxLength)
        {
            if (!string.IsNullOrEmpty(Items))
            {
                var itemList = Items.SplitLines(false).ToList();
                var index = Random.Next(0, itemList.Count());
                var result = itemList[index];
                if (!KeepWhitespace) { result = result.Trim(); }
                return result;
            }
            return string.Empty;
        }
    }
}
