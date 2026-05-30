using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SolidShineUi.Converters
{

    /// <summary>
    /// Converts a fully-qualified file path into a 16x16 icon for display in XAML.
    /// </summary>
    [ValueConversion(typeof(string), typeof(BitmapSource))]
    public class FilePathToIconConverter : IValueConverter
    {

        /// <summary>
        /// Converts a file's full path to a 16x16 <see cref="BitmapSource"/> icon.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<string> ie)
            {
                string first = ie.FirstOrDefault() ?? "";
                return GetImageFromFilePath(first);
            }
            else if (value is string s)
            {
                try
                {
                    return GetImageFromFilePath(s);
                }
                catch (ArgumentNullException)
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        private static BitmapSource GetImageFromFilePath(string path)
        {
            return NativeMethods.GetSmallIcon(path);
        }

        /// <summary>
        /// This is not implemented. Returns <see cref="DependencyProperty.UnsetValue"/>.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
