using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace SolidShineUi.Converters
{

    /// <summary>
    /// A XAML converter that writes the inputted value to console, and then returns the value. No actual conversion is performed.
    /// </summary>
    /// <remarks>
    /// Pass in an <see cref="IValueConverter"/> as a parameter to chain on that converter and have it convert the outputted value;
    /// any other item passed in as a parameter will be prepended to the written value to console.
    /// </remarks>
    public class DebugOutputValueConverter : IValueConverter
    {

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is IValueConverter converter)
            {
                PostItem(value);
                return converter.Convert(value, targetType, null, culture);
            }
            else
            {
                if (parameter != null)
                {
                    PostItem(parameter.ToString() + ": " + value.ToString());
                }
                else
                {
                    PostItem(value);
                }
                return value;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is IValueConverter converter)
            {
                PostItem(value);
                return converter.ConvertBack(value, targetType, null, culture);
            }
            else
            {
                if (parameter != null)
                {
                    PostItem(parameter.ToString() + ": " + value.ToString());
                }
                else
                {
                    PostItem(value);
                }
                return value;
            }
        }

        void PostItem(string s)
        {
            Debug.WriteLine(s);
        }

        void PostItem(object item)
        {
            PostItem(item.ToString() ?? "(null)");
        }
    }
}
