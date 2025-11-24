using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SolidShineUi.Converters
{
    /// <summary>
    /// Takes a number and multiplies it by the amount in the parameter, and returns that value as a <see cref="double"/>.
    /// </summary>
    public class MultiplierConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
            catch (FormatException)
            {
                return DependencyProperty.UnsetValue;
            }
            catch (InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
            }

            return dblVal * dblParam;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
            catch (FormatException)
            {
                return DependencyProperty.UnsetValue;
            }
            catch (InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
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
    /// Any other values are treated as invalid, and <see cref="DependencyProperty.UnsetValue"/> will be returned.
    /// <para/>
    /// The parameter must be a double, a string that can be parsed as a double, or an <see cref="IConvertible"/> that can be converted into a double. 
    /// If set to <c>null</c> (or not specifying the parameter), then the value <c>1</c> is assumed, causing the result to be the same as the input.
    /// <para/>
    /// Conversion back is also supported, by instead dividing by the parameter value rather than multiplying.
    /// </remarks>
    public class ThicknessMultiplierConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
                    ThicknessConverter tc = new ThicknessConverter();
#if NETCOREAPP
                    object? Maybeval = tc.ConvertFromString(null, culture, vs);
#else
                    object Maybeval = tc.ConvertFromString(null, culture, vs);
#endif
                    if (Maybeval is Thickness tt)
                    {
                        val = tt;
                    }
                    else { throw new NotSupportedException(); } // this can also be thrown by ThicknessConverter if it can't do the conversion
                }
                catch (NotSupportedException)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new Thickness(dval);
                    }
                    else
                    {
                        return DependencyProperty.UnsetValue;
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
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
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
                    return DependencyProperty.UnsetValue;
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            if (!val.HasValue) return DependencyProperty.UnsetValue;

            return new Thickness(val.Value.Left * dblParam, val.Value.Top * dblParam, val.Value.Right * dblParam, val.Value.Bottom * dblParam);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
                    ThicknessConverter tc = new ThicknessConverter();
#if NETCOREAPP
                    object? Maybeval = tc.ConvertFromString(null, culture, vs);
#else
                    object Maybeval = tc.ConvertFromString(null, culture, vs);
#endif
                    if (Maybeval is Thickness tt)
                    {
                        val = tt;
                    }
                    else { throw new NotSupportedException(); } // this can also be thrown by ThicknessConverter if it can't do the conversion
                }
                catch (NotSupportedException)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new Thickness(dval);
                    }
                    else
                    {
                        return DependencyProperty.UnsetValue;
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
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
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
                    return DependencyProperty.UnsetValue;
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            if (!val.HasValue) return DependencyProperty.UnsetValue;

            return new Thickness(val.Value.Left / dblParam, val.Value.Top / dblParam, val.Value.Right / dblParam, val.Value.Bottom / dblParam);
        }
    }

    /// <summary>
    /// Takes a number or <see cref="CornerRadius"/> and multiplies it by the amount in the parameter, and returns that value as a <see cref="CornerRadius"/>.
    /// </summary>
    /// <remarks>
    /// For the conversions, the input value must be a <see cref="CornerRadius"/>, a string that can be parsed as a CornerRadius or a double,
    /// or an <see cref="IConvertible"/> that can be converted into a double (e.g., int, float, byte). Doubles are then turned into a uniform CornerRadius. 
    /// Any other values are treated as invalid, and <see cref="DependencyProperty.UnsetValue"/> will be returned.
    /// <para/>
    /// The parameter must be a double, a string that can be parsed as a double, or an <see cref="IConvertible"/> that can be converted into a double. 
    /// If set to <c>null</c> (or not specifying the parameter), then the value <c>1</c> is assumed, causing the result to be the same as the input.
    /// <para/>
    /// Conversion back is also supported, by instead dividing by the parameter value rather than multiplying.
    /// </remarks>
    public class CornerRadiusMultiplierConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
                    CornerRadiusConverter tc = new CornerRadiusConverter();
#if NETCOREAPP
                    object? Maybeval = tc.ConvertFromString(null, culture, vs);
#else
                    object Maybeval = tc.ConvertFromString(null, culture, vs);
