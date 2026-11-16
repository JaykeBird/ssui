using System;
using System.Globalization;
using Avalonia.Media;
using Avalonia.Data.Converters;
using Avalonia.Data;

namespace SolidShineUi.Converters
{
    /// <summary>
    /// Converts a string into a FontFamily.
    /// </summary>
    public class FontFamilyConverter : IValueConverter
    {
        /// <summary>
        /// Convert a string into a FontFamily with that name.
        /// </summary>
        /// <remarks>
        /// If the value is not of type <see cref="string"/>, then a <see cref="BindingNotification"/> in an error state is returned.
        /// </remarks>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return new FontFamily(s);
            }
            else return new BindingNotification(new InvalidCastException("value is not of type string"), BindingErrorType.Error);
        }

        /// <summary>
        /// Convert a FontFamily into a string.
        /// </summary>
        /// <remarks>
        /// If the value is not of type <see cref="FontFamily"/>, then a <see cref="BindingNotification"/> in an error state is returned.
        /// </remarks>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is FontFamily f)
            {
                return f.Name;
            }
            else return new BindingNotification(new InvalidCastException("value is not of type FontFamily"), BindingErrorType.Error);
        }
    }
}
