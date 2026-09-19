using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Windows.Graphics;

namespace SsuiWindows11Interop
{

    /// <summary>
    /// A helper class for interacting with a <see cref="Rect"/> and subtracting areas from it.
    /// </summary>
    public static class RectHelper
    {

        /// <summary>
        /// Subtracts a list of rectangles from a rectangle and returns the remaining rectangles.
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

        /// <summary>
        /// Convert a <see cref="Rect"/> to a <see cref="RectInt32"/> that can be used with modern Windows APIs.
        /// <para/>
        /// If the source Rect has non-integral values, they will be rounded when they are converted to integers.
        /// </summary>
        /// <param name="rect">The rect to convert</param>
        /// <returns>A <see cref="RectInt32"/> with the same size</returns>
        public static RectInt32 ToRectInt32(this Rect rect)
        {
            return new RectInt32(_X: (int)rect.X, _Y: (int)rect.Y, _Width: (int)rect.Width, _Height: (int)rect.Height);
        }

        /// <summary>
        /// Convert a <see cref="Int32Rect"/> to a <see cref="RectInt32"/> that can be used with modern Windows APIs.
        /// </summary>
        /// <param name="rect">The rect to convert</param>
        /// <returns>A <see cref="RectInt32"/> with the same size</returns>
        public static RectInt32 ToRectInt32(this Int32Rect rect)
        {
            return new RectInt32(_X: rect.X, _Y: rect.Y, _Width: rect.Width, _Height: rect.Height);
        }
    }
}
