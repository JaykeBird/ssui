using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Load an icon from SolidShineUi's internal icon list.
    /// </summary>
    public static class IconLoader
    {
        /// <summary>
        /// Black monochrome icon
        /// </summary>
        public const string ICON_BLACK = "Black";
        /// <summary>
        /// Multi-colored icon
        /// </summary>
        public const string ICON_COLOR = "Color";
        /// <summary>
        /// White monochrome icon
        /// </summary>
        public const string ICON_WHITE = "White";

        /// <summary>
        /// Load an icon from SolidShineUi's internal icon list.
        /// </summary>
        /// <param name="iconName">The name of the icon.</param>
        /// <param name="colorVariation">The color variation to use: <c>"Color"</c>, <c>"Black"</c>, or <c>"White"</c>.</param>
        /// <returns></returns>
        public static BitmapImage LoadIcon(string iconName, string colorVariation = ICON_COLOR)
        {
            try
            {
                return new BitmapImage(new Uri("/SolidShineUi;component/Images/" + iconName + colorVariation + ".png", UriKind.Relative));
            }
            catch (IOException ex)
            {
                throw new ArgumentException("Cannot find an icon with this name.", nameof(iconName), ex);
            }
        }

        /// <summary>
        /// Load an icon from SolidShineUi's internal icon list.
        /// </summary>
        /// <param name="iconName">The name of the icon.</param>
        /// <param name="colorVariation">The icon color variation to use.</param>
        /// <returns></returns>
        public static BitmapImage LoadIcon(string iconName, IconVariation colorVariation = IconVariation.Color)
        {
            try
            {
                return new BitmapImage(new Uri("/SolidShineUi;component/Images/" + iconName + colorVariation + ".png", UriKind.Relative));
            }
            catch (IOException ex)
            {
                throw new ArgumentException("Cannot find an icon with this name.", nameof(iconName), ex);
            }
        }

        /// <summary>
        /// Load an icon from SolidShineUI's internal icon list, using the given ColorScheme to determine which icon variation to use.
        /// </summary>
        /// <param name="iconName">The name of the icon.</param>
        /// <param name="cs">The ColorScheme to use to determine which variation to use.</param>
        /// <returns></returns>
        public static BitmapSource LoadIcon(string iconName, ColorScheme cs)
        {
            return LoadIcon(iconName, SelectVariationFromColorScheme(cs));
        }

        /// <summary>
        /// Select the icon variation to use based upon an inputted color scheme.
        /// </summary>
        /// <param name="cs"></param>
        /// <returns>The recommended IconColor to use for this color scheme</returns>
        /// <remarks>While built for internal use, this could be helpful as a recommendation for which icon variation to use based upon a color scheme.</remarks>
        public static IconVariation SelectVariationFromColorScheme(ColorScheme cs)
        {
            if (cs.BackgroundColor == Colors.Black || cs.ForegroundColor == Colors.White)
            {
                return IconVariation.White;
            }
            else if (cs.BackgroundColor == Colors.White)
            {
                return IconVariation.Black;
            }
            else
            {
                return IconVariation.Color;
            }
        }

    }

    /// <summary>
    /// The different icon variations available for Solid Shine UI icons. Used internally with <see cref="MessageDialog"/> and <see cref="IconLoader"/>,
    /// but could also help as an indicator for which icon to use based upon various color schemes or other visual info.
    /// </summary>
    public enum IconVariation
    {
        /// <summary>
        /// Black monochrome icon
        /// </summary>
        Black,
        /// <summary>
        /// Multicolored icon
        /// </summary>
        Color,
        /// <summary>
        /// White monochrome icon
        /// </summary>
        White
    }
}