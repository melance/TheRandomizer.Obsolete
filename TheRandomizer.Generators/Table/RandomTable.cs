using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheRandomizer.Generators.Table
{
    [DisplayName("Random Table")]
    public class RandomTable : BaseTable
    {
        #region Properties
        /// <summary>This determines what modifier to apply to the die roll</summary>
        [XmlAttribute("randomModifier")]
        public string Modifier { get; set; }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Selects a random row from the table
        /// </summary>
        protected override Dictionary<string, object> ProcessTableInternal()
        {
            var max = Table.AsEnumerable().Max(r => Int32.Parse((string)r[Column]));
            var value = 0;
            var results = new Dictionary<string, object>();
            var modifier = 0;
            var count = GetRepeat();

            for (var i = 1; i <= count; i++)
            {
                DataRow selectedRow = null;
                var index = 0;

                modifier = GetModifier();

                value = Random.Next(Math.Abs(max + modifier)) * (max + modifier < 0 ? -1 : 1);
                while (index < Table.Rows.Count && selectedRow == null)
                {
                    var row = Table.Rows[index];
                    if (value < Int32.Parse((string)row[Column])) selectedRow = row;
                    index += 1;
                }

                if (selectedRow == null) selectedRow = Table.Rows[Table.Rows.Count - 1];

                ProcessRow(results, selectedRow);
            }

            return results;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets or evaluates the value of the modifier property
        /// </summary>
        private Int32 GetModifier()
        {
            if (string.IsNullOrWhiteSpace(Modifier)) return 0;
            return OnEvaluate<Int32>(Modifier);
        }
        #endregion
    }
}
