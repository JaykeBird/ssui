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
    /// A helper method for WPF controls, to only selectively apply a <see cref="Thickness"/> value to only some sides.
    /// </summary>
    /// <remarks>
    /// For SolidShineUi 2.0 and later versions, all value converters are found in the <c>SolidShineUi.Converters</c> namespace.
    /// </remarks>
    [ValueConversion(typeof(Thickness), typeof(Thickness))]
    [ValueConversion(typeof(IConvertible), typeof(Thickness))]
    [ValueConversion(typeof(double), typeof(Thickness))]
    public class PartialThicknessConverter : IValueConverter
    {

        /// <summary>
        /// Modify a <see cref="Thickness"/> to only apply to a few sides, as based upon the values in <paramref name="parameter"/>.
        /// </summary>
        /// <param name="value">The <see cref="Thickness"/> object to read from (or a <see cref="double"/> or a <see cref="IConvertible"/> number for a uniform value)</param>
        /// <param name="targetType">Not used, returned type will always be a <see cref="Thickness"/></param>
        /// <param name="parameter">The sides to apply to the result; one or more of <c>L,T,B,R</c></param>
        /// <param name="culture">Used for converting an <see cref="IConvertible"/> object to a double</param>
        /// <returns>A <see cref="Thickness"/> that only has a portion of its values set </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            (bool left, bool top, bool right, bool bottom) vals = (false, false, false, false);
            Thickness baseVal;

            // first discern the value
            if (value is double d)
            {
                baseVal = new Thickness(d);
            }
            else if (value is IConvertible ic)
            {
                try
                {
                    baseVal = new Thickness(ic.ToDouble(culture));
                }
                catch (InvalidCastException)
                {
                    return new Thickness();
                }
            }
            else if (value is Thickness th)
            {
                baseVal = th;
            }
            else { return new Thickness(); }

            // then discern the parameter
            if (parameter is string s)
            {
                vals = PartialValueHelper.DecodeThicknessPartialValue(s);
            }
            else if (parameter is byte b)
            {
                vals = PartialValueHelper.DecodeThicknessPartialValue(b);
            }
            else
            {
                return baseVal;
            }

            // finally, do the "conversion"
            return new Thickness(vals.left ? baseVal.Left : 0, vals.top ? baseVal.Top : 0, vals.right ? baseVal.Right : 0, vals.bottom ? baseVal.Bottom : 0);
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