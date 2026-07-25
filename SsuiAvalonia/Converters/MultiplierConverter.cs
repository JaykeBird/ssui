using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace SolidShineUi.Converters
{
    /// <summary>
    /// Takes a number and multiplies it by the amount in the parameter, and returns that value as a <see cref="double"/>.
    /// </summary>
    public class MultiplierConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            double dblVal;
            double dblParam;

            if (value is string vs)
            {
                if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblVal))
                {
                    value = dblVal;
                }
            }

            if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
            }

            try
            {
                dblVal = System.Convert.ToDouble(value, culture);
                dblParam = System.Convert.ToDouble(parameter, culture);
            }
            catch (FormatException ex)
            {
                return new BindingNotification(new FormatException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }
            catch (InvalidCastException ex)
            {
                return new BindingNotification(new InvalidCastException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }
            catch (OverflowException ex)
            {
                return new BindingNotification(new InvalidCastException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }

            return dblVal * dblParam;
        }

        /// <inheritdoc/>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            double dblVal;
            double dblParam;

            if (value is string vs)
            {
                if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblVal))
                {
                    value = dblVal;
                }
            }
            if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
            }

            try
            {
                dblVal = System.Convert.ToDouble(value, culture);
                dblParam = System.Convert.ToDouble(parameter, culture);
            }
            catch (FormatException ex)
            {
                return new BindingNotification(new FormatException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }
            catch (InvalidCastException ex)
            {
                return new BindingNotification(new InvalidCastException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }
            catch (OverflowException ex)
            {
                return new BindingNotification(new InvalidCastException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }

            return dblVal / dblParam;
        }
    }

    /// <summary>
    /// Takes a number or <see cref="Thickness"/> and multiplies it by the amount in the parameter, and returns that value as a <see cref="Thickness"/>.
    /// </summary>
    /// <remarks>
    /// For the conversions, the input value must be a <see cref="Thickness"/>, a string that can be parsed as a Thickness or a double,
    /// or an <see cref="IConvertible"/> that can be converted into a double (e.g., int, float, byte). Doubles are then turned into a uniform Thickness. 
    /// Any other values are treated as invalid, and a binding error is returned.
    /// <para/>
    /// The parameter must be a double, a string that can be parsed as a double, or an <see cref="IConvertible"/> that can be converted into a double. 
    /// If set to <c>null</c> (or not specifying the parameter), then the value <c>1</c> is assumed, causing the result to be the same as the input.
    /// <para/>
    /// Conversion back is also supported, by instead dividing by the parameter value rather than multiplying.
    /// </remarks>
    public class ThicknessMultiplierConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Thickness? val = null;
            double dblParam = 1.0;

            if (value is Thickness t)
            {
                val = t;
            }
            else if (value is string vs)
            {
                try
                {
                    val = Thickness.Parse(vs);
                }
                catch (FormatException ex)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new Thickness(dval);
                    }
                    else
                    {
                        return new BindingNotification(new FormatException("Unable to parse the input value as a Thickness.", ex), BindingErrorType.Error);
                    }
                }
            }
            else if (value is IConvertible vi)
            {
                try
                {
                    double dval = System.Convert.ToDouble(vi, culture);
                    val = new Thickness(dval);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the input value as a Thickness.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the input value as a Thickness.", ex), BindingErrorType.Error);
                }
            }

            if (parameter is double d)
            {
                dblParam = d;
            }
            else if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
                else
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double."), BindingErrorType.Error);
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
            }

            if (!val.HasValue) return new BindingNotification(new InvalidOperationException("Unable to parse the input value as a Thickness."), BindingErrorType.Error);

            return new Thickness(val.Value.Left * dblParam, val.Value.Top * dblParam, val.Value.Right * dblParam, val.Value.Bottom * dblParam);
        }

        /// <inheritdoc/>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Thickness? val = null;
            double dblParam = 1.0;

            if (value is Thickness t)
            {
                val = t;
            }
            else if (value is string vs)
            {
                try
                {
                    val = Thickness.Parse(vs);
                }
                catch (FormatException ex)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new Thickness(dval);
                    }
                    else
                    {
                        return new BindingNotification(new FormatException("Unable to parse the input value as a Thickness.", ex), BindingErrorType.Error);
                    }
                }
            }
            else if (value is IConvertible vi)
            {
                try
                {
                    double dval = System.Convert.ToDouble(vi, culture);
                    val = new Thickness(dval);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the input value as a Thickness.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the input value as a Thickness.", ex), BindingErrorType.Error);
                }
            }

            if (parameter is double d)
            {
                dblParam = d;
            }
            else if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
                else
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double."), BindingErrorType.Error);
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
            }

            if (!val.HasValue) return new BindingNotification(new InvalidOperationException("Unable to parse the input value as a Thickness."), BindingErrorType.Error);

            return new Thickness(val.Value.Left / dblParam, val.Value.Top / dblParam, val.Value.Right / dblParam, val.Value.Bottom / dblParam);
        }
    }

    /// <summary>
    /// Takes a number or <see cref="CornerRadius"/> and multiplies it by the amount in the parameter, and returns that value as a <see cref="CornerRadius"/>.
    /// </summary>
    /// <remarks>
    /// For the conversions, the input value must be a <see cref="CornerRadius"/>, a string that can be parsed as a CornerRadius or a double,
    /// or an <see cref="IConvertible"/> that can be converted into a double (e.g., int, float, byte). Doubles are then turned into a uniform CornerRadius. 
    /// Any other values are treated as invalid, and a binding error is returned.
    /// <para/>
    /// The parameter must be a double, a string that can be parsed as a double, or an <see cref="IConvertible"/> that can be converted into a double. 
    /// If set to <c>null</c> (or not specifying the parameter), then the value <c>1</c> is assumed, causing the result to be the same as the input.
    /// <para/>
    /// Conversion back is also supported, by instead dividing by the parameter value rather than multiplying.
    /// </remarks>
    public class CornerRadiusMultiplierConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            CornerRadius? val = null;
            double dblParam = 1.0;

            if (value is CornerRadius t)
            {
                val = t;
            }
            else if (value is string vs)
            {
                try
                {
                    val = CornerRadius.Parse(vs);
                }
                catch (FormatException ex)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new CornerRadius(dval);
                    }
                    else
                    {
                        return new BindingNotification(new FormatException("Unable to parse the input value as a CornerRadius.", ex), BindingErrorType.Error);
                    }
                }
            }
            else if (value is IConvertible vi)
            {
                try
                {
                    double dval = System.Convert.ToDouble(vi, culture);
                    val = new CornerRadius(dval);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the input value as a CornerRadius.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the input value as a CornerRadius.", ex), BindingErrorType.Error);
                }
            }

            if (parameter is double d)
            {
                dblParam = d;
            }
            else if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
                else
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double."), BindingErrorType.Error);
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
            }

            if (!val.HasValue) return new BindingNotification(new InvalidOperationException("Unable to parse the parameter as a CornerRadius."), BindingErrorType.Error);

            return new CornerRadius(val.Value.TopLeft * dblParam, val.Value.TopRight * dblParam, val.Value.BottomLeft * dblParam, val.Value.BottomRight * dblParam);
        }

        /// <inheritdoc/>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            CornerRadius? val = null;
            double dblParam = 1.0;

            if (value is CornerRadius t)
            {
                val = t;
            }
            else if (value is string vs)
            {
                try
                {
                    val = CornerRadius.Parse(vs);
                }
                catch (FormatException ex)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new CornerRadius(dval);
                    }
                    else
                    {
                        return new BindingNotification(new FormatException("Unable to parse the input value as a CornerRadius.", ex), BindingErrorType.Error);
                    }
                }
            }
            else if (value is IConvertible vi)
            {
                try
                {
                    double dval = System.Convert.ToDouble(vi, culture);
                    val = new CornerRadius(dval);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the input value as a CornerRadius.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the input value as a CornerRadius.", ex), BindingErrorType.Error);
                }
            }

            if (parameter is double d)
            {
                dblParam = d;
            }
            else if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
                else
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double."), BindingErrorType.Error);
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
            }

            if (!val.HasValue) return new BindingNotification(new InvalidOperationException("Unable to parse the input value as a CornerRadius."), BindingErrorType.Error);

            return new CornerRadius(val.Value.TopLeft / dblParam, val.Value.TopRight / dblParam, val.Value.BottomLeft / dblParam, val.Value.BottomRight / dblParam);
        }
    }

    /// <summary>
    /// Takes a number or <see cref="Point"/> and multiplies it by the amount in the parameter, and returns that value as a <see cref="Point"/>.
    /// </summary>
    /// <remarks>
    /// For the conversions, the input value must be a <see cref="Point"/>, a string that can be parsed as a Point or a double,
    /// or an <see cref="IConvertible"/> that can be converted into a double (e.g., int, float, byte). Doubles are then turned into a uniform Point. 
    /// Any other values are treated as invalid, and a binding error is returned.
    /// <para/>
    /// The parameter must be a double, a string that can be parsed as a double, or an <see cref="IConvertible"/> that can be converted into a double. 
    /// If set to <c>null</c> (or not specifying the parameter), then the value <c>1</c> is assumed, causing the result to be the same as the input.
    /// <para/>
    /// Conversion back is also supported, by instead dividing by the parameter value rather than multiplying.
    /// </remarks>
    public class PointMultiplierConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Point? val = null;
            double dblParam = 0.0;

            if (value is Point t)
            {
                val = t;
            }
            else if (value is string vs)
            {
                try
                {
                    val = Point.Parse(vs);
                }
                catch (FormatException ex)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new Point(dval, dval);
                    }
                    else
                    {
                        return new BindingNotification(new FormatException("Unable to parse the input value as a Point.", ex), BindingErrorType.Error);
                    }
                }
            }
            else if (value is IConvertible vi)
            {
                try
                {
                    double dval = System.Convert.ToDouble(vi, culture);
                    val = new Point(dval, dval);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the input value as a Point.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the input value as a Point.", ex), BindingErrorType.Error);
                }
            }

            if (parameter is double d)
            {
                dblParam = d;
            }
            else if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
                else
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double."), BindingErrorType.Error);
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
            }

            if (!val.HasValue) return new BindingNotification(new InvalidOperationException("Unable to parse the parameter as a Point."), BindingErrorType.Error);

            return new Point(val.Value.X * dblParam, val.Value.Y * dblParam);
        }

        /// <inheritdoc/>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Point? val = null;
            double dblParam = 0.0;

            if (value is Point t)
            {
                val = t;
            }
            else if (value is string vs)
            {
                try
                {
                    val = Point.Parse(vs);
                }
                catch (FormatException ex)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new Point(dval, dval);
                    }
                    else
                    {
                        return new BindingNotification(new FormatException("Unable to parse the input value as a Point.", ex), BindingErrorType.Error);
                    }
                }
            }
            else if (value is IConvertible vi)
            {
                try
                {
                    double dval = System.Convert.ToDouble(vi, culture);
                    val = new Point(dval, dval);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the input value as a Point.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the input value as a Point.", ex), BindingErrorType.Error);
                }
            }

            if (parameter is double d)
            {
                dblParam = d;
            }
            else if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
                else
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a Point."), BindingErrorType.Error);
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
            }

            if (!val.HasValue) return new BindingNotification(new InvalidOperationException("Unable to parse the input value as a Point."), BindingErrorType.Error);

            return new Point(val.Value.X / dblParam, val.Value.Y / dblParam);
        }
    }

    /// <summary>
    /// Takes a number or <see cref="RelativeScalar"/> and multiplies it by the amount in the parameter, and returns that value as a <see cref="RelativeScalar"/>.
    /// </summary>
    public class RelativeScalarMultiplierConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            double dblVal;
            double dblParam;
            RelativeUnit unit = RelativeUnit.Relative;

            if (value is RelativeScalar rs)
            {
                dblVal = rs.Scalar;
                unit = rs.Unit;
            }
            else if (value is string vs)
            {
                try
                {
                    RelativeScalar vrs = RelativeScalar.Parse(vs);
                    dblVal = vrs.Scalar;
                    unit = vrs.Unit;
                }
                catch (FormatException)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblVal))
                    {
                        value = dblVal;
                    }
                }
            }

            if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
            }

            try
            {
                dblVal = System.Convert.ToDouble(value, culture);
                dblParam = System.Convert.ToDouble(parameter, culture);
            }
            catch (FormatException ex)
            {
                return new BindingNotification(new FormatException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }
            catch (InvalidCastException ex)
            {
                return new BindingNotification(new InvalidCastException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }
            catch (OverflowException ex)
            {
                return new BindingNotification(new InvalidCastException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }

            return new RelativeScalar(dblVal * dblParam, unit);
        }

        /// <inheritdoc/>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            double dblVal;
            double dblParam;
            RelativeUnit unit = RelativeUnit.Relative;

            if (value is RelativeScalar rs)
            {
                dblVal = rs.Scalar;
                unit = rs.Unit;
            }
            else if (value is string vs)
            {
                try
                {
                    RelativeScalar vrs = RelativeScalar.Parse(vs);
                    dblVal = vrs.Scalar;
                    unit = vrs.Unit;
                }
                catch (FormatException)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblVal))
                    {
                        value = dblVal;
                    }
                }
            }

            if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
            }

            try
            {
                dblVal = System.Convert.ToDouble(value, culture);
                dblParam = System.Convert.ToDouble(parameter, culture);
            }
            catch (FormatException ex)
            {
                return new BindingNotification(new FormatException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }
            catch (InvalidCastException ex)
            {
                return new BindingNotification(new InvalidCastException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }
            catch (OverflowException ex)
            {
                return new BindingNotification(new InvalidCastException("Unable to parse the input or parameter value as a double.", ex), BindingErrorType.Error);
            }

            return new RelativeScalar(dblVal / dblParam, unit);
        }
    }

    /// <summary>
    /// Takes a number or <see cref="RelativePoint"/> and multiplies it by the amount in the parameter, and returns that value as a <see cref="RelativePoint"/>.
    /// </summary>
    /// <remarks>
    /// For the conversions, the input value must be a <see cref="RelativePoint"/>, a <see cref="Point"/>, a string that can be parsed as a Point or a double,
    /// or an <see cref="IConvertible"/> that can be converted into a double (e.g., int, float, byte). Doubles are then turned into a uniform Point. 
    /// Any other values are treated as invalid, and a binding error is returned.
    /// <para/>
    /// The parameter must be a double, a string that can be parsed as a double, or an <see cref="IConvertible"/> that can be converted into a double. 
    /// If set to <c>null</c> (or not specifying the parameter), then the value <c>1</c> is assumed, causing the result to be the same as the input.
    /// <para/>
    /// Conversion back is also supported, by instead dividing by the parameter value rather than multiplying.
    /// </remarks>
    public class RelativePointMultiplierConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            RelativePoint? val = null;
            double dblParam = 0.0;

            if (value is RelativePoint t)
            {
                val = t;
            }
            else if (value is Point at)
            {
                val = new RelativePoint(at, RelativeUnit.Relative);
            }
            else if (value is string vs)
            {
                try
                {
                    val = RelativePoint.Parse(vs);
                }
                catch (FormatException ex)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new RelativePoint(dval, dval, RelativeUnit.Relative);
                    }
                    else
                    {
                        return new BindingNotification(new FormatException("Unable to parse the input value as a RelativePoint.", ex), BindingErrorType.Error);
                    }
                }
            }
            else if (value is IConvertible vi)
            {
                try
                {
                    double dval = System.Convert.ToDouble(vi, culture);
                    val = new RelativePoint(dval, dval, RelativeUnit.Relative);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the input value as a RelativePoint.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the input value as a RelativePoint.", ex), BindingErrorType.Error);
                }
            }

            if (parameter is double d)
            {
                dblParam = d;
            }
            else if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
                else
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double."), BindingErrorType.Error);
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
            }

            if (!val.HasValue) return new BindingNotification(new InvalidOperationException("Unable to parse the parameter as a RelativePoint."), BindingErrorType.Error);

            return new RelativePoint(val.Value.Point.X * dblParam, val.Value.Point.Y * dblParam, val.Value.Unit);
        }

        /// <inheritdoc/>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            RelativePoint? val = null;
            double dblParam = 0.0;

            if (value is RelativePoint t)
            {
                val = t;
            }
            else if (value is Point at)
            {
                val = new RelativePoint(at, RelativeUnit.Relative);
            }
            else if (value is string vs)
            {
                try
                {
                    val = RelativePoint.Parse(vs);
                }
                catch (FormatException ex)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new RelativePoint(dval, dval, RelativeUnit.Relative);
                    }
                    else
                    {
                        return new BindingNotification(new FormatException("Unable to parse the input value as a RelativePoint.", ex), BindingErrorType.Error);
                    }
                }
            }
            else if (value is IConvertible vi)
            {
                try
                {
                    double dval = System.Convert.ToDouble(vi, culture);
                    val = new RelativePoint(dval, dval, RelativeUnit.Relative);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the input value as a RelativePoint.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the input value as a RelativePoint.", ex), BindingErrorType.Error);
                }
            }

            if (parameter is double d)
            {
                dblParam = d;
            }
            else if (parameter is string ps)
            {
                if (double.TryParse(ps, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out dblParam))
                {
                    parameter = dblParam;
                }
                else
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double."), BindingErrorType.Error);
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException ex)
                {
                    return new BindingNotification(new FormatException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
                catch (InvalidCastException ex)
                {
                    return new BindingNotification(new InvalidCastException("Unable to parse the parameter as a double.", ex), BindingErrorType.Error);
                }
            }

            if (!val.HasValue) return new BindingNotification(new InvalidOperationException("Unable to parse the parameter as a RelativePoint."), BindingErrorType.Error);

            return new RelativePoint(val.Value.Point.X / dblParam, val.Value.Point.Y / dblParam, val.Value.Unit);
        }
    }
}
