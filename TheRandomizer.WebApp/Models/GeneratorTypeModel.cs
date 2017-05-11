using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheRandomizer.Generators;
using TheRandomizer.Generators.Attributes;

namespace TheRandomizer.WebApp.Models
{
    public class GeneratorTypeModel
    {
        public GeneratorTypeModel(Type type, string action)
        {
            Type = type;
            Action = action;
        }
        public string Action { get; private set; }
        public Type Type { get; private set; }
        public Func<BaseGenerator, BaseGenerator> PrepMethod { get; private set; }

        public string Name {
            get
            {
                var attribute = GeneratorDisplay;
                if (attribute == null) return Type.Name;
                return attribute.Name;
            }
        }
        public string Description {
            get
            {
                var attribute = GeneratorDisplay;
                if (attribute == null) return string.Empty;
                return attribute.Description;
            }
        }

        private GeneratorDisplayAttribute GeneratorDisplay
        {
            get
            {
                return (GeneratorDisplayAttribute)Attribute.GetCustomAttribute(Type, typeof(GeneratorDisplayAttribute));
            }
        }
    }
}