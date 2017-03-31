using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheRandomizer.Generators.Table
{
    [DisplayName("Select Table")]
    public class SelectTable : BaseTable
    {
        /// <summary>The value to find in Column</summary>
        [XmlAttribute("selectValue")]
        [Display(Name = "Select Value")]
        public string SelectValue { get; set; }

        protected override Dictionary<string, object> ProcessTableInternal()
        {
            object value;
            var result = new Dictionary<string, object>();
            DataRow row;

            value = OnEvaluate<Object>(SelectValue);

            row = Table.AsEnumerable().FirstOrDefault(r => r[Column].Equals(value));

            if (row != null)
            {
                ProcessRow(result, row);
            }
            return result;
        }
    }
}
