using Avalonia;
using Avalonia.Controls.Converters;
using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A helper method for Avalonia controls, to only selectively apply a <see cref="CornerRadius"/> value to only some corners.
    /// </summary>
    /// <remarks>
    /// This is similar to the <see cref="CornerRadiusFilterConverter"/> already present in Avalonia, but is a bit more flexible
    /// (and is compatible with the <c>PartialCornerRadiusConverter</c> used in SolidShineUi for WPF).
    /// </remarks>
    public class PartialCornerRadiusConverter : IValueConverter
    {

        /// <summary>
        /// Modify a <see cref="CornerRadius"/> to only apply to a few corners, as based upon the values in <paramref name="parameter"/>.
        /// </summary>
        /// <param name="value">The <see cref="CornerRadius"/> object to read from (or a <see cref="double"/> for a uniform value)</param>
        /// <param name="targetType">Not used, returned type will always be a <see cref="CornerRadius"/></param>
        /// <param name="parameter">The corners to carry over to the final output; one or more of strings <c>"TL,TR,BL,BR"</c>, or a <see cref="byte"/></param>
        /// <param name="culture">Not used</param>
        /// <returns>A <see cref="CornerRadius"/> that only has a portion of its values set </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            (bool topLeft, bool topRight, bool bottomLeft, bool bottomRight) vals = (false, false, false, false);
            CornerRadius baseVal;

            // first discern the value
            if (value is double d)
            {
                baseVal = new CornerRadius(d);
            }
            else if (value is CornerRadius cr)
            {
                baseVal = cr;
            }
            else { return new CornerRadius(); }

            // then discern the parameter
            if (parameter is string s)
            {
                vals = PartialValueHelper.DecodeCornerRadiusPartialValue(s);
            }
            else if (parameter is byte b)
            {
                vals = PartialValueHelper.DecodeCornerRadiusPartialValue(b);
            }
            else if (parameter is Corners c)
            {
                vals = PartialValueHelper.DecodeCornerRadiusPartialValue(c);
            }
            else
            {
                return baseVal;
            }

            // finally, do the "conversion"
            return new CornerRadius(vals.topLeft ? baseVal.TopLeft : 0, vals.topRight ? baseVal.TopRight : 0, vals.bottomRight ? baseVal.BottomRight : 0, vals.bottomLeft ? baseVal.BottomLeft : 0);
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
