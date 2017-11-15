using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace TheRandomizer.WinApp.Converters
{
    class IntegerToVisiblityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility) && value is int)
            {
                if ((int)value == 0)
                {
                    if (parameter is Visibility)
                    {
                        return parameter;
                    }
                    else
                    {
                        return Visibility.Hidden;
                    }
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
