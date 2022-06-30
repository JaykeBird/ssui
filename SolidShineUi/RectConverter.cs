using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SolidShineUi
{
    /// <summary>
    /// A MultiValueConverter that takes 4 values and creates a Rect. This class can be used to get around the fact that a Rect cannot be used for XAML binding.
    /// </summary>
    public class RectConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Convert 4 doubles to a Rect structure.
        /// </summary>
        /// <param name="values">The four doubles to use to measure the Rect's X, Y, Width, and Height values respectively.</param>
        /// <param name="targetType">Not used, always returns a <see cref="Rect"/></param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Rect((double)values[0], (double)values[1], (double)values[2], (double)values[3]);
        }

        /// <summary>
        /// Convert a Rect into an array of doubles representing its X, Y, Width, and Height values respectively.
        /// </summary>
        /// <exception cref="InvalidCastException">Thrown if the value passed in is not a Rect structure.</exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Rect r)
            {
                //return new double[4] { r.X, r.Y, r.Width, r.Height };
                return new object[4] { r.X, r.Y, r.Width, r.Height };
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        #endregion

        /// <summary>
        /// A double defined as zero. Can be used if there is a value in your Rect that you do not want to set (via {x:Static} in XAML).
        /// </summary>
        public static double ZERO = 0.0d;
    }
}
