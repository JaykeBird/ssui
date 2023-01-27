using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Media;

namespace SolidShineUi.Utils
{
    // from https://stackoverflow.com/a/54738646/2987285

    /// <summary>
    /// Forces the selection of a given size from the ICO file/resource. 
    /// If the exact size does not exists, selects the closest smaller if possible otherwise closest higher resolution.
    /// If no parameter is given, the smallest frame available will be selected
    /// </summary>
    public class IconSizeConverter : IValueConverter
    {
        /// <inheritdoc/>
#if NETCOREAPP
        public virtual object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri? uri = null;
#else
        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri uri = null;
#endif

            int size = string.IsNullOrWhiteSpace(parameter?.ToString()) ? 0 : System.Convert.ToInt32(parameter);

            if (value is BitmapFrame bfd)
            {
                // a future efficiency thing would be to reuse this existing decoder, rather than creating our own later
                uri = new Uri(bfd.Decoder.ToString());
            }
            else if (value is BitmapImage bmp)
            {
                if (!string.IsNullOrWhiteSpace(bmp.UriSource.ToString()))
                {
                    uri = bmp.UriSource;
                }
            }
            else
            {
                string suri = (value?.ToString() ?? "").Trim();

                if (string.IsNullOrWhiteSpace(suri))
                {
                    return null;
                }
                else if (Uri.IsWellFormedUriString(suri, UriKind.Absolute))
                {
                    return suri;
                }
                else if (!suri.StartsWith("pack:"))
                {
                    suri = $"pack://application:,,,{suri}";
                }

                uri = new Uri(suri);
            }

            var decoder = BitmapDecoder.Create(uri, BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);

            //Debug.WriteLine("DECODER FRAMES: " + decoder.Frames);

#if NETCOREAPP
            BitmapFrame? result = decoder.Frames.Where(f => f.Width <= size).OrderByDescending(f => f.Width).FirstOrDefault()
                ?? decoder.Frames.OrderBy(f => f.Width).FirstOrDefault();
#else
            BitmapFrame result = decoder.Frames.Where(f => f.Width <= size).OrderByDescending(f => f.Width).FirstOrDefault()
                ?? decoder.Frames.OrderBy(f => f.Width).FirstOrDefault();
#endif

            //Debug.WriteLine("SELECTED FRAME: " + result?.Width ?? "(null)");

            return result;
        }

        /// <summary>Not implemented, throws a <see cref="NotImplementedException"/>.</summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
