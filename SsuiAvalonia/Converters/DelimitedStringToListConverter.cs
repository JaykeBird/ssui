using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.Data.Converters;
using Avalonia.Data;
using static SolidShineUi.Utils.CsvLineHelper;
using Avalonia;

namespace SolidShineUi.Converters
{
    /// <summary>
    /// A converter to change a comma-separated string to a List of strings. Use the parameter to change the delimiter character.
    /// </summary>
    public class DelimitedStringToListConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string val = value?.ToString() ?? "";
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
                return new BindingNotification(new FormatException("value is not a parseable string"), BindingErrorType.Error);
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
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

            if (value is string[] array)
            {
                return ArrayToCsvLine(array, delimiter);
            }
            else if (value is ReadOnlyMemory<string> span)
            {
                return ArrayToCsvLine(span.Span, delimiter);
            }
            else if (value is IEnumerable<string> strings)
            {
                return ArrayToCsvLine([.. strings], delimiter);
            }
            else
            {
                return new BindingNotification(new FormatException("value is not a collection/enumerable of strings"), BindingErrorType.Error);
            }
        }
    }
}
