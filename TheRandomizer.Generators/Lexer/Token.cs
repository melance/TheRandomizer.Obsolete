using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Lexer
{
    internal enum TokenType
    {
        String,
        WhiteSpace,
        Item,
        Variable,
        Equation,
        EOF,
        Optional,
        Character
    }
    
    internal class Token
    {
        public TokenType TokenType { get; private set; }

        public string TokenValue { get; private set; }

        public Token(TokenType tokenType, string token)
        {
            TokenType = tokenType;
            TokenValue = token;
        }

        public Token(TokenType tokenType)
        {
            TokenType = tokenType;
            TokenValue = null;
        }

        public override string ToString()
        {
            return TokenType + ": " + TokenValue;
        }
    }
}
