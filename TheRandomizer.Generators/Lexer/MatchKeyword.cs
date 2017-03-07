using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace TheRandomizer.Generators.Lexer
{
    internal class MatchKeyword : MatcherBase
    {
        public string Match { get; set; }

        private TokenType TokenType { get; set; }

        public Boolean AllowAsSubString { get; set; }

        public List<MatchKeyword> SpecialCharacters { get; set; }

        public MatchKeyword(TokenType type, string match)
        {
            Match = match;
            TokenType = type;
            AllowAsSubString = true;
        }

        protected override Token IsMatchImpl(Tokenizer tokenizer)
        {
            foreach (var character in Match)
            {
                if (tokenizer.Current == character.ToString(CultureInfo.InvariantCulture))
                {
                    tokenizer.Consume();
                }
                else
                {
                    return null;
                }
            }

            bool found;

            if (!AllowAsSubString)
            {
                var next = tokenizer.Current;

                found = string.IsNullOrWhiteSpace(next) || SpecialCharacters.Any(c => c.Match == next);
            }
            else
            {
                found = true;
            }

            if (found)
            {
                return new Token(TokenType, Match);
            }

            return null;
        }
    }
}
