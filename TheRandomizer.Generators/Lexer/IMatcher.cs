using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Lexer
{
    internal interface IMatcher
    {
        Token IsMatch(Tokenizer tokenizer);
    }
}
