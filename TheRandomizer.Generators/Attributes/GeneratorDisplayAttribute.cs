using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Attributes
{
    public class GeneratorDisplayAttribute : Attribute
    {
        public GeneratorDisplayAttribute(GeneratorType type, string description)
        {            
            Description = description;
            GeneratorType = type;
        }

        public string Name { get { return $"{GeneratorType} Generator"; } }
        public string Description { get; }        
        public GeneratorType GeneratorType { get; }
    }
}
