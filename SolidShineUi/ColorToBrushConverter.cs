using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        /// <summary>
        /// Convert a SolidColorBrush to its Color. If the passed in value is not a SolidColorBrush, this returns null.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if NETCOREAPP
            if (value is SolidColorBrush s)
            {
                return s.Color;
            }
            else
            {
                return null!;
            }
#else
            if (value is SolidColorBrush s)
            {
                return s.Color;
            }
            else
            {
                return null;
            }
#endif
        }
    }
}