#endif
                    if (Maybeval is CornerRadius tt)
                    {
                        val = tt;
                    }
                    else { throw new NotSupportedException(); } // this can also be thrown by CornerRadiusConverter if it can't do the conversion
                }
                catch (NotSupportedException)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new CornerRadius(dval);
                    }
                    else
                    {
                        return DependencyProperty.UnsetValue;
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
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
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
                    return DependencyProperty.UnsetValue;
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            if (!val.HasValue) return DependencyProperty.UnsetValue;

            return new CornerRadius(val.Value.TopLeft * dblParam, val.Value.TopRight * dblParam, val.Value.BottomLeft * dblParam, val.Value.BottomRight * dblParam);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
                    CornerRadiusConverter tc = new CornerRadiusConverter();
#if NETCOREAPP
                    object? Maybeval = tc.ConvertFromString(null, culture, vs);
#else
                    object Maybeval = tc.ConvertFromString(null, culture, vs);
#endif
                    if (Maybeval is CornerRadius tt)
                    {
                        val = tt;
                    }
                    else { throw new NotSupportedException(); } // this can also be thrown by CornerRadiusConverter if it can't do the conversion
                }
                catch (NotSupportedException)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new CornerRadius(dval);
                    }
                    else
                    {
                        return DependencyProperty.UnsetValue;
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
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
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
                    return DependencyProperty.UnsetValue;
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            if (!val.HasValue) return DependencyProperty.UnsetValue;

            return new CornerRadius(val.Value.TopLeft / dblParam, val.Value.TopRight / dblParam, val.Value.BottomLeft / dblParam, val.Value.BottomRight / dblParam);
        }
    }

    /// <summary>
    /// Takes a number or <see cref="Point"/> and multiplies it by the amount in the parameter, and returns that value as a <see cref="Point"/>.
    /// </summary>
    /// <remarks>
    /// For the conversions, the input value must be a <see cref="Point"/>, a string that can be parsed as a Point or a double,
    /// or an <see cref="IConvertible"/> that can be converted into a double (e.g., int, float, byte). Doubles are then turned into a uniform Point. 
    /// Any other values are treated as invalid, and <see cref="DependencyProperty.UnsetValue"/> will be returned.
    /// <para/>
    /// The parameter must be a double, a string that can be parsed as a double, or an <see cref="IConvertible"/> that can be converted into a double. 
    /// If set to <c>null</c> (or not specifying the parameter), then the value <c>1</c> is assumed, causing the result to be the same as the input.
    /// <para/>
    /// Conversion back is also supported, by instead dividing by the parameter value rather than multiplying.
    /// </remarks>
    public class PointMultiplierConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
                    PointConverter tc = new PointConverter();
#if NETCOREAPP
                    object? Maybeval = tc.ConvertFromString(null, culture, vs);
#else
                    object Maybeval = tc.ConvertFromString(null, culture, vs);
#endif
                    if (Maybeval is Point tt)
                    {
                        val = tt;
                    }
                    else { throw new NotSupportedException(); } // this can also be thrown by PointConverter if it can't do the conversion
                }
                catch (NotSupportedException)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new Point(dval, dval);
                    }
                    else
                    {
                        return DependencyProperty.UnsetValue;
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
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
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
                    return DependencyProperty.UnsetValue;
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            if (!val.HasValue) return DependencyProperty.UnsetValue;

            return new Point(val.Value.X * dblParam, val.Value.Y * dblParam);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
                    PointConverter tc = new PointConverter();
#if NETCOREAPP
                    object? Maybeval = tc.ConvertFromString(null, culture, vs);
#else
                    object Maybeval = tc.ConvertFromString(null, culture, vs);
#endif
                    if (Maybeval is Point tt)
                    {
                        val = tt;
                    }
                    else { throw new NotSupportedException(); } // this can also be thrown by PointConverter if it can't do the conversion
                }
                catch (NotSupportedException)
                {
                    if (double.TryParse(vs, NumberStyles.Number | NumberStyles.AllowExponent, culture.NumberFormat, out double dval))
                    {
                        val = new Point(dval, dval);
                    }
                    else
                    {
                        return DependencyProperty.UnsetValue;
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
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
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
                    return DependencyProperty.UnsetValue;
                }
            }
            else if (parameter is IConvertible vp)
            {
                try
                {
                    dblParam = System.Convert.ToDouble(parameter, culture);
                }
                catch (FormatException)
                {
                    return DependencyProperty.UnsetValue;
                }
                catch (InvalidCastException)
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            if (!val.HasValue) return DependencyProperty.UnsetValue;

            return new Point(val.Value.X / dblParam, val.Value.Y / dblParam);
        }
    }
}
