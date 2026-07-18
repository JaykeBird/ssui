using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static SolidShineUi.Utils.CsvLineHelper;

namespace SolidShineUi.Converters
{
    /// <summary>
    /// A converter to change a comma-separated string to a List of strings. Use the parameter to change the delimiter character.
    /// </summary>
    [ValueConversion(typeof(string), typeof(List<string>))]
    public class DelimitedStringToListConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value.ToString() ?? "";
            if (string.IsNullOrEmpty(val))
            {
                return new List<string>() { "" };
            }

            char delimiter = ',';
            if (parameter is char c)
            {
                delimiter = c;
            }
            else if (parameter is string s && s.Length > 0)
            {
                delimiter = s[0];
            }

            if (CsvLineToArray(val, delimiter, out var list))
            {
                return list.ToList();
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            char delimiter = ',';
            if (parameter is char c)
            {
                delimiter = c;
            }
            else if (parameter is string s && s.Length > 0)
            {
                delimiter = s[0];
            }

            if (value is List<string> strings)
            {
                return ArrayToCsvLine(strings.ToArray(), delimiter);
            }
            else if (value is string[] array)
            {
                return ArrayToCsvLine(array, delimiter);
            }
#if NETCOREAPP
            else if (value is ReadOnlyMemory<string> span)
            {
                return ArrayToCsvLine(span.Span, delimiter);
            }
#endif
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
