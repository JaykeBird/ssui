using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using Avalonia.Data;
using Avalonia.Media;

namespace SolidShineUi.Converters
{
    /// <summary>
    /// Converts a <see cref="FontWeight"/> OpenType weight value into a human-friendly English string (based upon the names in <see cref="FontWeight"/>).
    /// </summary>
    public class FontWeightStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert a <see cref="FontWeight"/> or its <see cref="int"/> OpenType weight value to a human-friendly string, if there is a string for that particular value.
        /// </summary>
        /// <remarks>
        /// Valid values that don't have a name will return <c>"(no name)"</c>.
        /// Values that aren't a <see cref="FontWeight"/> or <see cref="int"/> will return a binding error.
        /// </remarks>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int weight)
            {
                if (FontWeightToString(weight, out string ss))
                {
                    return ss;
                }
                else
                {
                    return "(no name)";
                }
            }
            else if (value is FontWeight fw)
            {
                if (FontWeightToString((int)fw, out string ss))
                {
                    return ss;
                }
                else
                {
                    return "(no name)";
                }
            }
            //else return "(not an int)";
            else return new BindingNotification(new FormatException("value cannot be parsed as a FontWeight"), BindingErrorType.Error);
        }

        /// <summary>
        /// Convert a <see cref="string"/> back into a <see cref="FontWeight"/> OpenType weight value, if the string is a name of a value in <see cref="FontWeight"/>.
        /// </summary>
        /// <remarks>
        /// This returns an <see cref="int"/> representing the OpenType weight value of the particular FontWeight, not the actual FontWeight itself.
        /// Strings that don't correspond to a value in <see cref="FontWeight"/> will return a binding error.
        /// </remarks>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                if (FontWeightStringToKnownWeight(s, culture, out FontWeight fw))
                {
                    return (int)fw;
                }
                else
                {
                    return new BindingNotification(new FormatException("value is not a string"), BindingErrorType.Error);
                }
            }
            else return new BindingNotification(new FormatException("value is not a string"), BindingErrorType.Error);
        }

        // code from https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/FontWeights.cs (with minor changes)
        // why do you mark so much helpful stuff as internal, Microsoft WPF team???

        static bool FontWeightToString(int weight, out string convertedValue)
        {
            switch (weight)
            {
                case 100:
                    convertedValue = "Thin";
                    return true;
                case 200:
                    convertedValue = "ExtraLight";
                    return true;
                case 300:
                    convertedValue = "Light";
                    return true;
                case 400:
                    convertedValue = "Normal";
                    return true;
                case 500:
                    convertedValue = "Medium";
                    return true;
                case 600:
                    convertedValue = "SemiBold";
                    return true;
                case 700:
                    convertedValue = "Bold";
                    return true;
                case 800:
                    convertedValue = "ExtraBold";
                    return true;
                case 900:
                    convertedValue = "Black";
                    return true;
                case 950:
                    convertedValue = "ExtraBlack";
                    return true;
            }
            convertedValue = "(no name)";
            return false;
        }

        static bool FontWeightStringToKnownWeight(string s, IFormatProvider provider, out FontWeight fontWeight)
        {
            if (Enum.TryParse(s, out FontWeight fw))
            {
                fontWeight = fw;
                return true;
            }
            else if (int.TryParse(s, NumberStyles.Integer, provider, out int weightValue))
            {
                fontWeight = (FontWeight)weightValue;
                return true;
            }
            else
            {
                fontWeight = 0;
                return false;
            }
        }
    }

    /// <summary>
    /// Converts an integer to a <see cref="FontWeight"/>.
    /// </summary>
    public class FontWeightValueConverter : IValueConverter
    {
        /// <summary>
        /// Convert an integer into a <see cref="FontWeight"/>.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                return (FontWeight)i;
            }
            else if (value is IConvertible ic)
            {
                try
                {
                    return (FontWeight)System.Convert.ToInt32(ic);
                }
                catch (FormatException ex) { return new BindingNotification(new FormatException("value is not an int", ex), BindingErrorType.Error); }
                catch (InvalidCastException ex) { return new BindingNotification(new FormatException("value is not an int", ex), BindingErrorType.Error); }
                catch (OverflowException ex) { return new BindingNotification(new FormatException("value is not an int", ex), BindingErrorType.Error); }
            }
            else return new BindingNotification(new FormatException("value is not an int"), BindingErrorType.Error);
        }

        /// <summary>
        /// Convert a <see cref="FontWeight"/> to its OpenType weight value (as in <c>int</c>).
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontWeight fw)
            {
                return (int)fw;
            }
            else return new BindingNotification(new FormatException("value is not a FontWeight"), BindingErrorType.Error);
        }
    }
}
