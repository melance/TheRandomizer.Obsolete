using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Lexer.Phonotactics
{
    internal class PhonotacticsLexer : LexerBase
    {
        public PhonotacticsLexer(string expression) : base(expression) { }

        protected override List<IMatcher> InitializeMatchList()
        {
            var matchers = new List<IMatcher>();

            matchers.Add(new MatchOptional());

            return matchers;
        }
    }
}
