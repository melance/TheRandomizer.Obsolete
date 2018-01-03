using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel.DataAnnotations;
using TheRandomizer.Utility;

namespace TheRandomizer.Generators.Table
{
    public abstract class BaseTable : ObservableBase
    {
        #region Static Methods
        public static DataTable StringToTable(string value)
        {
            var table = new DataTable();
            if (string.IsNullOrWhiteSpace(value)) return table;

            using (var reader = new System.IO.StringReader(value))
            {
                using (var parser = new TextFieldParser(reader))
                {
                    parser.Delimiters = new string[] { DELIMITER_TOKEN };
                    parser.TextFieldType = FieldType.Delimited;
                    parser.CommentTokens = new string[] { COMMENT_TOKEN };
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.TrimWhiteSpace = true;

                    var headers = parser.ReadFields();
                    foreach (var header in headers)
                    {
                        if (!string.IsNullOrWhiteSpace(header)) table.Columns.Add(header);
                    }

                    while (!parser.EndOfData)
                    {
                        var fields = parser.ReadFields();
                        table.Rows.Add(fields);
                    }
                }
            }
            return table;
        }

        public static string TableToString(DataTable value)
        {
            var builder = new StringBuilder();

            foreach (DataColumn column in value.Columns)
            {
                builder.Append($"{column.ColumnName} {DELIMITER_TOKEN}");
            }

            builder.AppendLine();

            foreach (DataRow row in value.Rows)
            {
                builder.AppendLine(string.Join(DELIMITER_TOKEN, row.ItemArray.Select(i => i.ToString())));
            }

            return builder.ToString();
        }
        #endregion

        #region Constants
        private const string EXPRESSION_TOKEN = "=";
        private const string COMMENT_TOKEN = "#";
        private const string DELIMITER_TOKEN = "|";
        #endregion

        #region Events
        /// <summary>Raised when the table needs to evaluate an NCalc expression</summary>
        internal event EventHandler<EvaluateArgs> Evaluate;
        #endregion

        #region Public Properties
        /// <summary>The column value is handled differently by the different action types</summary>
        [XmlAttribute("column")]
        public string Column { get { return GetProperty<string>(); } set { SetProperty(value); } }
        /// <summary>How many times should this table be processed</summary>
        [XmlAttribute("repeat")]
        public string Repeat { get { return GetProperty<string>(); } set { SetProperty(value); } }
        /// <summary>This string will be used to join together multiple results when the table processing is repeated</summary>
        [XmlAttribute("repeatJoin")]
        [Display(Name = "Repeat Join")]
        public string RepeatJoin { get { return GetProperty<string>(); } set { SetProperty(value); } }
        /// <summary>The name of this table</summary>
        [XmlAttribute("name")]
        public string Name { get { return GetProperty<string>(); } set { SetProperty(value); } }
        ///// <summary>The string that delimits columns in the table</summary>
        //[XmlAttribute("delimiter")]
        //public string Delimiter { get { return GetProperty<string>("|"); } set { SetProperty(value); } } 
        /// <summary>An expression that determines if this table should be skipped</summary>
        [XmlAttribute("skipTable")]
        public string SkipTable { get { return GetProperty<string>(); } set { SetProperty(value); } }
        /// <summary>The table itself</summary>
        [XmlText]
        [Display(Name = "Table")]
        public string Value { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string TableType
        {
            get
            {
                return GetType().DisplayName();
            }
        }
        #endregion

        #region Private Properties
        /// <summary>Stores the parsed table data</summary>
        [XmlIgnore]
        protected DataTable Table { get; set; }

        /// <summary>A random number generator </summary>
        [XmlIgnore]
        protected Random Random
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
            if (Table != null && Table.Rows.Count > 0)
            {
                return ProcessTableInternal();
            }
            return new Dictionary<string, object>();
        }
        #endregion

        #region Protected Methods
        protected abstract Dictionary<string, object> ProcessTableInternal();

        /// <summary>
        /// Process a data row into the result
        /// </summary>
        protected void ProcessRow(Dictionary<string, object> result, DataRow row)
        {
            if (row != null)
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    var value = OnEvaluate<object>(row[column.ColumnName]);
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
        protected Int32 GetRepeat()
        {
            if (string.IsNullOrWhiteSpace(Repeat)) return 1;
            return OnEvaluate<Int32>(Repeat);
        }
        
        /// <summary>
        /// Evaluates the provided expression 
        /// </summary>    
        protected T OnEvaluate<T>(object expression)
        {
            var isString = expression.GetType() == typeof(string);
            var value = expression;
            var parseMethod = typeof(T).GetMethod("Parse", new Type[] { typeof(string) });
            if (isString && ((string)expression).StartsWith(EXPRESSION_TOKEN))
            {
                var e = new EvaluateArgs() { Expression = ((string)expression).Substring(1) };
                Evaluate(this, e);
                value = e.Result;
            }
            
            if (isString && parseMethod != null)
            {
                return (T)parseMethod.Invoke(null, new object[] { value });
            }
            else
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Parses the delimited table into a datatable
        /// </summary>
        private void ParseTable()
        {
            if (Table == null)
            {
                Table = StringToTable(Value);
                Table.TableName = Name;                
            }
        }
        #endregion
    }
}
