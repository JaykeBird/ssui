using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SolidShineUi
{
    public static class BrushSerializer
    {
        const string separator = ";";
        static readonly char[] splitChar = new char[] { ';' };
        static readonly char subSeparator = ',';
        private static readonly char[] subSplitChar = new char[] { subSeparator };

        private static string SerializeSolidColorBrush(SolidColorBrush brush)
        {
            string color = brush.Color.GetHexStringWithAlpha();
            return string.Join(separator, "s", color, SerializeBrushProperties(brush));
        }

        private static string SerializeLinearGradientBrush(LinearGradientBrush lgb)
        {
            string props = "";
            switch (lgb.MappingMode)
            {
                case BrushMappingMode.Absolute:
                    props += "a";
                    break;
                default:
                    break;
            }

            switch (lgb.ColorInterpolationMode)
            {
                case ColorInterpolationMode.ScRgbLinearInterpolation:
                    props += "i";
                    break;
                default:
                    break;
            }

            switch (lgb.SpreadMethod)
            {
                case GradientSpreadMethod.Pad:
                    props += "p";
                    break;
                case GradientSpreadMethod.Reflect:
                    props += "f";
                    break;
                case GradientSpreadMethod.Repeat:
                    props += "r";
                    break;
                default:
                    props += "p";
                    break;
            }

            string start = lgb.StartPoint.ToString(CultureInfo.InvariantCulture);
            string end = lgb.EndPoint.ToString(CultureInfo.InvariantCulture);

            string bprops = SerializeBrushProperties(lgb, props);

            if (string.IsNullOrEmpty(bprops))
            {
                return string.Join(separator, "l", start, end, SerializeGradientStopCollection(lgb.GradientStops));
            }
            else
            {
                return string.Join(separator, "l", start, end, SerializeGradientStopCollection(lgb.GradientStops), bprops);
            }
        }

        private static string SerializeRadialGradientBrush(RadialGradientBrush rgb)
        {
            string props = "";
            switch (rgb.MappingMode)
            {
                case BrushMappingMode.Absolute:
                    props += "a";
                    break;
                case BrushMappingMode.RelativeToBoundingBox:
                    props += "b";
                    break;
                default:
                    props += "b";
                    break;
            }

            switch (rgb.ColorInterpolationMode)
            {
                case ColorInterpolationMode.ScRgbLinearInterpolation:
                    props += "i";
                    break;
                default:
                    break;
            }

            switch (rgb.SpreadMethod)
            {
                case GradientSpreadMethod.Pad:
                    props += "p";
                    break;
                case GradientSpreadMethod.Reflect:
                    props += "f";
                    break;
                case GradientSpreadMethod.Repeat:
                    props += "r";
                    break;
                default:
                    props += "p";
                    break;
            }

            string start = rgb.GradientOrigin.ToString(CultureInfo.InvariantCulture);
            string end = rgb.Center.ToString(CultureInfo.InvariantCulture);
            string cx = rgb.RadiusX.ToString(CultureInfo.InvariantCulture);
            string cy = rgb.RadiusY.ToString(CultureInfo.InvariantCulture);

            string bprops = SerializeBrushProperties(rgb, props);

            if (string.IsNullOrEmpty(bprops))
            {
                return string.Join(separator, "r", start, end, cx, cy, SerializeGradientStopCollection(rgb.GradientStops));
            }
            else
            {
                return string.Join(separator, "r", start, end, cx, cy, SerializeGradientStopCollection(rgb.GradientStops), bprops);
            }
        }

        private static string SerializeGradientStopCollection(GradientStopCollection gsc)
        {
            // to aid with deserialization, we will want to make sure that the gradient stop collection has at least 2 items
            if (gsc.Count == 0)
            {
                // what the heck man - let's just create some transparent gradient stops
                gsc.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.0));
                gsc.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.0));
            }
            if (gsc.Count == 1)
            {
                double newOffset = gsc[0].Offset == 1 ? 0.99 : gsc[0].Offset + 0.01;
                gsc.Add(new GradientStop(gsc[0].Color, newOffset));
            }

            // {stop1Color}@{stop1Offset},{stop2Color}@{stop2Offset}
            IEnumerable<string> stops = gsc.Select(g => SerializeGradientStop(g));
            return string.Join(new string(subSeparator, 1), stops);
        }

        private static string SerializeGradientStop(GradientStop g)
        {
            return g.Color.GetHexStringWithAlpha() + "@" + g.Offset.ToString(CultureInfo.InvariantCulture);
        }

        private static string SerializeBrushProperties(Brush brush, string gradientBrushProperties = "")
        {
            List<string> values = new List<string>();

            if (brush.Opacity != 1.0)
            {
                values.Add("o" + subSeparator + brush.Opacity.ToString(CultureInfo.InvariantCulture));
            }

            if (brush.Transform != null && !brush.Transform.Value.IsIdentity)
            {
                values.Add("t" + subSeparator + TransformSerializer.SerializeTransform(brush.Transform));
            }

            if (brush.RelativeTransform != null && !brush.RelativeTransform.Value.IsIdentity)
            {
                values.Add("r" + subSeparator + TransformSerializer.SerializeTransform(brush.RelativeTransform));
            }

            if (!string.IsNullOrEmpty(gradientBrushProperties))
            {
                values.Add("p" + subSeparator + gradientBrushProperties);
            }

            if (values.Count == 0) return string.Empty;

            return string.Join(separator, values);
        }

        /// <summary>
        /// Create a string that represents this SolidColorBrush.
        /// <para/>
        /// Use <see cref="DeserializeBrush(string)"/> to convert the string back into a brush.
        /// </summary>
        public static string Serialize(this SolidColorBrush scb)
        {
            return SerializeSolidColorBrush(scb);
        }

        /// <summary>
        /// Create a string that represents this LinearGradientBrush.
        /// <para/>
        /// Use <see cref="DeserializeBrush(string)"/> to convert the string back into a brush.
        /// </summary>
        public static string Serialize(this LinearGradientBrush lgb)
        {
            return SerializeLinearGradientBrush(lgb);
        }

        /// <summary>
        /// Create a string that represents this RadialGradientBrush.
        /// <para/>
        /// Use <see cref="DeserializeBrush(string)"/> to convert the string back into a brush.
        /// </summary>
        public static string Serialize(this RadialGradientBrush rgb)
        {
            return SerializeRadialGradientBrush(rgb);
        }

        /// <summary>
        /// Create a string that represents this brush if possible. If not, an empty string is returned.
        /// <para/>
        /// Use <see cref="DeserializeBrush(string)"/> to convert the string back into a brush.
        /// </summary>
        /// <remarks>
        /// Only <see cref="SolidColorBrush"/>, <see cref="LinearGradientBrush"/>, and <see cref="RadialGradientBrush"/> are supported.
        /// All other brush types will return an empty string.
        /// </remarks>
        public static string Serialize(this Brush b)
        {
            if (b is SolidColorBrush scb)
            {
                return SerializeSolidColorBrush(scb);
            }
            else if (b is LinearGradientBrush lgb)
            {
                return SerializeLinearGradientBrush(lgb);
            }
            else if (b is RadialGradientBrush rgb)
            {
                return SerializeRadialGradientBrush(rgb);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Create a string that represents this gradient stop.
        /// <para/>
        /// Use <see cref="DeserializeGradientStop(string)"/> to convert the string back into a gradient stop.
        /// </summary>
        public static string Serialize(this GradientStop gs)
        {
            return SerializeGradientStop(gs);
        }

        /// <summary>
        /// Deserialize a brush string back into a brush.
        /// <para/>
        /// Brushes can be serialized/converted to a string using <see cref="Serialize(Brush)"/>.
        /// </summary>
        /// <param name="s">the string representing the brush to deserialize</param>
        /// <remarks>
        /// Only <see cref="SolidColorBrush"/>, <see cref="LinearGradientBrush"/>, and <see cref="RadialGradientBrush"/> are supported.
        /// The deserialized brush will be of the same type as the serialized type created using <see cref="Serialize(Brush)"/>.
        /// The returned brush will be frozen; use <see cref="DeserializeBrush(string, bool)"/> if you want an unfrozen brush.
        /// </remarks>
        /// <exception cref="FormatException">Thrown if an invalid/unparseable string is inputted</exception>
        public static Brush DeserializeBrush(string s)
        {
            return DeserializeBrush(s, true);
        }

        /// <summary>
        /// Deserialize a brush string back into a brush.
        /// <para/>
        /// Brushes can be serialized/converted to a string using <see cref="Serialize(Brush)"/>.
        /// </summary>
        /// <param name="s">the string representing the brush to deserialize</param>
        /// <param name="freeze">whether or not the returned brush should be frozen</param>
        /// <remarks>
        /// Only <see cref="SolidColorBrush"/>, <see cref="LinearGradientBrush"/>, and <see cref="RadialGradientBrush"/> are supported.
        /// The deserialized brush will be of the same type as the serialized type created using <see cref="Serialize(Brush)"/>.
        /// If <paramref name="freeze"/> is set to true, the returned brush will be frozen.
        /// </remarks>
        /// <exception cref="FormatException">Thrown if an invalid/unparseable string is inputted</exception>
        public static Brush DeserializeBrush(string s, bool freeze)
        {
            // if (s.Contains(':')) s = s.Replace(':', ',');

            if (!s.Contains(separator))
            {
                throw new FormatException("Invalid brush string");
            }
            else
            {
                string[] vals = s.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);

                switch (vals[0].ToLowerInvariant())
                {
                    case "s":
                        // solid color brush
                        try
                        {
                            string color = vals[1];

                            SolidColorBrush scb = new SolidColorBrush(ColorsHelper.CreateFromHex(color));

                            if (vals.Length > 2)
                            {
                                // there are other properties defined too
                                DeserializeBrushProperties(vals, 2, out double opacity, out Transform transform, out Transform relativeTransform, out string _);
                                scb.Opacity = opacity;
                                scb.Transform = transform;
                                scb.RelativeTransform = relativeTransform;
                            }

                            if (freeze && scb.CanFreeze) scb.Freeze();

                            return scb;
                        }
                        catch (FormatException ex)
                        {
                            throw new FormatException("Invalid brush string", ex);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new FormatException("Invalid brush string", ex);
                        }
                    case "l":
                        // linear gradient brush
                        if (vals.Length < 4)
                        {
                            throw new FormatException("Invalid brush string");
                        }

                        try
                        {
                            string start = vals[1];
                            string end = vals[2];
                            string stops = vals[3];
                            
                            if (start.StartsWith("b,", StringComparison.OrdinalIgnoreCase)) start = start.Substring(2);
                            if (end.StartsWith("c,", StringComparison.OrdinalIgnoreCase)) end = end.Substring(2);
                            if (stops.StartsWith("s,", StringComparison.OrdinalIgnoreCase)) stops = stops.Substring(2);

                            LinearGradientBrush lgb = new LinearGradientBrush(DeserializeGradientStopCollection(stops), Point.Parse(start), Point.Parse(end));

                            if (vals.Length > 4)
                            {
                                // there are other properties defined too
                                DeserializeBrushProperties(vals, 4, out double opacity, out Transform transform, out Transform relativeTransform, out string props);
                                lgb.Opacity = opacity;
                                lgb.Transform = transform;
                                lgb.RelativeTransform = relativeTransform;

                                DeserializeGradientBrushProperties(props, out var map, out var colorMode, out var spread);
                                lgb.MappingMode = map;
                                lgb.ColorInterpolationMode = colorMode;
                                lgb.SpreadMethod = spread;
                            }

                            if (freeze && lgb.CanFreeze) lgb.Freeze();

                            return lgb;
                        }
                        catch (FormatException ex)
                        {
                            throw new FormatException("Invalid brush string", ex);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new FormatException("Invalid brush string", ex);
                        }
                    case "r":
                        // radial gradient brush
                        if (vals.Length < 6)
                        {
                            throw new FormatException("Invalid brush string");
                        }

                        try
                        {
                            string start = vals[1];
                            string end = vals[2];
                            string cx = vals[3];
                            string cy = vals[4];
                            string stops = vals[5];

                            if (start.StartsWith("b,", StringComparison.OrdinalIgnoreCase)) start = start.Substring(2);
                            if (end.StartsWith("c,", StringComparison.OrdinalIgnoreCase)) end = end.Substring(2);
                            if (cx.StartsWith("x,", StringComparison.OrdinalIgnoreCase)) cx = cx.Substring(2);
                            if (cy.StartsWith("y,", StringComparison.OrdinalIgnoreCase)) cy = cy.Substring(2);
                            if (stops.StartsWith("s,", StringComparison.OrdinalIgnoreCase)) stops = stops.Substring(2);

                            RadialGradientBrush rgb = new RadialGradientBrush(DeserializeGradientStopCollection(stops));

                            rgb.GradientOrigin = Point.Parse(start);
                            rgb.Center = Point.Parse(end);
                            rgb.RadiusX = Convert.ToDouble(cx, CultureInfo.InvariantCulture);
                            rgb.RadiusY = Convert.ToDouble(cy, CultureInfo.InvariantCulture);

                            if (vals.Length > 6)
                            {
                                // there are other properties defined too
                                DeserializeBrushProperties(vals, 6, out double opacity, out Transform transform, out Transform relativeTransform, out string props);
                                rgb.Opacity = opacity;
                                rgb.Transform = transform;
                                rgb.RelativeTransform = relativeTransform;

                                DeserializeGradientBrushProperties(props, out var map, out var colorMode, out var spread);
                                rgb.MappingMode = map;
                                rgb.ColorInterpolationMode = colorMode;
                                rgb.SpreadMethod = spread;
                            }

                            if (freeze && rgb.CanFreeze) rgb.Freeze();

                            return rgb;
                        }
                        catch (FormatException ex)
                        {
                            throw new FormatException("Invalid brush string", ex);
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new FormatException("Invalid brush string", ex);
                        }
                    default:
                        throw new FormatException("Invalid brush string");
                }
            }
        }


        private static void DeserializeBrushProperties(string[] values, int startIndex, out double opacity, 
            out Transform transform, out Transform relativeTransform, out string extraProperties)
        {
            opacity = 1.0;
            transform = Transform.Identity;
            relativeTransform = Transform.Identity;
            extraProperties = "";

            for (int i = startIndex; i < values.Length; i++)
            {
                string val = values[i];
                if (!val.Contains(subSeparator))
                {
                    // probably not meant to be here
                    continue;
                }

                string[] subvals = val.Split(subSplitChar);

                switch (subvals[0].ToLowerInvariant())
                {
                    case "o":
                        // opacity
                        opacity = Convert.ToDouble(subvals[1], CultureInfo.InvariantCulture);
                        break;
                    case "t":
                        // transform
                        transform = TransformSerializer.DeserializeTransform(val.Substring(2));
                        break;
                    case "r":
                        // relative transform
                        relativeTransform = TransformSerializer.DeserializeTransform(val.Substring(2));
                        break;
                    case "p":
                        extraProperties = val.Substring(2);
                        break;
                }
            }
        }

        public static GradientStop DeserializeGradientStop(string stop)
        {
            try
            {
                int atIndex = stop.IndexOf('@');
                if (atIndex < 0) throw new FormatException("Gradient stop string invalid");
                string color = stop.Substring(0, atIndex);
                string offset = stop.Substring(atIndex + 1);

                return new GradientStop(ColorsHelper.CreateFromHex(color), Convert.ToDouble(offset, CultureInfo.InvariantCulture));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new FormatException("Gradient stop string invalid", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new FormatException("Gradient stop string invalid", ex);
            }
        }

        private static GradientStopCollection DeserializeGradientStopCollection(string stops)
        {
            if (!stops.Contains(subSeparator))
            {
                throw new FormatException("Gradient stops list incomplete");
            }

            string[] stopCol = stops.Split(subSplitChar, StringSplitOptions.RemoveEmptyEntries);

            GradientStopCollection gsc = new GradientStopCollection();

            foreach (string item in stopCol)
            {
                gsc.Add(DeserializeGradientStop(item));
            }

            return gsc;
        }

        private static void DeserializeGradientBrushProperties(string values, out BrushMappingMode map, 
            out ColorInterpolationMode colorMode, out GradientSpreadMethod spread)
        {
            if (string.IsNullOrEmpty(values)) // quick exit for an empty string
            {
                map = BrushMappingMode.RelativeToBoundingBox;
                colorMode = ColorInterpolationMode.SRgbLinearInterpolation;
                spread = GradientSpreadMethod.Pad;
                return;
            }

            if (values.Contains('a'))
            {
                map = BrushMappingMode.Absolute;
            }
            else
            {
                map = BrushMappingMode.RelativeToBoundingBox;
            }

            if (values.Contains('i'))
            {
                colorMode = ColorInterpolationMode.ScRgbLinearInterpolation;
            }
            else
            {
                colorMode = ColorInterpolationMode.SRgbLinearInterpolation;
            }

            if (values.Contains('p'))
            {
                spread = GradientSpreadMethod.Pad;
            }
            else if (values.Contains('f'))
            {
                spread = GradientSpreadMethod.Reflect;
            }
            else if (values.Contains('r'))
            {
                spread = GradientSpreadMethod.Repeat;
            }
            else
            {
                spread = GradientSpreadMethod.Pad;
            }
        }

    }
}
