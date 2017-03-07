using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Table
{
    public class LoopTable : BaseTable
    {
        protected override Dictionary<string, object> ProcessTableInternal()
        {
            var results = new Dictionary<string, object>();
            var count = GetRepeat();

            // Repeat as many times as is requested by Repeat
            for (var i = 1; i <= count; i++)
            {
                // loop through each row of the table
                foreach (DataRow row in Table.Rows)
                {
                    // Get the id for this row based on the Column property
                    var id = row[Column].ToString();
                    foreach (DataColumn column in Table.Columns)
                    {
                        // Create the key for this row and column
                        var key = $"{id}.{column.ColumnName}";
                        // Get and evaluate the expression for this column
                        var expression = row[column.ColumnName].ToString();
                        object value = OnEvaluate<object>(expression);
                        // Store the results from the expression in teh results dictionary
                        if (results.ContainsKey(key))
                        {
                            results[key] = $"{results[key]}{RepeatJoin}{value}";
                        }
                        else
                        {
                            results.Add(key, value);
                        }
                    }
                }
            }

            return results;
        }
    }
}
