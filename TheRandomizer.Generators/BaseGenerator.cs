using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NCalc;
using TheRandomizer.Generators.Parameter;
using System.Xml.Xsl;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.ComponentModel.DataAnnotations;
using TheRandomizer.Generators.Attributes;

namespace TheRandomizer.Generators
{


    /// <summary>
    /// This is the base class for all generator types.
    /// </summary>
    [XmlRoot("generator", Namespace="")]
    public abstract class BaseGenerator
    {
        #region Static Members
        private static List<Type> _knownTypes;
        #endregion

        #region Static Methods
        /// <summary>
        /// Deserializes the xml into a the appropriate generator object as the base type
        /// </summary>
        /// <param name="xml">The xml to deserialize</param>
        /// <returns>A generator object as the base type</returns>
        public static BaseGenerator Deserialize(string xml)
        {
            // Transform the document to the latest version
            xml = TransformXml.TransformToLatestVersion(xml);

            // Perform the serialization
            var serializer = new XmlSerializer(typeof(BaseGenerator), KnownTypes());
            using (var stream = XmlReader.Create(new StringReader(xml)))
            {
                return Deserialize(stream);
            }
        }

        /// <summary>
        /// Deserializes the xml reader into a the appropriate generator object as the base type
        /// </summary>
        /// <param name="reader">The xml reader to deserialize</param>
        /// <returns>A generator object as the base type</returns>
        public static BaseGenerator Deserialize(XmlReader reader)
        {
            var serializer = new XmlSerializer(typeof(BaseGenerator), KnownTypes());
            var generator = (BaseGenerator)serializer.Deserialize(reader);
            generator.MarkClean();
            return generator;
        }
        
        /// <summary>
        /// Returns the array of types that inherit from BaseGenerator
        /// </summary>
        /// <returns>An array of type that inherit from BaseGenerator</returns>
        static Type[] KnownTypes()
        {
            if (_knownTypes == null)
            {
                _knownTypes = new List<Type>(Assembly.GetExecutingAssembly().GetTypes().Where((Type t) => typeof(BaseGenerator).IsAssignableFrom(t)).ToArray());
            }
            return _knownTypes.ToArray();
        }
        #endregion

        #region Events
        /// <summary>Requests the calling application provide the configured generator</summary>
        public event EventHandler<RequestGeneratorEventArgs> RequestGenerator;
        #endregion

        #region Constants
        private const string GENERATE_FUNCTION = "Generate";
        private const Int32 LATEST_VERSION = 2;
        private const string TAG_DELIMITER = ", ";
        #endregion

        #region Constructors
        public BaseGenerator()
        {
            _cleanHash = GetHashCode();
        }
        #endregion

        #region Members
        private Random _random;
        private static Expression _calculator;
        private Int32 _cleanHash;
        #endregion

        #region Public Properties
        [XmlIgnore]
        public Int32 Id { get; set; }
        /// <summary>The name of the generator that is to be displayed to the user</summary>
        [XmlElement("name")]
        [Required]
        public string Name { get; set; }
        /// <summary>The identifying name of the author</summary>
        [XmlElement("author")]
        public string Author { get; set; }
        /// <summary>A long description of the purpose and output of the generator</summary>
        [XmlElement("description")]
        [Required]
        public string Description { get; set; }
        /// <summary>The version of the generator, provided for breaking changes</summary>
        [XmlAttribute("version")]
        public virtual Int32 Version { get; set; } = LATEST_VERSION;
        /// <summary>A URL that can be displayed to the user, such as the website of the author</summary>
        [XmlElement("url")]
        [Url]
        public string Url { get; set; }
        /// <summary>The format of the generator results</summary>
        [XmlElement("outputFormat")]
        [Display(Name = "Output Format")]
        public OutputFormat OutputFormat { get; set; } = OutputFormat.Html;
        /// <summary>A list of tags used to categorize the generator</summary>
        [XmlArray("tags")]
        [XmlArrayItem("tag")]
        public List<string> Tags { get; } = new List<string>();
        [XmlElement("tagList")]
        [Required]
        [Display(Name = "Tags")]
        public string TagList
        {
            get
            {
                return string.Join(TAG_DELIMITER, Tags);
            }
            set
            {
                if (value == null) { value = string.Empty; }
                Tags.AddRange(value.Split(new string[] { TAG_DELIMITER }, StringSplitOptions.RemoveEmptyEntries).Select<string, string>(t => t.Trim()));
            }
        }
        /// <summary>A boolean that determines if the generator supports limiting the length of the generated value</summary>
        [XmlElement("supportsMaxLength")]
        [Display(Name = "Supports Max Length")]
        public virtual bool SupportsMaxLength { get; set; }
        /// <summary>A list of parameters to provide to the generator</summary>
        [XmlArray("parameters")]
        [XmlArrayItem("parameter")]
        public virtual ConfigurationList Parameters { get; } = new ConfigurationList();

