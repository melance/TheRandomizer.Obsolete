using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TheRandomizer.WinApp.Converters
{
    class MarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var left = 0D;
            var top = 0D;
            var right = 0D;
            var bottom = 0D;

            switch (values.Count())
            {
                case 1:
                    left = System.Convert.ToDouble(values[0]);
                    top = left;
                    right = left;
                    bottom = left;
                    break;
                case 4:
                    left = System.Convert.ToDouble(values[0]);
                    top = System.Convert.ToDouble(values[1]);
                    right = System.Convert.ToDouble(values[2]);
                    bottom = System.Convert.ToDouble(values[3]);
                    break;
            }

            return new Thickness(left, top, right, bottom);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
