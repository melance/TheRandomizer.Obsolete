using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Lexer
{
    internal abstract class LexerBase
    {
        private List<IMatcher> Matchers { get; set; }
        private Tokenizer Tokenizer { get; set; }

        public LexerBase(string source)
        {
            Tokenizer = new Tokenizer(source);
        }

        protected abstract List<IMatcher> InitializeMatchList();

        public IEnumerable<Token> Lex()
        {
            Matchers = InitializeMatchList();

            var current = Next();

            while (current != null && current.TokenType != TokenType.EOF)
            {
                yield return current;

                current = Next();
            }
        }

        private Token Next()
        {
            if (Tokenizer.End())
            {
                return new Token(TokenType.EOF);
            }

            return
                 (from match in Matchers
                  let token = match.IsMatch(Tokenizer)
                  where token != null
                  select token).FirstOrDefault();
        }
    }
}
