using Avalonia;
using Avalonia.Controls.Converters;
using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A helper method for WPF controls, to only selectively apply a <see cref="CornerRadius"/> value to only some corners.
    /// </summary>
    public class PartialThicknessConverter : IValueConverter
    {

        /// <summary>
        /// Convert a double into a string, with rounding possible by setting the <paramref name="parameter"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Thickness"/> object to read from (or <see cref="double"/> for a uniform value)</param>
        /// <param name="targetType">Not used, returned type will always be a <see cref="Thickness"/></param>
        /// <param name="parameter">The corners to apply to the result; one or more of <c>L,T,B,R</c></param>
        /// <param name="culture">Not used</param>
        /// <returns>A <see cref="Thickness"/> that only has a portion of its values set </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            (bool left, bool top, bool right, bool bottom) vals = (false, false, false, false);
            Thickness baseVal;

            // first discern the value
            if (value is double d)
            {
                baseVal = new Thickness(d);
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
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
        }

    }
}
