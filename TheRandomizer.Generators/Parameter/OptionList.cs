using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRandomizer.Utility.Collections;

namespace TheRandomizer.Generators.Parameter
{
    [TypeConverter(typeof(OptionListConverter))]
    public class OptionList : ObservableCollection<Option>
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
