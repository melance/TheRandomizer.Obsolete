using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Lexer.Phonotactics
{
    internal class MatchCharacter : IMatcher
    {
        public Token IsMatch(Tokenizer tokenizer)
        {
            if (!tokenizer.End() && tokenizer.Current != "(") return new Token(TokenType.Character, tokenizer.Current);
            return null;
        }
    }
}
