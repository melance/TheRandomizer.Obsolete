using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Attributes
{
    public class GeneratorDisplayAttribute : Attribute
    {
        public GeneratorDisplayAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }        
    }
}
