using System;
using System.Globalization;
using System.Windows;
using Avalonia.Data.Converters;
using Avalonia.Data;

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
        /// <param name="parameter">The number of decimals to use (integer), or <c>null</c> to not round the input</param>
        /// <param name="culture">Used to set the culture used for coverrting to a string (see <see cref="double.ToString(IFormatProvider)"/>)</param>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                if (parameter is int decimals) // let's round the output
                {
                    try
                    {
                        return Math.Round(d, decimals).ToString(culture);
                    }
                    catch (ArgumentOutOfRangeException) // the decimal parameter inputted is either below 0 or above 15
                    {
                        return d.ToString(culture);
                    }
                }
                else
                {
                    return d.ToString(culture);
                }
            }
            else if (value is IConvertible ic)
            {
                try
                {
                    double dd = System.Convert.ToDouble(ic);
                    if (parameter is int decimals) // let's round the output
                    {
                        try
                        {
                            return Math.Round(dd, decimals).ToString(culture);
                        }
                        catch (ArgumentOutOfRangeException) // the decimal parameter inputted is either below 0 or above 15
                        {
                            return dd.ToString(culture);
                        }
                    }
                    else
                    {
                        return dd.ToString(culture);
                    }
                }
                catch (FormatException ex) { return new BindingNotification(new FormatException("value is not a double", ex), BindingErrorType.Error, "0");  }
                catch (InvalidCastException ex) { return new BindingNotification(new FormatException("value is not a double", ex), BindingErrorType.Error, "0"); }
                catch (OverflowException ex) { return new BindingNotification(new FormatException("value is not a double", ex), BindingErrorType.Error, "0"); }
            }
            else
            {
                return new BindingNotification(new FormatException("value is not a double"), BindingErrorType.Error, "0");
            }
        }

        /// <summary>
        /// Convert a string back into a double, by parsing the string.
        /// </summary>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                if (value is string s)
                {
                    return double.Parse(s, culture);
                }
                else
                {
                    return new BindingNotification(new FormatException("value cannot be parsed as a double"), BindingErrorType.Error, 0.0);
                }
            }
            catch (FormatException e)
            {
                return new BindingNotification(e, BindingErrorType.Error);
            }
        }
    }
}