        [XmlIgnore]
        public virtual bool IsDirty { get { return _cleanHash == GetHashCode(); } }
        #endregion

        #region Protected Properties
        /// <summary>Returns a random number generator that lasts for the life of the object to ensure a good spread on random numbers</summary>
        protected Random Random
        {
            get
            {
                if (_random == null) _random = new Random();
                return _random;
            }
        }

        protected Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Calls the inheriting class' GenerateInteral one time with no parameters and no max length
        /// </summary>
        /// <returns>A generated value</returns>
        public virtual string Generate()
        {
            return Generate(1, null).First();
        }
        
        /// <summary>
        /// Calls the inheriting class' GenerateInternal as many times as specified with no max length or parameters
        /// </summary>
        /// <param name="repeat">The number of times to call GenerateInternal</param>
        /// <returns></returns>
        public virtual IEnumerable<string> Generate(Int32 repeat)
        {
            return Generate(repeat, null);
        }

        /// <summary>
        /// Calls the inheriting class' GenerateInternal as many times as is specified by the <paramref name="repeat">repeat</paramref> parameter.
        /// </summary>
        /// <param name="repeat">The number of times to call GenerateInternal</param>
        /// <param name="maxLength">The maximum length of each generated value, if supported.  If null then it is set to Int32.MaxLength.</param>
        /// <param name="parameters">A list of parameters to provide to the inheriting generator.</param>
        /// <returns>A list of generated values</returns>
        public virtual IEnumerable<string> Generate(Int32 repeat, Int32? maxLength)
        {
            var values = new List<string>();

            if (SupportsMaxLength && maxLength == 0) maxLength = Int32.MaxValue;

            AssignParameterValues();

            for (var i=0; i < repeat; i++)
            {
                values.Add(GenerateInternal(maxLength));
            }

            return values;
        }

        /// <summary>
        /// Serializes this generator as an XML string
        /// </summary>
        /// <returns>An xml string</returns>
        public string Serialize()
        {
            var serializer = new XmlSerializer(typeof(BaseGenerator), KnownTypes());
            string value;
            using (var stream = new StringWriter())
            {
                using (var xml = XmlWriter.Create(stream))
                {
                    Serialize(xml);
                }
                value = stream.ToString();
            }
            return value;
        }

        /// <summary>
        /// Serializes this generator to an Xml writer
        /// </summary>
        public void Serialize(XmlWriter xml)
        {
            var serializer = new XmlSerializer(typeof(BaseGenerator), KnownTypes());
            serializer.Serialize(xml, this);
        }

        public void MarkClean()
        {
            _cleanHash = GetHashCode();
        }

