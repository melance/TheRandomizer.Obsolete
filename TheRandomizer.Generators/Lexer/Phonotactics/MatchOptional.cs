using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Lexer.Phonotactics
{
    internal class MatchOptional : IMatcher
    {
        public Token IsMatch(Tokenizer tokenizer)
        {
            string value;
            var type = TokenType.Character;

            if (tokenizer.Current == "(" && tokenizer.Peek(2) == ")")
            {
                tokenizer.Consume();
                value = tokenizer.Current;
                tokenizer.Consume();
                if (!tokenizer.End()) { tokenizer.Consume(); }
                type = TokenType.Optional;
            }
            else
            {
                value = tokenizer.Current;
                tokenizer.Consume();
            }

            return new Token(type, value);
        }
    }
}
