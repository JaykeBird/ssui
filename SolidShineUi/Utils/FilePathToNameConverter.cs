using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.IO;

namespace SolidShineUi.Utils
{

    /// <summary>
    /// Convert a fully-qualified path to a file into just displaying the filename, for use in XAML.
    /// </summary>
    public class FilePathToNameConverter : IValueConverter
    {
        /// <summary>
        /// Convert a file's full path to only display the filename.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if NETCOREAPP
            if (value is IEnumerable<string> ie)
            {
                string first = ie.FirstOrDefault() ?? "";
                return GetFileNameFromFullPath(first);
            }
            else if (value is string s)
            {
                return GetFileNameFromFullPath(s);
            }
            else
            {
                return null!;
            }
#else
            if (value is IEnumerable<string> ie)
            {
                string first = ie.FirstOrDefault() ?? "";
                return GetFileNameFromFullPath(first);
            }
            else if (value is string s)
            {
                return GetFileNameFromFullPath(s);
            }
            else
            {
                return null;
            }
#endif
        }

        private static string GetFileNameFromFullPath(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// This is not implemented. Throws a <see cref="NotImplementedException"/>.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
