using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace SolidShineUi.Converters
{
    /// <summary>
    /// Converts a <see cref="FontWeight"/> OpenType weight value into a human-friendly English string (based upon the names in <see cref="FontWeights"/>).
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    [ValueConversion(typeof(FontWeight), typeof(string))]
    public class FontWeightStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert a <see cref="FontWeight"/> or its <see cref="int"/> OpenType weight value to a human-friendly string, if there is a string for that particular value.
        /// </summary>
        /// <remarks>
        /// Valid values that don't have a name will return <c>"(no name)"</c>.
        /// Values that aren't a <see cref="FontWeight"/> or <see cref="int"/> will return <see cref="DependencyProperty.UnsetValue"/>.
        /// </remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
                if (FontWeightToString(fw.ToOpenTypeWeight(), out string ss))
                {
                    return ss;
                }
                else
                {
                    return "(no name)";
                }
            }
            //else return "(not an int)";
            else return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Convert a <see cref="string"/> back into a <see cref="FontWeight"/> OpenType weight value, if the string is a name of a FontWeight in <see cref="FontWeights"/>.
        /// </summary>
        /// <remarks>
        /// This returns an <see cref="int"/> representing the OpenType weight value of the particular FontWeight, not the actual FontWeight itself.
        /// Strings that don't correspond to a value in <see cref="FontWeights"/> will return <see cref="DependencyProperty.UnsetValue"/>.
        /// </remarks>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                FontWeight fw = FontWeights.Bold;
                if (FontWeightStringToKnownWeight(s, culture, ref fw))
                {
                    return fw.ToOpenTypeWeight();
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            else return DependencyProperty.UnsetValue;
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

        static bool FontWeightStringToKnownWeight(string s, IFormatProvider provider, ref FontWeight fontWeight)
        {
            switch (s.Length)
            {
                case 4:
                    if (s.Equals("Bold", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.Bold;
                        return true;
                    }

                    if (s.Equals("Thin", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.Thin;
                        return true;
                    }

                    break;
                case 5:
                    if (s.Equals("Black", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.Black;
                        return true;
                    }

                    if (s.Equals("Light", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.Light;
                        return true;
                    }

                    if (s.Equals("Heavy", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.Heavy;
                        return true;
                    }

                    break;
                case 6:
                    if (s.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.Normal;
                        return true;
                    }

                    if (s.Equals("Medium", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.Medium;
                        return true;
                    }

                    break;
                case 7:
                    if (s.Equals("Regular", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.Regular;
                        return true;
                    }

                    break;
                case 8:
                    if (s.Equals("SemiBold", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.SemiBold;
                        return true;
                    }

                    if (s.Equals("DemiBold", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.DemiBold;
                        return true;
                    }

                    break;
                case 9:
                    if (s.Equals("ExtraBold", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.ExtraBold;
                        return true;
                    }

                    if (s.Equals("UltraBold", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.UltraBold;
                        return true;
                    }

                    break;
                case 10:
                    if (s.Equals("ExtraLight", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.ExtraLight;
                        return true;
                    }

                    if (s.Equals("UltraLight", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.UltraLight;
                        return true;
                    }

                    if (s.Equals("ExtraBlack", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.ExtraBlack;
                        return true;
                    }

                    if (s.Equals("UltraBlack", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = FontWeights.UltraBlack;
                        return true;
                    }

                    break;
            }

            if (int.TryParse(s, NumberStyles.Integer, provider, out int weightValue))
            {
                fontWeight = FontWeight.FromOpenTypeWeight(weightValue);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Converts an integer to a <see cref="FontWeight"/>.
    /// </summary>
    [ValueConversion(typeof(int), typeof(FontWeight))]
    public class FontWeightValueConverter : IValueConverter
    {
        /// <summary>
        /// Convert an integer into a <see cref="FontWeight"/>, based upon the value of <see cref="FontWeight.FromOpenTypeWeight(int)"/>.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                return FontWeight.FromOpenTypeWeight(i);
            }
            else return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Convert a <see cref="FontWeight"/> to its OpenType weight value, based upon the value of <see cref="FontWeight.ToOpenTypeWeight"/>.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontWeight fw)
            {
                return fw.ToOpenTypeWeight();
            }
            else return DependencyProperty.UnsetValue;
        }
    }
}