        public GeneratorInfo AsGeneratorInfo()
        {
            var generator = new GeneratorInfo();
            generator.Name = Name;
            generator.Author = Author;
            generator.Description = Description;
            generator.OutputFormat = OutputFormat;
            generator.SupportsMaxLength = SupportsMaxLength;
            generator.Tags.AddRange(Tags);
            generator.Url = Url;
            generator.Version = Version;
            return generator;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Implemented in the child generator, this is the method that produces the content
        /// </summary>
        /// <param name="maxLength">The maximum length of each generated value, if supported.  If null then it is set to Int32.MaxLength.</param>
        /// <param name="parameters">A list of parameters to provide to the inheriting generator.</param>
        /// <returns>The generated value</returns>
        protected abstract string GenerateInternal(Int32? maxLength);

        /// <summary>
        /// Runs the provided expression throug the NCalc engine
        /// </summary>
        /// <param name="expression">The expression to evaluate</param>
        /// <returns>The result of the calculation</returns>
        protected string Calculate(string expression)
        {
            object value;
            _calculator = new Expression(expression, EvaluateOptions.NoCache);
            _calculator.EvaluateFunction += OnEvaluateFunction;
            _calculator.EvaluateParameter += OnEvaluateParameter;
            value = _calculator.Evaluate();
            _calculator.EvaluateFunction -= OnEvaluateFunction;
            _calculator.EvaluateParameter -= OnEvaluateParameter;
            return value.ToString();
        }

        /// <summary>
        /// Called when the base generator doesn't isn't aware of the function named
        /// </summary>
        protected virtual void EvaluateFunction(string name, FunctionArgs e) { }

        /// <summary>
        /// Called when the base generator doesn't isn't aware of the parameter named
        /// </summary>
        protected virtual void EvaluateParameter(string name, ParameterArgs e) { }
        #endregion

        #region Private Methods
        /// <summary>
        /// Adds the parameters and their values to the variable list
        /// </summary>
        /// <param name="userParameters">The list of parameters from the user</param>
        private void AssignParameterValues()
        {
            if (Parameters != null)
            {
                // Add configured parameters
                foreach (var parameter in Parameters)
                {
                    if (!Variables.ContainsKey(parameter.Name))
                    {
                        Variables.Add(parameter.Name, parameter.Value);
                    }
                    else
                    {
                        Variables[parameter.Name] = parameter.Value;
                    }
                }                
            }
        }

        /// <summary>
        /// Handles custom functions for the NCalc engine
        /// </summary>
        private void OnEvaluateFunction(string name, FunctionArgs e)
        {
            if (CustomNCalcFunctions.Functions.ContainsKey(name))
            {
                CustomNCalcFunctions.Functions[name].Invoke(e);
            }
            else
            {
                switch (name)
                {
                    case GENERATE_FUNCTION:
                        var parameters = e.EvaluateParameters();
                        if (parameters.Count() > 0)
                        {
                            var arg = new RequestGeneratorEventArgs(parameters[0].ToString());
                            BaseGenerator generator;
                            RequestGenerator(this, arg);
                            generator = BaseGenerator.Deserialize(arg.Generator);
                            if (parameters.Count() > 1)
                            {
                                for (var i = 1; i < parameters.Count(); i += 2)
                                {
                                    var paramName = parameters[i].ToString();
                                    var maxLength = Int32.MaxValue;
                                    if (paramName == "MaxLength")
                                    {
                                        maxLength = Int32.Parse(parameters[i + 1].ToString());
                                    }
                                    else
                                    {
                                        generator.Parameters[paramName].Value = parameters[i + 1].ToString();
                                    }
                                }
                            }
                            e.Result = generator.Generate();
                        }
                        else
                        {
                            throw new NCalc.EvaluationException("Generate requires at least one parameter.");
                        }
                        break;
                    default:
                        EvaluateFunction(name, e);
                        break;
                }
            }
        }

        /// <summary>
        /// Handles custom parameters for the NCalc engine
        /// </summary>
        private void OnEvaluateParameter(string name, ParameterArgs e)
        {
            if (Variables != null && Variables.ContainsKey(name))
            {
                e.Result = Variables[name];
            }
            else
            {
                EvaluateParameter(name, e);
            }
        }
        #endregion
    }
}
