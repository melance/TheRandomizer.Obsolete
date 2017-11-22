using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Generators.Parameter;
using TheRandomizer.Utility;
using TheRandomizer.Generators.Attributes;

namespace TheRandomizer.Generators.Dice
{
    [XmlType(TypeName = "Dice")]
    [GeneratorDisplay("Dice Generator", "A generator designed specifically to handle programmed dice rolling.")]
    public class DiceGenerator : BaseGenerator
    {
        #region Constants
        private const string ROLL_FUNCTION = "RollFunction";
        private const string ROLL_FUNCTION_DISPLAY = "Method";
        private const string COMMENT_INDICATOR = "//";
        private const string VARIABLE_ASSIGNMENT = ":=";
        #endregion

        #region Public Properties
        /// <summary>A list of dice roll functions the user can select from using a RollFunction parameter</summary>
        [XmlElement("function")]
        public List<RollFunction> Functions { get; set; } = new List<RollFunction>();

        /// <summary>A list of parameters to provide to the generator</summary>
        [XmlArray("parameters")]
        [XmlArrayItem("parameter")]
        public override ConfigurationList Parameters
        {
            get
            {
                if (Functions.Count > 1)
                {
                    var parameter = base.Parameters[ROLL_FUNCTION];
                    if (parameter == null)
                    {
                        parameter = new Configuration() { Name = ROLL_FUNCTION, DisplayName = ROLL_FUNCTION_DISPLAY, Type = Parameter.Configuration.ParameterType.List };
                        base.Parameters.Add(parameter);
                    }
                    foreach (var function in Functions)
                    {
                        if (parameter.Options.ToList().Find(o => o.Value == function.Name) == null)
                        {
                            parameter.Options.Add(new Option() { Value = function.Name, DisplayName = !string.IsNullOrWhiteSpace(function.DisplayName) ? function.DisplayName : function.Name });
                        }
                    }
                }
                else
                {
                    if (base.Parameters[ROLL_FUNCTION] != null) base.Parameters.Remove(base.Parameters[ROLL_FUNCTION]);
                }
                return base.Parameters;
            }
        }
        #endregion

        #region Private Properties
        /// <summary>Configures the Roll Function Parameter</summary>
        private Parameter.Configuration RollFunctionParameter
        {
            get
            {
                if (!Parameters.Contains(ROLL_FUNCTION))
                {
                    var p = new Parameter.Configuration() { Name = ROLL_FUNCTION, Type = Parameter.Configuration.ParameterType.List };

                    Parameters.Add(p);
                }
                return Parameters[ROLL_FUNCTION];
            }
        }
        #endregion

        #region Protected Methods
        protected override string GenerateInternal(int? maxLength)
        {
            var function = GetRollFunction();
            var lines = function.SplitLines(true);
            var results = new List<string>();

            foreach (var line in lines.Where(l => l != null))
            {
                var current = line.Trim();

                if (!string.IsNullOrWhiteSpace(current) && !current.StartsWith(COMMENT_INDICATOR))
                {
                    var parts = current.Split(new string[] { VARIABLE_ASSIGNMENT }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Count() == 1)
                    {
                        results.Add(Calculate(parts[0]));
                    }
                    else
                    {
                        parts[0] = parts[0].Trim();
                        if (!Variables.ContainsKey(parts[0])) Variables.Add(parts[0], string.Empty);
                        Variables[parts[0]] = Calculate(parts[1]);

                    }
                }
                else if (string.IsNullOrWhiteSpace(current))
                {
                    results.Add(string.Empty);
                }

            }

            return string.Join($"{Environment.NewLine}<br />", results.ToArray());

        }
        #endregion

        #region Private Methods
        private string GetRollFunction()
        {
            if (Functions.Count == 1) return Functions[0].Function;
            var rollFunction = Parameters[ROLL_FUNCTION];
            if (rollFunction == null)
            {
                return Functions.First().Function;
            }
            else
            {
                return Functions.First((RollFunction rf) => rf.Name == rollFunction.Value).Function;
            }
        }        
        #endregion
    }
}
