using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SolidShineUi.Converters
{
    /// <summary>
    /// Converts a double into a string for display in XAML, with rounding possible by setting the parameter to the decimal places to round to.
    /// </summary>
    public class DoubleStringConverter : IValueConverter
    {

        /// <summary>
        /// Convert a double into a string, with rounding possible by setting the <paramref name="parameter"/> value.
        /// </summary>
        /// <param name="value">The double object to convert</param>
        /// <param name="targetType">Not used, returned type will always be string</param>
        /// <param name="parameter">The number of decimals to use (integer)</param>
        /// <param name="culture">Not used</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int Decimals = (int)parameter;
                return Math.Round((double)value, Decimals).ToString(culture);
            }
            catch (InvalidCastException) // this means that the parameter is not set, and we won't round the input; still fine to have and use
            {
                return (value ?? 0.0d).ToString() ?? "0.0";
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Convert a string back into a double, by parsing the string.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return double.Parse(value as string ?? "0", culture);
            }
            catch (FormatException)
            {
                return DependencyProperty.UnsetValue;
            }
            //throw new NotImplementedException();
        }
    }
}
