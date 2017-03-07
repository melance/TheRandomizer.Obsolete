using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Lexer
{
    internal class MatchVariable : MatcherBase
    {
        private const string OPEN_VARIABLE = "[";
        private const string CLOSE_VARIABLE = "]";
        private const string EQUATION_TOKEN = "=";
        private const string VARIABLE_TOKEN = "*";
                
        public MatchVariable()
        {
        }

        protected override Token IsMatchImpl(Tokenizer tokenizer)
        {
            var str = new StringBuilder();
            var type = TokenType.Item;
            var openCount = 0;

            if (tokenizer.Current == OPEN_VARIABLE)
            {
                tokenizer.Consume();
                openCount = 1;

                if (tokenizer.Current == EQUATION_TOKEN)
                {
                    tokenizer.Consume();
                    type = TokenType.Equation;
                }
                if (tokenizer.Current == VARIABLE_TOKEN)
                {
                    tokenizer.Consume();
                    type = TokenType.Variable;
                }
                
                while (!tokenizer.End() && openCount != 0)
                {
                    switch (tokenizer.Current)
                    {
                        case OPEN_VARIABLE: openCount++;
                            break;
                        case CLOSE_VARIABLE: openCount--;
                            break;
                    }
                    if (openCount != 0)
                    {
                        str.Append(tokenizer.Current);
                        tokenizer.Consume();
                    }
                }

                if (tokenizer.Current == CLOSE_VARIABLE)
                {
                    tokenizer.Consume();
                }
            }

            if (str.Length > 0)
            {
                return new Token(type, str.ToString());
            }

            return null;
        }
    }
}
