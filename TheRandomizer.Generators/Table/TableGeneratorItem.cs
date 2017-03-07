using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.VisualBasic.FileIO;

namespace TheRandomizer.Generators.Table
{
    /// <summary>
    /// Contains a single table in a table generator
    /// </summary>
    public class TableGeneratorItem
    {
        #region Constants
        private const string EXPRESSION_TOKEN = "=";
        private const string COMMENT_TOKEN = "#";
        #endregion

        #region Events
        /// <summary>Raised when the table needs to evaluate an NCalc expression</summary>
        internal event EventHandler<EvaluateArgs> Evaluate;
        #endregion

        #region Public Properties
        /// <summary>What action should be taken when processing this table</summary>
        [XmlAttribute("action")]
        public Actions Action { get; set; } = Actions.Random;
        /// <summary>The column value is handled differently by the different action types</summary>
        /// <remarks>
        /// Actions.Random : Which column contains the dice roll values
        /// Actions.Select : Which column contains the value to find
        /// Actions.Loop : Which column acts as the unique identifier for each row
        /// </remarks>
        [XmlAttribute("column")]
        public string Column { get; set; }
        /// <summary>For Actions.Random, this determines what modifier to apply to the die roll</summary>
        [XmlAttribute("randomModifier")]
        public string RandomModifier { get; set; }
        /// <summary>How many times should this table be processed</summary>
        [XmlAttribute("repeat")]
        public string Repeat { get; set; }
        /// <summary>This string will be used to join together multiple results when the table processing is repeated</summary>
        [XmlAttribute("repeatJoin")]
        public string RepeatJoin { get; set; } = " ";
        /// <summary>For Actions.Select, the value to find in Column</summary>
        [XmlAttribute("selectValue")]
        public string SelectValue { get; set; }
        /// <summary>The name of this table</summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        /// <summary>The string that delimits columns in the table</summary>
        [XmlAttribute("delimiter")]
        public string Delimiter { get; set; }
        /// <summary>An expression that determines if this table should be skipped</summary>
        [XmlAttribute("skipTable")]
        public string SkipTable { get; set; }
        /// <summary>The table itself</summary>
        [XmlText]
        public string Value { get; set; }
        #endregion

        #region Private Properties
        /// <summary>Stores the parsed table data</summary>
        [XmlIgnore]
        private DataTable Table { get; set; }
        
        /// <summary>A random number generator </summary>
        [XmlIgnore]
        private Random Random
        {
            get
            {
                if (_random == null) _random = new Random();
                return _random;
            }
        }
        #endregion

        #region Members
        private Random _random;
        #endregion

        #region Public Methods
        /// <summary>
        /// Performs all processing on the table to generate the results
        /// </summary>
        /// <returns>A dictionary with the results</returns>
        public Dictionary<string, object> ProcessTable()
        {
            if (!string.IsNullOrWhiteSpace(SkipTable) && OnEvaluate<bool>(SkipTable)) return new Dictionary<string, object>();
            ParseTable();
            switch (Action)
            {
                case Actions.Loop: return ProcessLoop();
                case Actions.Random: return ProcessRandom();
                case Actions.Select: return ProcessSelect();
            }
            return new Dictionary<string, object>();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Processes an Actions.Loop table
        /// </summary>
        private Dictionary<string, object> ProcessLoop()
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

        /// <summary>
        /// Processes an Actions.Random table
        /// </summary>
        private Dictionary<string, object> ProcessRandom()
        {
            var max = Table.AsEnumerable().Max(r => (Int32)r[Column]);
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
                    if (value < (Int32)row[Column]) selectedRow = row;
                    index += 1;
                }

                if (selectedRow == null) selectedRow = Table.Rows[Table.Rows.Count - 1];

                ProcessRow(results, selectedRow);
            }

            return results;
        }

        /// <summary>
        /// Processes an Actions.Select table
        /// </summary>
        private Dictionary<string, object> ProcessSelect()
        {
            object value;
            var result = new Dictionary<string, object>();
            DataRow row;
            
            value = OnEvaluate<Object>(SelectValue);
            
            row = Table.AsEnumerable().FirstOrDefault(r => r[Column].Equals(value));

            ProcessRow(result, row);
            return result;
        }

        /// <summary>
        /// Parses the delimited table into a datatable
        /// </summary>
        private void ParseTable()
        {
            if (Table == null)
            {
                var reader = new System.IO.StringReader(Value);
                var parser = new TextFieldParser(reader);
                parser.Delimiters = new string[] { Delimiter };
                parser.TextFieldType = FieldType.Delimited;
                parser.CommentTokens = new string[] { COMMENT_TOKEN };
                parser.HasFieldsEnclosedInQuotes = false;
                parser.TrimWhiteSpace = true;
                Table = new DataTable();
                Table.TableName = Name;

                var headers = parser.ReadFields();
                foreach (var header in headers)
                {
                    if (!string.IsNullOrWhiteSpace(header)) Table.Columns.Add(header);
                }

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    Table.Rows.Add(fields);
                }
            }
        }

        /// <summary>
        /// Process a data row into the result
        /// </summary>
        private void ProcessRow(Dictionary<string,object> result, DataRow row)
        {
            if (row != null)
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    var value = OnEvaluate<object>(row[Column]);
                    if (result.ContainsKey(column.ColumnName))
                    {
                        result[column.ColumnName] = $"{result[column.ColumnName]}{RepeatJoin}{value}";
                    }
                    else
                    {
                        result[column.ColumnName] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or evaluates the value of the repeat property
        /// </summary>
        private Int32 GetRepeat()
        {
            if (string.IsNullOrWhiteSpace(Repeat)) return 1;
            return OnEvaluate<Int32>(Repeat);
        }

        /// <summary>
        /// Gets or evaluates the value of the modifier property
        /// </summary>
        private Int32 GetModifier()
        {
            if (string.IsNullOrWhiteSpace(RandomModifier)) return 0;
            return OnEvaluate<Int32>(RandomModifier);
        }

        /// <summary>
        /// Evaluates the provided expression 
        /// </summary>    
        protected T OnEvaluate<T>(object expression)
        {
            var isString = expression.GetType() == typeof(string);

            if (isString && ((string)expression).StartsWith(EXPRESSION_TOKEN))
            {
                var e = new EvaluateArgs() { Expression = ((string)expression).Substring(1) };
                Evaluate(this, e);
                return (T)e.Result;
            }
            else
            {
                if (isString && typeof(T).GetMethod("Parse") != null)
                {
                    var method = typeof(T).GetMethod("Parse");
                    return (T)method.Invoke(null, new object[] { expression });
                }
                else
                {
                    return (T)Convert.ChangeType(expression, typeof(T));
                }
            }
        }
        #endregion  
    }
}
