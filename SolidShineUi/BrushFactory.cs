using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if AVALONIA
using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Imaging;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

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

        #region Create Linear Gradient

        /// <summary>
        /// Create a brush based upon two colors, made into a linear gradient.
        /// </summary>
        /// <param name="col1">The first color.</param>
        /// <param name="col2">The second color.</param>
        /// <param name="angle">The angle of the gradient to be made.</param>
        /// <returns>A LinearGradientBrush that includes the two colors.</returns>
        public static LinearGradientBrush Create(Color col1, Color col2, double angle)
        {
#if AVALONIA
            return new LinearGradientBrush
            {
                GradientStops = new GradientStops { new GradientStop(col1, 0.0), new GradientStop(col2, 1.0) },
                StartPoint = new RelativePoint(0d, 0d, RelativeUnit.Relative),
                EndPoint = new RelativePoint(EndPointFromAngle(angle), RelativeUnit.Relative)
            };
#else
            return new LinearGradientBrush(col1, col2, angle);
#endif
        }

        /// <summary>
        /// Create a linear gradient brush from a collection of gradient stops.
        /// </summary>
        /// <param name="stops">The collection of gradient stops to use for creating the brush</param>
        public static LinearGradientBrush Create(IEnumerable<GradientStop> stops)
        {
#if AVALONIA
            return new LinearGradientBrush { GradientStops = [.. stops] };
#else
            return new LinearGradientBrush(new GradientStopCollection(stops));
#endif
        }

        /// <summary>
        /// Create a linear gradient brush from a collection of gradient stops.
        /// </summary>
        /// <param name="stops">The collection of gradient stops to use for creating the brush</param>
        /// <param name="angle">the angle of the gradient</param>
        public static LinearGradientBrush Create(IEnumerable<GradientStop> stops, double angle)
        {
#if AVALONIA
            return new LinearGradientBrush 
            {
                GradientStops = [.. stops],
                StartPoint = new RelativePoint(0d, 0d, RelativeUnit.Relative),
                EndPoint = new RelativePoint(EndPointFromAngle(angle), RelativeUnit.Relative)
            };
#else
            return new LinearGradientBrush(new GradientStopCollection(stops), angle);
#endif
        }

        /// <summary>
        /// Create a linear gradient brush from a collection of gradient stops.
        /// </summary>
        /// <param name="stops">The collection of gradient stops to use for creating the brush</param>
        /// <param name="startPoint">the relative start point of the gradient</param>
        /// <param name="endPoint">the relative end point of the gradient</param>
        public static LinearGradientBrush Create(IEnumerable<GradientStop> stops, Point startPoint, Point endPoint)
        {
#if AVALONIA
            return new LinearGradientBrush
            {
                GradientStops = [.. stops],
                StartPoint = new RelativePoint(startPoint, RelativeUnit.Relative),
                EndPoint = new RelativePoint(endPoint, RelativeUnit.Relative)
            };
#else
            return new LinearGradientBrush(new GradientStopCollection(stops), startPoint, endPoint);
#endif
        }

        private static Point EndPointFromAngle(double angle)
        {
            angle = angle * (1.0 / 180.0) * Math.PI;
            return new Point(Math.Cos(angle), Math.Sin(angle));
        }

        #endregion

        #region CreateFromImage

        /// <summary>
        /// Create a brush based upon an image.
        /// </summary>
        /// <param name="image">The image to use.</param>
        /// <returns>An ImageBrush containing this image.</returns>
#if AVALONIA
        public static ImageBrush CreateFromImage(IImageBrushSource image)
#else
        public static ImageBrush CreateFromImage(ImageSource image)
#endif
        {
            return new ImageBrush(image);
        }

#if AVALONIA
        /// <summary>
        /// Create a brush based upon an image.
        /// </summary>
        /// <param name="fileName">The file location of the image to use.</param>
        /// <returns>An ImageBrush containing this image.</returns>
        public static ImageBrush CreateFromImage(string fileName)
        {
            return new ImageBrush(new Bitmap(fileName));
        }
#else
        /// <summary>
        /// Create a brush based upon an image.
        /// </summary>
        /// <param name="location">The location, such as a web address or file location, of the image to use.</param>
        /// <returns>An ImageBrush containing this image.</returns>
        public static ImageBrush CreateFromImage(Uri location)
        {
            return new ImageBrush(new BitmapImage(location));
        }
#endif

        /// <summary>
        /// Create a brush based upon an image, with tiling.
        /// </summary>
        /// <param name="image">The image to use.</param>
        /// <param name="tile">The tiling mode to use for tiling this image.</param>
        /// <returns>An ImageBrush containing this image.</returns>
