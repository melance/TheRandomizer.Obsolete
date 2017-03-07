using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Lexer
{
    internal class AssignmentLexer : LexerBase
    {
        public AssignmentLexer(string expression) : base(expression) { }

        protected override List<IMatcher> InitializeMatchList()
        {
            var matchers = new List<IMatcher>();

            matchers.Add(new MatchString("["));
            matchers.Add(new MatchVariable());

            return matchers;
        }
    }
}
