using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SolidShineUi.Converters
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
        /// If the value is not of type <see cref="Color"/>, then <see cref="DependencyProperty.UnsetValue"/> is returned.
        /// </remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
            {
                return new SolidColorBrush(c);
            }
            else return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Convert a SolidColorBrush to its Color.
        /// </summary>
        /// <remarks>
        /// If the value is not of type <see cref="SolidColorBrush"/>, then <see cref="DependencyProperty.UnsetValue"/> is returned.
        /// </remarks>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush s)
            {
                return s.Color;
            }
            else return DependencyProperty.UnsetValue;
        }
    }
}
