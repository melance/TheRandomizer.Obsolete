using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Generators.Lexer;
using System.Globalization;
using TheRandomizer.Generators.Attributes;
using System.ComponentModel.DataAnnotations;
using TheRandomizer.Generators.Parameter;

namespace TheRandomizer.Generators.Phonotactics
{
    [XmlType("Phonotactics")]
    [GeneratorDisplay(Generators.GeneratorType.Phonotactics, "A generator based on the concepts of phonotactics.")]
    public class PhonotacticsGenerator : BaseGenerator
    {
        /// <summary>Used to set the text casing of the result</summary>
        /// <remarks>
        /// None : Leave the text as is
        /// Lower : Change to lower case
        /// Upper : Change to UPPER CASE
        /// Title : Change to Title Case
        /// Propert : Change to Proper case
        /// </remarks>
        [XmlElement("textCase")]
        [Display(Name = "Text Case")]
        public TextCase TextCase { get; set; } = TextCase.None;

        /// <summary>List of Syllable Definitions</summary>
        [XmlArray("definitions")]
        [XmlArrayItem("item")]
        public List<Definition> Definitions { get; set; } = new List<Definition>();

        /// <summary>List of markov chain patterns</summary>
        [XmlArray("patterns")]
        [XmlArrayItem("item")]
        public List<Pattern> Patterns { get; set; } = new List<Pattern>();

        [XmlIgnore]
        public override bool? SupportsMaxLength { get { return null; } set { } }

        [XmlIgnore]
        public override ConfigurationList Parameters { get; set; } = new ConfigurationList();

        protected override string GenerateInternal(int? maxLength)
        {
            var pattern = SelectPattern();
            var tokens = new Lexer.Phonotactics.PhonotacticsLexer(pattern).Lex().ToList();
            var value = new StringBuilder();

            foreach (var token in tokens)
            {
                if (token.TokenType != TokenType.Optional || 
                    (token.TokenType == TokenType.Character && Random.Next(100) > 50))
                {
                    value.Append(GetValue(token.TokenValue));
                }
            }

            switch (TextCase)
            {
                case TextCase.Upper: return CultureInfo.CurrentCulture.TextInfo.ToUpper(value.ToString());
                case TextCase.Lower: return CultureInfo.CurrentCulture.TextInfo.ToLower(value.ToString());
                case TextCase.Title: return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToString());
                case TextCase.Proper: return CultureInfo.CurrentCulture.TextInfo.ToUpper(value.ToString()[0]) + CultureInfo.CurrentCulture.TextInfo.ToLower(value.ToString().Substring(1));
            }

            return value.ToString();
        }

        /// <summary>
        /// Randomly selects one of the configured patterns
        /// </summary>
        private string SelectPattern()
        {
            var max = Patterns.Sum(p => p.Weight);
            var selected = Random.Next(1, max + 1);
            var current = 0;

            foreach (var pattern in Patterns)
            {
                current += pattern.Weight;
                if (selected <= current)
                {
                    return pattern.Value;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Selects an item from the Definition identified by the provided key
        /// </summary>
        private string GetValue(string key)
        {
            var definition = Definitions.FirstOrDefault(d => d.Key == key);
            if (definition != null)
            {
                var index = Random.Next(0, definition.ValueList.Count() - 1);
                return definition.ValueList[index];
            }
            return key;
        }
    }
}
