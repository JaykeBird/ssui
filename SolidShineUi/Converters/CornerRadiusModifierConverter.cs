using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SolidShineUi.Converters
{
    /// <summary>
    /// A converter to modify one <see cref="CornerRadius"/>, by adding on a constant value or another CornerRadius object.
    /// </summary>
    public class CornerRadiusModifierConverter : IValueConverter
    {
        /// <summary>
        /// Modify a <see cref="CornerRadius"/> to add on a constant value set in <paramref name="parameter"/>.
        /// </summary>
        /// <param name="value">The <see cref="CornerRadius"/> object to read from (or a <see cref="double"/> for a uniform value)</param>
        /// <param name="targetType">Not used, returned type will always be a <see cref="CornerRadius"/></param>
        /// <param name="parameter">a <see cref="double"/> or <see cref="CornerRadius"/> to add on to <paramref name="value"/></param>
        /// <param name="culture">Not used</param>
        /// <returns>A <see cref="CornerRadius"/> that only has a portion of its values set </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CornerRadius cr = new CornerRadius();
            CornerRadius cr2 = new CornerRadius();

            if (value is double d)
            {
                cr = new CornerRadius(d);
            }
            else if (value is CornerRadius c)
            {
                cr = new CornerRadius(c.TopLeft, c.TopRight, c.BottomRight, c.BottomLeft);
            }

            if (parameter is double dp)
            {
                cr2 = new CornerRadius(dp);
            }
            else if (parameter is CornerRadius cp)
            {
                cr2 = new CornerRadius(cp.TopLeft, cp.TopRight, cp.BottomRight, cp.BottomLeft);
            }

            return new CornerRadius(cr.TopLeft + cr2.TopLeft, cr.TopRight + cr2.TopRight, cr.BottomRight + cr2.BottomRight, cr.BottomLeft + cr2.BottomLeft);
        }

        /// <summary>
        /// Converting back is not supported, as there is not a way to return the data that is lost.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }
}
