using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SolidShineUi
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

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