#if AVALONIA
        public static ImageBrush CreateFromImage(Bitmap image, TileMode tile)
        {
            ImageBrush br = new ImageBrush(image);
            br.DestinationRect = new RelativeRect(new Point(0, 0), new Size(image.Size.Width, image.Size.Height), RelativeUnit.Absolute);
            br.TileMode = tile;
            return br;
        }
#else
        public static ImageBrush CreateFromImage(ImageSource image, TileMode tile)
        {
            ImageBrush br = new ImageBrush(image);
            br.ViewportUnits = BrushMappingMode.Absolute;
            br.Viewport = new System.Windows.Rect(new System.Windows.Point(0, 0), new System.Windows.Size(image.Width, image.Height));
            br.TileMode = tile;
            return br;
        }
#endif

        /// <summary>
        /// Create a brush based upon an image, with tiling.
        /// </summary>
        /// <param name="location">The location, such as a web address or file location, of the image to use.</param>
        /// <param name="tile">The tiling mode to use for tiling this image.</param>
        /// <returns>An ImageBrush containing this image.</returns>
#if AVALONIA
        public static ImageBrush CreateFromImage(string location, TileMode tile)
        {
            Bitmap image = new Bitmap(location);
            ImageBrush br = new ImageBrush(image);
            br.DestinationRect = new RelativeRect(new Point(0, 0), new Size(image.Size.Width, image.Size.Height), RelativeUnit.Absolute);
            br.TileMode = tile;
            return br;
        }
#else
        public static ImageBrush CreateFromImage(Uri location, TileMode tile)
        {
            BitmapImage image = new BitmapImage(location);
            return CreateFromImage(image, tile);
        }
#endif

        #endregion

        #region Checkboard Brush

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
#if AVALONIA
            cbb.DestinationRect = new RelativeRect(0, 0, fullsize, fullsize, RelativeUnit.Absolute);
#else
            cbb.Viewport = new System.Windows.Rect(0, 0, fullsize, fullsize);
            cbb.ViewportUnits = BrushMappingMode.Absolute;
#endif

            DrawingGroup cbd = new DrawingGroup();
            cbd.Children.Add(new GeometryDrawing() { Brush = brush1, Geometry = Geometry.Parse($"M0,0 L{fullsize},0 {fullsize},{fullsize} 0,{fullsize}Z") });
            cbd.Children.Add(new GeometryDrawing() { Brush = brush2, Geometry = Geometry.Parse($"M0,{squareSize} L{fullsize},{squareSize} {fullsize},{fullsize} {squareSize},{fullsize} {squareSize},0 0,0Z") });

            cbb.Drawing = cbd;

            return cbb;
        }

        #endregion

        #region Avalonia Clone

