using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using TheRandomizer.Generators.Table;

namespace TheRandomizer.WinApp.Converters
{
    class TableConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // string to DataTable
            if (value == null) return new DataTable();

            if (value is string table)
            {
                return BaseTable.StringToTable(table);
            }
            throw new ArgumentException("value must be a string");            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // DataTable to string
            if (value == null) return string.Empty;
            if (value is DataTable table)
            {
                try
                {
                    return BaseTable.TableToString(table);
                }
                catch
                {
                }
            }
            throw new ArgumentException("value must be a DataTable");
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
