using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Attributes
{
    public class RequireOneElementAttribute : ValidationAttribute
    {
        public override bool IsValid(dynamic value)
        {
            if (value != null)
            {
                return value.Count > 0;
            }
            return false;
        }
    }
}
