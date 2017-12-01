using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Generators.Lexer;
using TheRandomizer.Utility;
using TheRandomizer.Generators.Exceptions;
using TheRandomizer.Generators.Attributes;
using System.Xml;
using System.IO;

namespace TheRandomizer.Generators.Assignment
{
    [XmlType("Assignment")]
    [GeneratorDisplay(Generators.GeneratorType.Assignment, "A highly customizable and general purpose generator useful for most content.")]
    public class AssignmentGenerator : BaseGenerator
    {
        #region Constants
        private const string START_ITEM = "Start";
        private const Int32 MAX_RECURSION_DEPTH = 100;
        private const Int32 MAX_LOOP_COUNT = 1000000;
        #endregion

        #region Static Methods
        public static AssignmentGenerator DeserializeLibrary(string xml)
        {
            var document = new XmlDocument();
            var generator = new AssignmentGenerator();
            using (var reader = new StringReader(xml))
            {
                document.Load(reader);
            }

            foreach (var item in document.GetElementsByTagName("item"))
            {
                var lineItem = new LineItem();
                var element = (XmlElement)item;
                lineItem.Name = element.GetAttribute("name");
                lineItem.Next = element.GetAttribute("next");
                lineItem.Variable = element.GetAttribute("variable");
                lineItem.Weight = element.GetAttribute("weight") == string.Empty ? 1 : Convert.ToInt32(element.GetAttribute("weight"));
                lineItem.Expression = element.InnerText;
                generator.LineItems.Add(lineItem);
            }
            generator.IsLibrary = true;
            return generator;
        }
        #endregion

        #region Constructors
        #endregion

        #region Members
        private Dictionary<string, CachedLineItems> _rulesByName = new Dictionary<string,CachedLineItems>(StringComparer.CurrentCultureIgnoreCase);
        private Int32 _recursionDepth = 0;
        private Int32 _loopCount = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Determines if this generator can be used as a library
        /// </summary>
        [XmlElement("isLibrary")]
        public bool IsLibrary { get; set; } = false;

        /// <summary>
        /// The list of line items used to generate the content
        /// </summary>
        [XmlArray("items")]
        [XmlArrayItem("item")]
        [RequireOneElement(ErrorMessage = "You must include at least one Line Item.")]
        public List<LineItem> LineItems { get; set; } = new List<LineItem>();

        /// <summary>
        /// An optional list of libraries to import into the generator
        /// </summary>
        [XmlArray("imports")]
        [XmlArrayItem("import")]
        public List<String> Imports { get; set; } = new List<String>();
        #endregion

        #region Protected Methods
        /// <summary>
        /// Generates the content
        /// </summary>
        /// <param name="maxLength">The maximum length of the returned content</param>
        /// <param name="parameters">The parameters supplied by the user</param>
        /// <returns>A randomly generated content string</returns>
        protected override string GenerateInternal(int? maxLength)
        {
            // Request Imports
            RequestImports();
            // Clear loop and recursion trackers
            _loopCount = 0;
            _recursionDepth = 0;
            // Find starting item
            LineItem item;
            var value = string.Empty;
            var next = START_ITEM;
            // Begin evaluation with the starting item
            do
            {
                item = ChooseItemByName(next);
                if (item != null)
                {
                    var repeat = 1;
                    if (!string.IsNullOrWhiteSpace(item.Repeat))
                    {
                        repeat = int.Parse(Calculate(item.Repeat));
                    }
                    for (var i = 0; i < repeat; i++)
                    {
                        value += Evaluate(item);
                    }
                    
                    next = item.Next;
                }
                else
                {
                    next = string.Empty;
                }
            } while (!string.IsNullOrWhiteSpace(next));
            
            // Return the generated result
            return value;
        }

        /// <summary>
        /// Requests the calling application provide the configured libraries
        /// </summary>
        /// <exception cref="LibraryNotFoundException">Thrown when the request returns a null library</exception>
        protected void RequestImports()
        {
            foreach (var import in Imports)
            {
                var e = new RequestGeneratorEventArgs(import);
                OnRequestGenerator(e);
                if (e.Generator != null)
                {
                    throw new Exceptions.LibraryNotFoundException(import);
                }
                else
                {
                    LineItems.AddRange(((AssignmentGenerator)e.Generator).LineItems);
                }
            }
        }
        
