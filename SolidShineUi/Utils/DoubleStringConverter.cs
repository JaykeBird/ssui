using System;
using System.Globalization;
using System.Windows.Data;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Converts a double into a string for display in XAML.
    /// </summary>
    public class DoubleStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int Decimals = (int)parameter;
            return Math.Round((double)value, Decimals).ToString();
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return double.Parse(value as string ?? "0");
            }
            catch (FormatException)
            {
                return 0d;
            }
            //throw new NotImplementedException();
        }
    }
}
