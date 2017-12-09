using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Utility
{
    public static class AttributeExtensions
    {
        public static bool HasAttribute(this Type type, Type attributeType) 
        {
            var attribute = Attribute.GetCustomAttribute(type, attributeType);
            return attribute != null;
        }

        public static bool HasAttribute(this PropertyInfo pi, Type attributeType)
        {
            var attribute = Attribute.GetCustomAttribute(pi, attributeType);
            return attribute != null;
        }

        public static string DisplayName(this Type type)
        {
            var attribute = type.GetCustomAttribute<DisplayNameAttribute>();
            if (attribute == null) return type.Name;
            return attribute.DisplayName;
        }
    }
}