        /// <summary>
        /// Evaluates custom parameters for the NCalc engine
        /// </summary>
        /// <param name="name">Name of the parameter requested</param>
        /// <param name="e">Event arguments for the parameter request</param>
        protected override void EvaluateParameter(string name, NCalc.ParameterArgs e)
        {
            var item = ChooseItemByName(name);
            e.Result = Evaluate(item);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Generates a string value based on the configuration of a line item
        /// </summary>
        /// <param name="item">The line item to evaluate</param>
        /// <returns>A generated string, or <value>string.Empty</value> if the expression is empty, 
        /// the item is null, or the line item has a variable name configured</returns>
        /// <exception cref="MaxRecursionDepthExceededException">Too many levels of recursion reached</exception>
        /// <exception cref="MaxLoopCountException">The evaluate method was called too many times in one run.</exception>
        private string Evaluate(LineItem item)
        {
            _recursionDepth ++;
            _loopCount++;
            if (_recursionDepth > MAX_RECURSION_DEPTH)
            {
                throw new MaxRecursionDepthExceededException(MAX_RECURSION_DEPTH);
            }
            if (_loopCount > MAX_LOOP_COUNT)
            {
                throw new MaxLoopCountException(MAX_LOOP_COUNT);
            }
            // If the item is null, return an empty string
            if (item != null)
            {
                // if the expression is null, return an empty string
                if (!string.IsNullOrWhiteSpace(item.Expression))
                {
                    if (string.IsNullOrWhiteSpace(item.Variable))
                    {
                        // Evaluate the expression and return its value
                        _recursionDepth--;
                        return Evaluate(item.Expression);
                    }
                    else
                    {
                        // If this variable doesn't exist yet, add it.
                        if (!Variables.ContainsKey(item.Variable))
                        {
                            Variables.Add(item.Variable, string.Empty);
                        }
                        // Set the variable value
                        Variables[item.Variable] = Evaluate(item.Expression);
                    }
                }
            }
            _recursionDepth--;
            return string.Empty;
        }

        /// <summary>
        /// Parses and evaluates a string expression and returns the generated string
        /// </summary>
        /// <param name="expression">The string expression to evaluate</param>
        /// <returns>A generated string</returns>
        private string Evaluate(string expression)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                // Parse the tokens found in the string
                var tokens = new AssignmentLexer(expression).Lex().ToList();
                var value = new StringBuilder();
                foreach (var token in tokens)
                {
                    switch (token.TokenType)
                    {
                        case TokenType.String:
                            // String tokens are added to the value as is
                            value.Append(token.TokenValue);
                            break;
                        case TokenType.Item:
                            string name = token.TokenValue;
                            // Check the parameters for the name and replace if necessary
                            if (Parameters.Contains(token.TokenValue))
                            {
                                name = Parameters[token.TokenValue].Value;
                            }
                            // An item must be reevaluated to allow nested item names
                            name = Evaluate(name);
                            // Select a random item containing the name in the item
                            var item = ChooseItemByName(name);
                            // Add the evaluated value
                            value.Append(Evaluate(item));
                            break;
                        case TokenType.Variable:
                            // A variable is found in the base grammar variable list
                            if (Variables.ContainsKey(token.TokenValue))
                            {
                                value.Append(Variables[token.TokenValue]);
                            }
                            else if (Parameters.Contains(token.TokenValue))
                            {
                                value.Append(Parameters[token.TokenValue].Value);
                            }
                            break;
                        case TokenType.WhiteSpace:
                            // Currently whitespace is included in the string token
                            // this is here in case it is needed later
                            value.Append(token.TokenValue);
                            break;
                        case TokenType.Equation:
                            // Calculate the equation using the ncalc engine
                            value.Append(Calculate(token.TokenValue));
                            break;
                    }
                }
                return value.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Using the name provided, selects a random line item with that name
        /// </summary>
        /// <param name="name">The name of the line item to find</param>
        /// <returns>The randomly selected line item</returns>
        private LineItem ChooseItemByName(string name)
        {
            // Verify that there are rules
            if (LineItems != null && LineItems.Count() > 0)
            {
                // names are allowed to include an or operator "|", determine which one to use randomly
                var nameListOr = name.Split('|');

                if (nameListOr.Count() > 1) name = nameListOr[Random.Next(0, nameListOr.Count())];

                // If we haven't already cached this name, do so now to improve performance
                if (!_rulesByName.ContainsKey(name))
                {
                    var nameListAnd = name.Split('+');
                    _rulesByName.Add(name, new CachedLineItems());
                    _rulesByName[name].Rules = new List<LineItem>();
                    foreach (var nameItem in nameListAnd)
                    {
                        foreach (var item in LineItems.Where(li => li.Name.Equals(nameItem.Remove("[", "]"), StringComparison.CurrentCultureIgnoreCase)))
                        {
                            _rulesByName[name].TotalWeight += item.Weight;
                            _rulesByName[name].Rules.Add(item);
                        }
                    }
                }

                // Select a random item from those available
                if (_rulesByName[name].Rules.Count == 1)
                {
                    // There is only one item available so return it
                    return _rulesByName[name].Rules.First();
                }
                else if (_rulesByName[name].TotalWeight > 0)
                {
                    // Select a random rule using a weighted index
                    var sum = 0;
                    var index = -1;

                    // Select a random number that was not used previously
                    do
                    {
                        index = Random.Next(0, _rulesByName[name].TotalWeight) + 1;
                    } while (_rulesByName[name].Rules.Count() > 1 && index == _rulesByName[name].LastSelect);

                    // Record this result so we don't use it next time
                    _rulesByName[name].LastSelect = index;

                    // Find the item that has the index
                    foreach (var item in _rulesByName[name].Rules)
                    {
                        sum += item.Weight;
                        if (sum >= index) return item;
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
