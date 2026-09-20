using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// Contains helper classes to interact with <see cref="Rect"/> objects.
    /// </summary>
    public static class RectHelper
    {

        /// <summary>
        /// Get a point that is at the center of a Rect.
        /// </summary>
        /// <param name="rect">the Rect to get the center of</param>
        public static Point GetCenter(this Rect rect)
        {
            return new Point(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
        }

        /// <summary>
        /// Subtracts (cuts out) a list of rectangles from a rectangle and returns a list of rectangles representing the remaining area.
        /// </summary>
        public static List<Rect> Subtract(this Rect source, List<Rect> cutouts)
        {
            // Start with the source geometry
            Geometry resultGeometry = new RectangleGeometry(source);

            // Subtract each cutout
            foreach (var cut in cutouts)
            {
                var cutGeo = new RectangleGeometry(cut);
                resultGeometry = Geometry.Combine(resultGeometry, cutGeo, GeometryCombineMode.Exclude, null);
            }

            // Extract resulting rectangles
            return ExtractRectsFromGeometry(resultGeometry);
        }

        /// <summary>
        /// Converts a Geometry into a list of Rects (handles GeometryGroup and RectangleGeometry).
        /// In other words, this flattens the complex Geometry object into a flat one-dimensional list.
        /// </summary>
        static List<Rect> ExtractRectsFromGeometry(Geometry geometry)
        {
            var rects = new List<Rect>();

            if (geometry is RectangleGeometry rg)
            {
                rects.Add(rg.Rect);
            }
            else if (geometry is GeometryGroup group)
            {
                foreach (var child in group.Children)
                {
                    rects.AddRange(ExtractRectsFromGeometry(child));
                }
            }
            else if (geometry is PathGeometry pg)
            {
                rects.Add(pg.Bounds);
            }

            return rects;
        }

    }
}
