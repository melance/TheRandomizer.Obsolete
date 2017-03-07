using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Parameter
{
    public class OptionListConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var list = new OptionList();
                var v = ((string)value).Split(new char[] { ',' });
                foreach (var item in v)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var itemValue = item;
                        var itemDisplay = item;
                        var option = new Option();

                        if (item.Contains('|'))
                        {
                            itemValue = item.Split(new char[] { '|' })[0];
                            itemDisplay = item.Split(new char[] { '|' })[1];
                        }

                        option.Value = itemValue.Trim();
                        option.DisplayName = itemDisplay.Trim();
                        list.Add(option);
                    }
                }

                return list;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var returnValue = new List<string>();
                var list = (OptionList)value;
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        var s = string.Empty;

                        if (item.DisplayName == item.Value)
                        {
                            s = item.Value;
                        }
                        else
                        {
                            s = $"{item.Value}|{item.DisplayName}";
                        }
                        returnValue.Add(s);
                    }
                }
                return string.Join(", ", returnValue);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
