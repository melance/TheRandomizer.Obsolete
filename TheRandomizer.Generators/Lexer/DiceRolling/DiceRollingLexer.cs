using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace TheRandomizer.Generators.Lexer.DiceRolling
{
    internal class DiceRollingLexer : LexerBase
    {
        public DiceRollingLexer(string expression) : base(expression) { }

        protected override List<IMatcher> InitializeMatchList()
        {
            var matchers = new List<IMatcher>();

            matchers.Add(new MatchNumber());
            matchers.Add(new MatchKeyword(TokenType.Dice, "d"));
            matchers.Add(new MatchKeyword(TokenType.))
            matchers.Add(new MatchKeyword(TokenType.Add, "+"));
            matchers.Add(new MatchKeyword(TokenType.Subtract, "-"));
            matchers.Add(new MatchKeyword(TokenType.Colon, ":"));
            matchers.Add(new MatchKeyword(TokenType.Equals, "="));

            return matchers;
        }

    }
}
