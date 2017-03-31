using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Utility;
using TheRandomizer.Generators.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TheRandomizer.Generators.List
{
    [XmlType("List")]
    [GeneratorDisplay("List Generator", "A simple generator that selects an item from a list of choices.")]
    public class ListGenerator : BaseGenerator
    {
        /// <summary>
        /// The newline delimited string of items to choose from
        /// </summary>
        [XmlElement("items")]
        [Required]
        public string Items { get; set; }

        /// <summary>
        /// If true, maintaines whitespace at the beginning and end of the result
        /// </summary>
        [XmlElement("keepWhitespace")]
        [Display(Name = "Keep Whitespace")]
        public bool KeepWhitespace { get; set; } = false;

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
