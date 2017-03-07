using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRandomizer.Utility;

namespace TheRandomizer.Generators.Lexer.DiceRolling
{
    internal class MatchNumber : MatcherBase
    {
        protected override Token IsMatchImpl(Tokenizer tokenizer)
        {
            var value = new StringBuilder();

            while (tokenizer.Current.IsNumeric() && !tokenizer.End())
            {
                value.Append(tokenizer.Current);
                tokenizer.Consume();
            }

            if (value.Length > 0)
            {
                return new Token(TokenType.Number, value.ToString());
            }

            return null; 
        }
    }
}
