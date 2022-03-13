using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.IO;

namespace SolidShineUi.Utils
{
    public class FilePathToNameConverter : IValueConverter
    {

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

        private string GetFileNameFromFullPath(string path)
        {
            return Path.GetFileName(path);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
