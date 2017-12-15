using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NCalc;
using TheRandomizer.Generators.Attributes;
using TheRandomizer.Utility.Collections;

namespace TheRandomizer.Generators.Table
{
    [XmlType("Table")]
    [GeneratorDisplay(Generators.GeneratorType.Table, "A generator meant to mimic the table format often found in random tables of rpg books.")]
    public class TableGenerator : BaseGenerator
    {
        /// <summary>
        /// The list of tables used to generate the content
        /// </summary>
        [XmlArray("tables")]
        [XmlArrayItem("loopTable", typeof(LoopTable))]
        [XmlArrayItem("randomTable", typeof(RandomTable))]
        [XmlArrayItem("selectTable", typeof(SelectTable))]
        public ObservableList<BaseTable> Tables { get; set; } = new ObservableList<BaseTable>();

        /// <summary>
        /// The formatted string used to output the results of the tables
        /// </summary>
        [XmlElement("output")]
        public string Output { get { return GetProperty<string>(); } set { SetProperty(value); } }

        /// <summary>
        /// A keyed list of the values calculated by the generator
        /// </summary>
        [XmlIgnore]
        private Dictionary<string, object> Values { get; } = new Dictionary<string, object>();

        [XmlIgnore]
        public override bool? SupportsMaxLength { get { return null; } set { } }

        protected override string GenerateInternal(int? maxLength)
        {
            Values.Clear();
            foreach (var table in Tables)
            {
                table.Evaluate += Evaluate;
                var results = table.ProcessTable();
                foreach (var result in results)
                {
                    Values.Add($"{table.Name}.{result.Key}", result.Value);
                }
                table.Evaluate -= Evaluate;
            }
            return FillOutput();
        }

        /// <summary>
        /// Evaluates a calculation for the Table classes
        /// </summary>
        private void Evaluate(object sender, EvaluateArgs e)
        {
            e.Result = Calculate(e.Expression);
        }

        /// <summary>
        /// Populate the output with the generated values 
        /// </summary>
        private string FillOutput()
        {
            var result = new StringBuilder();
            if (!string.IsNullOrEmpty(Output))
            {
                var lexer = new Lexer.AssignmentLexer(Output);

                foreach (var token in lexer.Lex().ToList())
                {
                    switch (token.TokenType)
                    {
                        case Lexer.TokenType.String: 
                        case Lexer.TokenType.WhiteSpace:
                            result.Append(token.TokenValue);
                            break;
                        case Lexer.TokenType.Item:
                        case Lexer.TokenType.Variable:
                        case Lexer.TokenType.Equation:
                            result.Append(Calculate($"[{token.TokenValue}]"));
                            break;
                    }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Handles NCalc custom parameters, returning the values from the generation
        /// </summary>
        protected override void EvaluateParameter(string name, ParameterArgs e)
        {
            if (Values.ContainsKey(name))
            {
                e.Result = Values[name];
            }
        }
    }
}
