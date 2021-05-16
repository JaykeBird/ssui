using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi
{

    /// <summary>
    /// A class that creates various brushes conveniently.
    /// </summary>
    public static class BrushFactory
    {

        /// <summary>
        /// Create a brush based upon a single color.
        /// </summary>
        /// <param name="color">The color to use.</param>
        /// <returns>A SolidColorBrush that includes the color.</returns>
        public static SolidColorBrush Create(Color color)
        {
            return new SolidColorBrush(color);
        }

        /// <summary>
        /// Create a brush based upon a single color.
        /// </summary>
        /// <param name="hex">The hex value of the color to use.</param>
        /// <returns>A SolidColorBrush that includes the color.</returns>
        public static SolidColorBrush Create(string hex)
        {
            return new SolidColorBrush(ColorsHelper.CreateFromHex(hex));
        }

        /// <summary>
        /// Create a brush based upon two colors, made into a linear gradient.
        /// </summary>
        /// <param name="col1">The first color.</param>
        /// <param name="col2">The second color.</param>
        /// <param name="angle">The angle of the gradient to be made.</param>
        /// <returns>A LinearGradientBrush that includes the two colors.</returns>
        public static LinearGradientBrush Create(Color col1, Color col2, double angle)
        {
            return new LinearGradientBrush(col1, col2, angle);
        }

        /// <summary>
        /// Create a brush based upon an image.
        /// </summary>
        /// <param name="image">The image to use.</param>
        /// <returns>An ImageBrush containing this image.</returns>
        public static ImageBrush CreateFromImage(ImageSource image)
        {
            return new ImageBrush(image);
        }

        /// <summary>
        /// Create a brush based upon an image.
        /// </summary>
        /// <param name="location">The location, such as a web address or file location, of the image to use.</param>
        /// <returns>An ImageBrush containing this image.</returns>
        public static ImageBrush CreateFromImage(Uri location)
        {
            return new ImageBrush(new BitmapImage(location));
        }
    }

    ///// <summary>
    ///// Contains some <code>SolidColorBrush</code> fields for usage in High Contrast mode.
    ///// </summary>
    //public static class HighContrastBrushes
    //{
    //    public static SolidColorBrush LightBlue { get; } = ColorsHelper.HighContrastLightBlue.ToBrush();
    //    public static SolidColorBrush LightPurple { get; } = ColorsHelper.HighContrastLightPurple.ToBrush();
    //    public static SolidColorBrush Purple { get; } = ColorsHelper.HighContrastPurple.ToBrush();
    //    public static SolidColorBrush Green { get; } = ColorsHelper.HighContrastGreen.ToBrush();
    //    public static SolidColorBrush LightGreen { get; } = ColorsHelper.HighContrastLightGreen.ToBrush();
    //    public static SolidColorBrush Blue { get; } = ColorsHelper.HighContrastBlue.ToBrush();
    //    public static SolidColorBrush Gray { get; } = ColorsHelper.HighContrastGray.ToBrush();
    //    public static SolidColorBrush Red { get; } = ColorsHelper.HighContrastRed.ToBrush();
    //}
}
