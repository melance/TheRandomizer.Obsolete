using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators
{
    public enum GeneratorType
    {
        Unknown,
        Assignment,
        Dice,
        List,
        Lua,
        Phonotactics,
        Table
    }

    public enum OutputFormat
    {
        Text,
        Html
    }
    
    namespace Dice
    {
        public enum ExplodeMethod
        {
            None,
            Simple,
            Compound
        }

        public enum TargetType
        {
            None,
            LessThan,
            GreaterThan
        }

        public enum DiceRollOptions
        {
            GreaterThan,
            LessThan,
            Explode,
            CompoundExplode,
            DropLowest,
            DropHighest,
            RerollBelow,
            RerollAbove,
            RuleOfOne
        }
    }

    namespace Phonotactics
    {
        public enum TextCase
        {
            None,
            Lower,
            Upper,
            Title,
            Proper
        }

    }
}
