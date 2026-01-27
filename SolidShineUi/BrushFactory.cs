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

        /// <summary>
        /// Create a brush based upon an image, with tiling.
        /// </summary>
        /// <param name="image">The image to use.</param>
        /// <param name="tile">The tiling mode to use for tiling this image.</param>
        /// <returns>An ImageBrush containing this image.</returns>
        public static ImageBrush CreateFromImage(ImageSource image, TileMode tile)
        {
            ImageBrush br = new ImageBrush(image);
            br.ViewportUnits = BrushMappingMode.Absolute;
            br.Viewport = new System.Windows.Rect(new System.Windows.Point(0, 0), new System.Windows.Size(image.Width, image.Height));
            br.TileMode = tile;
            return br;
        }

        /// <summary>
        /// Create a brush based upon an image, with tiling.
        /// </summary>
        /// <param name="location">The location, such as a web address or file location, of the image to use.</param>
        /// <param name="tile">The tiling mode to use for tiling this image.</param>
        /// <returns>An ImageBrush containing this image.</returns>
        public static ImageBrush CreateFromImage(Uri location, TileMode tile)
        {
            BitmapImage image = new BitmapImage(location);
            return CreateFromImage(image, tile);
        }

        /// <summary>
        /// Create a brush with a checkerboard pattern, where the size and colors of the squares are customizable.
        /// </summary>
        /// <param name="squareSize">The size of each square in the pattern. A size of 4 will create a pattern of 4x4 squares, each of alternating colors.</param>
        /// <param name="color1">The color to use for the first set of squares.</param>
        /// <param name="color2">The color to use for the second, alternating set of squares.</param>
        public static DrawingBrush CreateCheckerboardBrush(double squareSize, Color color1, Color color2)
        {
            return CreateCheckerboardBrush(squareSize, Create(color1), Create(color2));
        }

        /// <summary>
        /// Create a brush with a checkerboard pattern, where the size and colors of the squares are customizable.
        /// </summary>
        /// <param name="squareSize">The size of each square in the pattern. A size of 4 will create a pattern of 4x4 squares, each of alternating colors.</param>
        /// <param name="brush1">The brush to use for the first set of squares.</param>
        /// <param name="brush2">The brush to use for the second, alternating set of squares.</param>
        public static DrawingBrush CreateCheckerboardBrush(double squareSize, Brush brush1, Brush brush2)
        {
            double fullsize = squareSize * 2;
            //<DrawingBrush Stretch = "None" TileMode = "Tile" Viewport = "0,0,8,8" ViewportUnits = "Absolute">
            //    <!-- https://stackoverflow.com/questions/3827561/how-to-create-checker-board-pattern -->
            //    <DrawingBrush.Drawing>
            //        <DrawingGroup>
            //            <GeometryDrawing Geometry = "M0,0 L8,0 8,8 0,8Z" Brush = "White" />
            //            <GeometryDrawing Geometry = "M0,4 L8,4 8,8 4,8 4,0 0,0Z" Brush = "LightGray" />
            //        </DrawingGroup>
            //    </DrawingBrush.Drawing>
            //</DrawingBrush>

            DrawingBrush cbb = new DrawingBrush();
            cbb.Stretch = Stretch.None;
            cbb.TileMode = TileMode.Tile;
            cbb.Viewport = new System.Windows.Rect(0, 0, fullsize, fullsize);
            cbb.ViewportUnits = BrushMappingMode.Absolute;

            DrawingGroup cbd = new DrawingGroup();
            cbd.Children.Add(new GeometryDrawing() { Brush = brush1, Geometry = Geometry.Parse($"M0,0 L{fullsize},0 {fullsize},{fullsize} 0,{fullsize}Z") });
            cbd.Children.Add(new GeometryDrawing() { Brush = brush2, Geometry = Geometry.Parse($"M0,{squareSize} L{fullsize},{squareSize} {fullsize},{fullsize} {squareSize},{fullsize} {squareSize},0 0,0Z") });

            cbb.Drawing = cbd;

            return cbb;
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
