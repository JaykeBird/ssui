using System;
using System.Globalization;
using Avalonia.Media;
using Avalonia.Data.Converters;
using Avalonia.Data;

namespace SolidShineUi
{
    /// <summary>
    /// Converts a Color into a SolidColorBrush.
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Convert a Color into a SolidColorBrush.
        /// </summary>
        /// <remarks>
        /// If the value is not of type <see cref="Color"/>, then a <see cref="BindingNotification"/> in an error state is returned.
        /// </remarks>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Color c)
            {
                return new SolidColorBrush(c);
            }
            else return new BindingNotification(new InvalidCastException("value is not of type Color"), BindingErrorType.Error);
        }

        /// <summary>
        /// Convert a SolidColorBrush into a Color.
        /// </summary>
        /// <remarks>
        /// If the value is not of type <see cref="ISolidColorBrush"/>, then a <see cref="BindingNotification"/> in an error state is returned.
        /// </remarks>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ISolidColorBrush s)
            {
                return s.Color;
            }
            else return new BindingNotification(new InvalidCastException("value is not of type ISolidColorBrush"), BindingErrorType.Error);
        }
    }
}
