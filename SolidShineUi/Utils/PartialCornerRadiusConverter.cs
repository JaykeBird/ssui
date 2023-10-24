using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A helper method for WPF controls, to only selectively apply a <see cref="CornerRadius"/> value to only some corners.
    /// </summary>
    public class PartialCornerRadiusConverter : IValueConverter
    {

        /// <summary>
        /// Convert a double into a string, with rounding possible by setting the <paramref name="parameter"/> value.
        /// </summary>
        /// <param name="value">The <see cref="CornerRadius"/> object to read from</param>
        /// <param name="targetType">Not used, returned type will always be a <see cref="CornerRadius"/></param>
        /// <param name="parameter">The corners to apply to the result; one or more of <c>TL,TR,BL,BR</c></param>
        /// <param name="culture">Not used</param>
        /// <returns>A <see cref="CornerRadius"/> that only has a portion of its values set </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CornerRadius cr)
            {
                if (parameter is string s)
                {
                    CornerRadius cr2 = new CornerRadius(0.0);

                    if (s.Contains("TL"))
                    {
                        cr2.TopLeft = cr.TopLeft;
                    }
                    if (s.Contains("TR"))
                    {
                        cr2.TopRight = cr.TopRight;
                    }
                    if (s.Contains("BL"))
                    {
                        cr2.BottomLeft = cr.BottomLeft;
                    }
                    if (s.Contains("BR"))
                    {
                        cr2.BottomRight = cr.BottomRight;
                    }

                    return cr2;
                }
                else
                {
                    return cr;
                }
            }
            else { return new CornerRadius(); }
        }

        /// <summary>
        /// Converting back is not supported, as there is not a way to return the data that is lost.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
