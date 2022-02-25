using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace SolidShineUi.Utils
{
    public static class IconLoader
    {
        public const string ICON_BLACK = "Black";
        public const string ICON_COLOR = "Color";
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

    }
}