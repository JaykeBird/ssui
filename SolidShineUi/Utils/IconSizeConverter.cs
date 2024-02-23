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
    /// Select an icon from a ICO file/resource, that matches a given width or height.
    /// If the exact size wanted isn't available, this will instead return the nearest available size that's smaller than the given width or height.
    /// </summary>
    public class IconSizeConverter : IValueConverter
    {
        /// <summary>
        /// Select an icon from a ICO file/resource, that matches a given width or height (provided via the <paramref name="parameter"/>).
        /// </summary>
        /// <param name="value">The URI or file path to the icon to use, or a <see cref="BitmapImage"/> or <see cref="BitmapFrame"/> taken from the icon file/resource to use</param>
        /// <param name="targetType">Not used. This function will always return a <see cref="BitmapFrame"/> (or <c>null</c> if the image cannot be found).</param>
        /// <param name="culture">Not used. This does not affect the conversion or output.</param>
        /// <param name="parameter">The target width or height to use, as an <see cref="int"/>. If this cannot be parsed or a parameter is not passed, then the smallest size is used</param>
        /// <remarks>
        /// ICO resources can contain multiple bitmap images, at various available sizes. WPF by default only selects a specific size, so this converter allows access to
        /// any available size in a standard ICO file/resource.
        /// The ICO resource should be provided as a file path, a URL, or a pack:// URI if stored as a resource within the application or a DLL.
        /// Resources that come from streams or from other sources/locations won't work, and <c>null</c> will be returned.
        /// If the file path or URI provided doesn't point to a valid ICO file, then <c>null</c> is returned.
        /// <para />
        /// This will attempt to look for the icon that is exactly the size requested in <paramref name="parameter"/>, or the next smallest size
        /// (so if <c>32</c> isn't found, it'll instead return a <c>24</c> size or <c>16</c> size icon instead, if those are present).
        /// If no size that is at or below the desired size is found (or the <paramref name="parameter"/> value isn't an integer), then the smallest size in the ICO resource will be returned.
        /// </remarks>
        /// <returns>A <see cref="BitmapFrame"/> of an icon, that matches the size (or nearest size) passed in. If the icon cannot be found, returns <c>null</c>.</returns>
#if NETCOREAPP
        public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri? uri = null;
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri uri = null;
#endif
            int size = 0;
            try
            {
                size = string.IsNullOrWhiteSpace(parameter?.ToString()) ? 0 : System.Convert.ToInt32(parameter);
            }
            catch (FormatException) { }
            catch (InvalidCastException) { }
            catch (OverflowException) { }

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
                    return DependencyProperty.UnsetValue;
                }
                else if (Uri.IsWellFormedUriString(suri, UriKind.Absolute))
                {
                    // not sure why this was returning the string, so now this is removed
                    // return suri;
                }
                else if (!suri.StartsWith("pack:"))
                {
                    suri = $"pack://application:,,,{suri}";
                }

                uri = new Uri(suri);
            }

            try
            {
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
            catch (System.IO.FileFormatException) { return DependencyProperty.UnsetValue; }
            catch (ArgumentNullException) { return DependencyProperty.UnsetValue; }
        }

        /// <summary>Not implemented, returns <see cref="DependencyProperty.UnsetValue"/>.</summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}