#if AVALONIA

        /// <summary>
        /// Create a deep clone of a SolidColorBrush.
        /// </summary>
        /// <param name="brush">the brush to clone</param>
        /// <returns>a new SolidColorBrush, with the same properties</returns>
        public static SolidColorBrush Clone(this ISolidColorBrush brush)
        {
            var newBrush = new SolidColorBrush(Color.FromUInt32(brush.Color.ToUInt32()))
            {
                Opacity = brush.Opacity,
                Transform = brush.Transform,
                TransformOrigin = brush.TransformOrigin,
            };
            if (brush is SolidColorBrush scb)
            {
                newBrush.Transitions = TransitionsHelper.CloneTransitions(scb);
            }
            return newBrush;
        }

        /// <summary>
        /// Create a deep clone of a LinearGradientBrush.
        /// </summary>
        /// <param name="brush">the brush to clone</param>
        /// <returns>a new LinearGradientBrush, with the same properties</returns>
        public static LinearGradientBrush Clone(this ILinearGradientBrush brush)
        {
            var newBrush = new LinearGradientBrush
            {
                GradientStops = CloneStops(brush.GradientStops),
                StartPoint = brush.StartPoint,
                EndPoint = brush.EndPoint,
                SpreadMethod = brush.SpreadMethod,
                Opacity = brush.Opacity,
                Transform = brush.Transform,
                TransformOrigin = brush.TransformOrigin,
            };
            if (brush is LinearGradientBrush scb)
            {
                newBrush.Transitions = TransitionsHelper.CloneTransitions(scb);
            }
            return newBrush;
        }

        /// <summary>
        /// Create a deep clone of a RadialGradientBrush.
        /// </summary>
        /// <param name="brush">the brush to clone</param>
        /// <returns>a new RadialGradientBrush, with the same properties</returns>
        public static RadialGradientBrush Clone(this IRadialGradientBrush brush)
        {
            var newBrush = new RadialGradientBrush
            {
                GradientStops = CloneStops(brush.GradientStops),
                Center = brush.Center,
                GradientOrigin = brush.GradientOrigin,
                RadiusX = brush.RadiusX,
                RadiusY = brush.RadiusY,
                SpreadMethod = brush.SpreadMethod,
                Opacity = brush.Opacity,
                Transform = brush.Transform,
                TransformOrigin = brush.TransformOrigin,
            };
            if (brush is RadialGradientBrush scb)
            {
                newBrush.Transitions = TransitionsHelper.CloneTransitions(scb);
            }
            return newBrush;
        }

        /// <summary>
        /// Create a deep clone of a ConicGradientBrush.
        /// </summary>
        /// <param name="brush">the brush to clone</param>
        /// <returns>a new ConicGradientBrush, with the same properties</returns>
        public static ConicGradientBrush Clone(this IConicGradientBrush brush)
        {
            var newBrush = new ConicGradientBrush
            {
                GradientStops = CloneStops(brush.GradientStops),
                Center = brush.Center,
                Angle = brush.Angle,
                SpreadMethod = brush.SpreadMethod,
                Opacity = brush.Opacity,
                Transform = brush.Transform,
                TransformOrigin = brush.TransformOrigin,
            };
            if (brush is ConicGradientBrush scb)
            {
                newBrush.Transitions = TransitionsHelper.CloneTransitions(scb);
            }
            return newBrush;
        }

        /// <summary>
        /// Create a deep clone of an ImageBrush,
        /// except the <c>Source</c> property, which is only a shallow copy.
        /// </summary>
        /// <param name="brush">the brush to clone</param>
        /// <returns>a new ImageBrush, with the same properties</returns>
        public static ImageBrush Clone(this IImageBrush brush)
        {
            var newBrush = new ImageBrush(brush.Source)
            {
                DestinationRect = brush.DestinationRect,
                TileMode = brush.TileMode,
                Opacity = brush.Opacity,
                Transform = brush.Transform,
                TransformOrigin = brush.TransformOrigin,
                AlignmentX = brush.AlignmentX,
                AlignmentY = brush.AlignmentY,
                SourceRect = brush.SourceRect,
                Stretch = brush.Stretch,
            };
            if (brush is ImageBrush scb)
            {
                newBrush.Transitions = TransitionsHelper.CloneTransitions(scb);
            }
            return newBrush;
        }

        /// <summary>
        /// Create a deep clone of a VisualBrush.
        /// except the <c>Visual</c> property, which is only a shallow copy.
        /// </summary>
        /// <param name="brush">the brush to clone</param>
        /// <returns>a new VisualBrush, with the same properties</returns>
        public static VisualBrush Clone(this VisualBrush brush)
        {
            var newBrush = new VisualBrush()
            {
                Visual = brush.Visual,
                DestinationRect = brush.DestinationRect,
                TileMode = brush.TileMode,
                Opacity = brush.Opacity,
                Transform = brush.Transform,
                TransformOrigin = brush.TransformOrigin,
                AlignmentX = brush.AlignmentX,
                AlignmentY = brush.AlignmentY,
                SourceRect = brush.SourceRect,
                Stretch = brush.Stretch,
            };
            if (brush is VisualBrush scb)
            {
                newBrush.Transitions = TransitionsHelper.CloneTransitions(scb);
            }
            return newBrush;
        }

        /// <summary>
        /// Create a deep clone of a brush.
        /// </summary>
        /// <param name="brush">the brush to clone</param>
        /// <returns>a new brush, with the same properties</returns>
        /// <remarks>
        /// For <see cref="IImageBrush"/> and <see cref="VisualBrush"/>, the Source or Visual property
        /// will only be shallow copied, rather than a deep copy.
        /// </remarks>
        /// <exception cref="NotSupportedException">thrown when the brush type is not supported for cloning</exception>
        public static IBrush Clone(this IBrush brush)
        {
            if (brush is ISolidColorBrush scb)
            {
                return scb.Clone();
            }
            else if (brush is ILinearGradientBrush lgb)
            {
                return lgb.Clone();
            }
            else if (brush is IRadialGradientBrush rgb)
            {
                return rgb.Clone();
            }
            else if (brush is IConicGradientBrush cgb)
            {
                return cgb.Clone();
            }
            else if (brush is IImageBrush ib)
            {
                return ib.Clone();
            }
            else if (brush is VisualBrush vb)
            {
                return vb.Clone();
            }
            else
            {
                throw new NotSupportedException("The brush type " + brush.GetType().FullName + " is not supported for cloning.");
            }
        }

        /// <summary>
        /// Create a deep clone of a list of gradient stops.
        /// </summary>
        /// <param name="stops">the list of stops to clone</param>
        /// <returns>a new list of gradient stops, with the same properties</returns>
        static GradientStops CloneStops(IReadOnlyList<IGradientStop> stops)
        {
            var newStops = new GradientStops();
            foreach (var stop in stops)
            {
                newStops.Add(new GradientStop(stop.Color, stop.Offset));
            }
            return newStops;
        }

#endif
        #endregion

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
