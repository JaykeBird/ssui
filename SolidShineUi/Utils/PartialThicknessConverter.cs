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
    public class PartialThicknessConverter : IValueConverter
    {

        /// <summary>
        /// Convert a double into a string, with rounding possible by setting the <paramref name="parameter"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Thickness"/> object to read from</param>
        /// <param name="targetType">Not used, returned type will always be a <see cref="Thickness"/></param>
        /// <param name="parameter">The corners to apply to the result; one or more of <c>TL,TR,BL,BR</c></param>
        /// <param name="culture">Not used</param>
        /// <returns>A <see cref="Thickness"/> that only has a portion of its values set </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                Thickness th = new Thickness(d);
                if (parameter is string s)
                {
                    Thickness th2 = new Thickness(0.0);

                    if (s.Contains("T"))
                    {
                        th2.Top = th.Top;
                    }
                    if (s.Contains("L"))
                    {
                        th2.Left = th.Left;
                    }
                    if (s.Contains("R"))
                    {
                        th2.Right = th.Right;
                    }
                    if (s.Contains("B"))
                    {
                        th2.Bottom = th.Bottom;
                    }

                    return th2;
                }
                else if (parameter is byte b)
                {
                    Thickness th2 = new Thickness(0.0);

                    if (b % 2 == 0)
                    {
                        th2.Top = th.Top;
                    }
                    if (b % 3 == 0)
                    {
                        th2.Left = th.Left;
                    }
                    if (b % 5 == 0)
                    {
                        th2.Right = th.Right;
                    }
                    if (b % 7 == 0)
                    {
                        th2.Bottom = th.Bottom;
                    }

                    return th2;
                }
                else
                {
                    return th;
                }
            }
            else if (value is Thickness th)
            {
                if (parameter is string s)
                {
                    Thickness th2 = new Thickness(0.0);

                    if (s.Contains("T"))
                    {
                        th2.Top = th.Top;
                    }
                    if (s.Contains("L"))
                    {
                        th2.Left = th.Left;
                    }
                    if (s.Contains("R"))
                    {
                        th2.Right = th.Right;
                    }
                    if (s.Contains("B"))
                    {
                        th2.Bottom = th.Bottom;
                    }

                    return th2;
                }
                else if (parameter is byte b)
                {
                    Thickness th2 = new Thickness(0.0);

                    if (b % 2 == 0)
                    {
                        th2.Top = th.Top;
                    }
                    if (b % 3 == 0)
                    {
                        th2.Left = th.Left;
                    }
                    if (b % 5 == 0)
                    {
                        th2.Right = th.Right;
                    }
                    if (b % 7 == 0)
                    {
                        th2.Bottom = th.Bottom;
                    }

                    return th2;
                }
                else
                {
                    return th;
                }
            }
            else { return new Thickness(); }
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
