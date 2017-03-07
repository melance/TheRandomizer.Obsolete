using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Lexer
{
    internal class MatchString : MatcherBase
    {

        public string Delimiters { get; private set; }

        public MatchString(string delimiters)
        {
            Delimiters = delimiters;
        }

        protected override Token IsMatchImpl(Tokenizer tokenizer)
        {
            var value = new StringBuilder();

            while (!Delimiters.Any(c => c.ToString() == tokenizer.Current) && !tokenizer.End())
            {
                if (tokenizer.Current == "\\")
                {
                    tokenizer.Consume();
                }
                if (!tokenizer.End())
                {
                    value.Append(tokenizer.Current);
                    tokenizer.Consume();
                }
            }

            if (value.Length > 0)
            {
                return new Token(TokenType.String, value.ToString());
            }

            return null;            
        }

    }
}
