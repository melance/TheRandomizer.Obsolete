using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Parameter
{
    [TypeConverter(typeof(OptionListConverter))]
    public class OptionList : List<Option>
    {
        public override string ToString()
        {
            return ToString(CultureInfo.InvariantCulture);
        }

        public string ToString(CultureInfo culture)
        {
            return TypeDescriptor.GetConverter(GetType()).ConvertToString(null, culture, this);
        }
    }
}
