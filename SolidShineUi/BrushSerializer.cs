using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A helper class to serialize and deserialize brushes to and from strings.
    /// </summary>
    /// <remarks>
    /// Note that this only works for <see cref="SolidColorBrush"/>, <see cref="LinearGradientBrush"/>, and <see cref="RadialGradientBrush"/>.
    /// The strings created by this class are a lot shorter than serializing to XAML while containing the same data, making these easier to be
    /// transferred and stored. These strings can also be theoretically consumed or created by other GUI systems as a cross-compatible system.
    /// <para/>
    /// This class also adds on some extension methods to <see cref="Brush"/> and <see cref="GradientStop"/> to facilitate easily serializing those objects there.
    /// </remarks>
    public static class BrushSerializer
    {
        /*
        
        some thoughts on brush serialization:

        serializing WPF objects is a bit harder than normal due to the fact that WPF DependencyObjects tend to contain a number of properties and data that isn't meant
        to be stored (and is instead to be generated in real-time as needed), and (relatedly, or perhaps as a result) some properties end up tripping up serialization
        methods. The common workaround I've seen people refer to and use online seems to be XAML serialization, which luckily is a thing that is present included in
        WPF via the XamlWriter and XamlReader classes (and other stuff in that area). If all someone cares about is just serialization without caring about how long
        it is and how it looks, that works well enough.
         
        perhaps the convenience and ability to serialize/deserialize to XAML kind of defeats the purpose of making this, but I decided to do this anyway. This method
        is more straight forward in that it generates strings, rather than needing to set up a StringBuilder or other object to contain the written XAML text before
        converting that to a string. This also generates strings that one can also then reuse in other UI systems/libraries, such as Avalonia. And finally, of course,
        the strings generated here have the same data as the XAML serializations while using a lot less characters (although this code's outputs could be made even
        more compact, but I have no desire to do that.)

        there is one key problem that creating these bespoke serializers have over the XAML serialization, in which is that I had to write special code for each
        particular type of Brush I wanted to support. in comparison, XAML serialization can handle a lot more arbitrary and convoluted situations, as it's a pretty
        generic serialization system.

        in here, I opted to only support SolidColorBrush, LinearGradientBrush, and RadialGradientBrush. These have a set list of properties and don't have the potential
        to create endless options and complications in a way that an ImageBrush or DrawingBrush can. trying to support the various options of a drawing or an image
        source is beyond what I wanted to accomplish here and now, although I suppose that can be addressed in the future if needed/wanted. I could, of course, just
        only support a subset of options and features (such as only supporting certain types of image sources), but I figure it's easier starting off by drawing the
        line at the brush type level, rather than having to at this time think about and consider the syntax and how to draw the line for these other types.

        Simultaneously conveniently and inconveniently, WPF's brush classes are sealed or built in a way that can't be properly inherited from (due to the usage of
        internal virtual methods). so that means the existing list of brushes in WPF that we have right now is all we'll ever have (unless Microsoft, for some reason,
        decides to create a whole new type of brush; they are the only ones that can do so). I'm generally not a fan of how much code that Microsoft made internal
        for WPF that someone like me won't be able to come along and modify or build upon, but I think with a core class like brushes, it was a good call here
        
        */

        /// <summary>the character to use to separate out blocks of properties or data; similar data should be separated via a subSeparator</summary>
        const string separator = ";";
        /// <summary>the separator, put into a char array for easy usage with <see cref="string.Split(char[], StringSplitOptions)"/></summary>
        static readonly char[] splitChar = new char[] { ';' };
        /// <summary>the character to use to separate like data or subdata within a datum</summary>
        static readonly char subSeparator = ',';
        /// <summary>the subSeparator, put into a char array for easy usage with <see cref="string.Split(char[], StringSplitOptions)"/></summary>
        private static readonly char[] subSplitChar = new char[] { subSeparator };

        private static string SerializeSolidColorBrush(SolidColorBrush brush)
        {
            string color = brush.Color.GetHexStringWithAlpha();

            string bprops = SerializeBrushProperties(brush);

            if (string.IsNullOrEmpty(bprops))
            {
                return string.Join(separator, "s", color);
            }
            else
            {
                return string.Join(separator, "s", color, bprops);
            }
            // SolidColorBrush is type "s"
        }

        private static string SerializeLinearGradientBrush(LinearGradientBrush lgb)
        {
            string props = "";
            switch (lgb.MappingMode)
            {
                case BrushMappingMode.Absolute:
                    props += "aa"; // two A's, as Avalonia has the ability to set the relativeness of the start and end points independently of one another
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
            // LinearGradientBrush is type "l"
        }

        private static string SerializeRadialGradientBrush(RadialGradientBrush rgb)
        {
            string props = "";
            switch (rgb.MappingMode)
            {
                case BrushMappingMode.Absolute:
                    props += "aa"; // two A's, as Avalonia has the ability to set the relativeness of the start and end points independently of one another
                    break;
                default:
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
            // RadialGradientBrush is of type "r"
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
                // we'll create another gradient stop that's nearby with the same color, so it should look the same visually
                double newOffset = gsc[0].Offset == 1 ? 0.99 : gsc[0].Offset + 0.01;
                gsc.Add(new GradientStop(gsc[0].Color, newOffset));
            }

            // {stop1Color}@{stop1Offset},{stop2Color}@{stop2Offset}
            IEnumerable<string> stops = gsc.Select(g => SerializeGradientStop(g));
            return string.Join(new string(subSeparator, 1), stops);
        }

        /// <summary>
        /// Create a string representing a GradientStop, with the format being "<c>color</c>@<c>offset</c>" (e.g., "<c>FF00FF88@0.5</c>").
        /// </summary>
        /// <param name="g">the stop to serialize</param>
        public static string SerializeGradientStop(GradientStop g)
        {
            return g.Color.GetHexStringWithAlpha() + "@" + g.Offset.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Creates a string to represent various common brush properties, such as <see cref="Brush.Opacity"/> and <see cref="Brush.Transform"/>.
        /// </summary>
        /// <param name="brush">the brush to read property data from</param>
        /// <param name="gradientBrushProperties">a string representing the common gradient brush properties, to add on if present</param>
        /// <remarks>
        /// Each property has a unique value ('<c>o</c>' for Opacity, '<c>t</c>' for Transform, and '<c>r</c>' for RelativeTransform), and then
        /// separated with the separator ('<c>;</c>'), although <paramref name="gradientBrushProperties"/> are combined together under the 'p' value.
        /// Transforms are serialized using <see cref="TransformSerializer"/>.
        /// </remarks>
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
        /// <remarks>
        /// The format is "<c>color</c>@<c>offset</c>" with the color represented by a hex code (e.g., "<c>FF00FF88@0.5</c>").
        /// </remarks>
        public static string Serialize(this GradientStop gs)
        {
            return SerializeGradientStop(gs);
        }

        /// <summary>
        /// Deserialize a brush string back into a <see cref="Brush"/>.
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
        /// Deserialize a brush string back into a <see cref="Brush"/>.
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

                switch (vals[0].ToLowerInvariant()) // the initial character tells us what type of brush this is
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
                            
                            // we'll optionally support if a property-defining character was added at the beginning of each value; we'll just remove the characters
                            // this does not change the fact that these values still need to be in the correct order
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

                            // we'll optionally support if a property-defining character was added at the beginning of each value; we'll just remove the characters
                            // this does not change the fact that these values still need to be in the correct order
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

        /// <summary>
        /// Deserialize a collection of common brush properties from a string array into individual variables to apply to a brush.
        /// </summary>
        /// <param name="values">the string array of values to deserialize (each value as its own item)</param>
        /// <param name="startIndex">the index in the array to start reading from; earlier indices would've been handled by the calling method</param>
        /// <param name="opacity">the opacity value read from the array ('<c>o</c>' value); default is <c>1.0</c> if not present</param>
        /// <param name="transform">the transform value read from the array ('<c>t</c>' value); default is <see cref="Transform.Identity"/> if not present</param>
        /// <param name="relativeTransform">the relative transform value read from the array ('<c>r</c>' value); default is <see cref="Transform.Identity"/> if not present</param>
        /// <param name="extraProperties">if a '<c>p</c>' value is present, the text that was contained with that value; otherwise, an empty string</param>
        private static void DeserializeBrushProperties(string[] values, int startIndex, out double opacity, 
            out Transform transform, out Transform relativeTransform, out string extraProperties)
        {
            // unlike the properties unique to each brush, these can be defined in any order
            // (or in fact, even multiple can be put in, but in that case, only the last one defined will be the one returned)

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

        /// <summary>
        /// Create a <see cref="GradientStop"/> by deserializing a string representing that gradient stop.
        /// <para/>
        /// GradientStops can be serialized using <see cref="SerializeGradientStop(GradientStop)"/>.
        /// </summary>
        /// <param name="str">the string to deserialize</param>
        /// <exception cref="FormatException">thrown if the inputted text is invalid and can't be parsed</exception>
        public static GradientStop DeserializeGradientStop(string str)
        {
            try
            {
                int atIndex = str.IndexOf('@');
                if (atIndex < 0) throw new FormatException("Gradient stop string invalid");
                string color = str.Substring(0, atIndex);
                string offset = str.Substring(atIndex + 1);

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

        private static void DeserializeGradientBrushProperties(string values, out BrushMappingMode mappingMode, 
            out ColorInterpolationMode colorMode, out GradientSpreadMethod spread)
        {
            if (string.IsNullOrEmpty(values)) // quick exit for an empty string
            {
                mappingMode = BrushMappingMode.RelativeToBoundingBox;
                colorMode = ColorInterpolationMode.SRgbLinearInterpolation;
                spread = GradientSpreadMethod.Pad;
                return;
            }

            // MappingMode
            //
            // unlike Avalonia, here the point mapping is applied to the whole brush, rather than just each point, so just finding 
            // one A will be enough to trigger this brush as being in absolute mode; no need to search for 'ab' or 'ba' or 'aa'
            if (values.Contains('a'))
            {
                mappingMode = BrushMappingMode.Absolute;
            }
            else
            {
                mappingMode = BrushMappingMode.RelativeToBoundingBox;
            }

            // ColorInterpolationMode
            if (values.Contains('i'))
            {
                colorMode = ColorInterpolationMode.ScRgbLinearInterpolation;
            }
            else
            {
                colorMode = ColorInterpolationMode.SRgbLinearInterpolation;
            }

            // SpreadMethod
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
