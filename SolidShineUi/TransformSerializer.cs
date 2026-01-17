using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Converters;

namespace SolidShineUi
{

    /// <summary>
    /// A helper class to serialize and deserialize WPF <see cref="Transform"/> objects to and from strings.
    /// </summary>
    /// <remarks>
    /// The standard Serialize methods will create special strings for the common Transform types 
    /// (<see cref="ScaleTransform"/>, <see cref="SkewTransform"/>, <see cref="RotateTransform"/>, <see cref="TranslateTransform"/>, <see cref="TransformGroup"/>)
    /// and then fall back to serializing the Transform's Matrix value for any remaining types. <see cref="SerializeAsMatrix(Transform)"/> can also be used to
    /// always serialize a transform with its matrix value, even if it's one of the common types.
    /// <para/>
    /// This class also adds on some extension methods to <see cref="Transform"/> to facilitate easily serializing those objects there.
    /// </remarks>
    public static class TransformSerializer
    {
        /// <summary>the character to use for separating values in a Transform; this should be the same as <c>subSeparator</c> in BrushSerializer</summary>
        const string separator = ",";
        /// <summary>the character to use for separating child transforms when serializing a TransformGroup</summary>
        static readonly char groupSeparator = '/';
        /// <summary>the group separator, put into a char array for easy usage with <see cref="string.Split(char[], StringSplitOptions)"/></summary>
        private static readonly char[] groupSplitChar = new char[] { groupSeparator };
        /// <summary>the separator, put into a char array for easy usage with <see cref="string.Split(char[], StringSplitOptions)"/></summary>
        private static readonly char[] splitChar = new char[] { ',' };

        private static string SerializeRotateTransform(RotateTransform rt)
        {
            return string.Join(separator, "tr", GetDouble(rt.Angle), GetDouble(rt.CenterX), GetDouble(rt.CenterY));
        }

        private static string SerializeSkewTransform(SkewTransform st)
        {
            return string.Join(separator, "tk", GetDouble(st.AngleX), GetDouble(st.AngleY), GetDouble(st.CenterX), GetDouble(st.CenterY));
        }

        private static string SerializeScaleTransform(ScaleTransform st)
        {
            return string.Join(separator, "ts", GetDouble(st.ScaleX), GetDouble(st.ScaleY), GetDouble(st.CenterX), GetDouble(st.CenterY));
        }

        private static string SerializeTranslateTransform(TranslateTransform tt)
        {
            return string.Join(separator, "tt", GetDouble(tt.X), GetDouble(tt.Y));
        }

        private static string SerializeTransformGroup(TransformGroup tg)
        {
            if (tg.Children == null || tg.Children.Count == 0)
            {
                return SerializeTransform(Transform.Identity);
            }
            else
            {
                return SerializeTransformCollection(tg.Children);
            }
        }

        private static string SerializeTransformCollection(TransformCollection tg)
        {
            return string.Join(new string(groupSeparator, 1), tg.Select(t => SerializeTransform(t)));
        }

        /// <summary>
        /// Get a string that represents the value of a transform. This string can later be used to recreate this transform using 
        /// <see cref="DeserializeTransform(string)"/>.
        /// </summary>
        /// <param name="transform">the transform to serialize</param>
        /// <remarks>
        /// This has special handling for <see cref="RotateTransform"/>, <see cref="TranslateTransform"/>, 
        /// <see cref="ScaleTransform"/>, <see cref="SkewTransform"/>, and <see cref="TransformGroup"/>, and will return a special string
        /// that contains the values of those transforms directly.
        /// All other transforms create a string by taking the transform's matrix value and using <see cref="Matrix.ToString()"/>.
        /// </remarks>
        public static string SerializeTransform(Transform transform)
        {
            if (transform is RotateTransform rt)
            {
                return SerializeRotateTransform(rt);
            }
            else if (transform is TranslateTransform tt)
            {
                return SerializeTranslateTransform(tt);
            }
            else if (transform is ScaleTransform st)
            {
                return SerializeScaleTransform(st);
            }
            else if (transform is SkewTransform kt)
            {
                return SerializeSkewTransform(kt);
            }
            else if (transform is TransformGroup tg)
            {
                return SerializeTransformGroup(tg);
            }
            else
            {
                return string.Join(separator, "tm", transform.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Get a string that represents this transform. This string can be converted back into this transform using 
        /// <see cref="DeserializeTransform(string)"/>.
        /// </summary>
        /// <param name="transform">the transform to serialize</param>
        /// <remarks>See the remarks for <see cref="SerializeTransform(Transform)"/> for more details.</remarks>
        public static string Serialize(this Transform transform)
        {
            return SerializeTransform(transform);
        }

        /// <summary>
        /// Get a string that represents this transform, by writing out the matrix value of this transform.
        /// This string can be converted back into a <see cref="MatrixTransform"/> using <see cref="DeserializeTransform(string)"/>.
        /// </summary>
        /// <param name="transform">the transform to serialize</param>
        /// <remarks>
        /// This will create a consistently formatted string, regardless of the type of transform, whereas <see cref="Serialize(Transform)"/>
        /// will create special strings for serializing common Transform types.
        /// </remarks>
        public static string SerializeAsMatrix(this Transform transform)
        {
            return string.Join(separator, "tm", transform.Value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Get a string that represents all of the transforms in this collection. This string can be converted into a 
        /// <see cref="TransformGroup"/> by using <see cref="DeserializeTransform(string)"/>.
        /// </summary>
        /// <param name="tc">the collection to serialize</param>
        public static string Serialize(this TransformCollection tc)
        {
            return SerializeTransformCollection(tc);
        }

        /// <summary>
        /// Parse a string and return a <see cref="Transform"/> object based upon its contents.
        /// </summary>
        /// <param name="s">the string to parse/deserialize</param>
        /// <returns>A <see cref="Transform"/> with a value based upon the contents of the string</returns>
        /// <exception cref="FormatException">Thrown if the string <paramref name="s"/> is not a valid parseable string</exception>
        /// <remarks>
        /// If the string was made by serializing a <see cref="RotateTransform"/>, <see cref="SkewTransform"/>,
        /// <see cref="ScaleTransform"/>, <see cref="TranslateTransform"/>, or <see cref="TransformGroup"/> 
        /// using <see cref="Serialize(Transform)"/> or <see cref="SerializeTransform(Transform)"/>, then the returned object
        /// will be of that same type. Otherwise, a <see cref="MatrixTransform"/> will be returned by parsing the Matrix value
        /// within this string.
        /// </remarks>
        public static Transform DeserializeTransform(string s)
        {
            if (s[0] != 't')
            {
                throw new FormatException("Not valid transform serialization string");
            }

            // check if it's a TransformGroup
            if (s.Contains(groupSeparator))
            {
                string[] strings = s.Split(groupSplitChar, StringSplitOptions.RemoveEmptyEntries);
                TransformCollection tc = new TransformCollection();
                foreach (string s2 in strings)
                {
                    tc.Add(DeserializeTransform(s2));
                }

                return new TransformGroup() { Children = tc };
            }
            else
            {
                string[] vals = s.Split(splitChar); // we don't want to remove empty entries, as those actually mean 0
                switch(vals[0].ToLowerInvariant())
                {
                    case "tr":
                        // rotate transform
                        if (vals.Length < 4) throw new FormatException("Not valid transform serialization string");
                        return new RotateTransform(ConvDouble(vals[1]), ConvDouble(vals[2]), ConvDouble(vals[3]));
                    case "tk":
                        // skew transform
                        if (vals.Length < 5) throw new FormatException("Not valid transform serialization string");
                        return new SkewTransform(ConvDouble(vals[1]), ConvDouble(vals[2]), ConvDouble(vals[3]), ConvDouble(vals[4]));
                    case "ts":
                        // scale transform
                        if (vals.Length < 5) throw new FormatException("Not valid transform serialization string");
                        return new ScaleTransform(ConvDouble(vals[1]), ConvDouble(vals[2]), ConvDouble(vals[3]), ConvDouble(vals[4]));
                    case "tt":
                        // translate transform
                        if (vals.Length < 3) throw new FormatException("Not valid transform serialization string");
                        return new TranslateTransform(ConvDouble(vals[1]), ConvDouble(vals[2]));
                    case "tm":
                        // matrix transform
                        if (vals.Length < 2) throw new FormatException("Not valid transform serialization string");
                        try
                        {
                            return new MatrixTransform(Matrix.Parse(vals[1]));
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new FormatException("Not valid transform serialization string", ex);
                        }
                    default:
                        throw new FormatException("Not valid transform serialization string");
                }
            }
        }

        private static string GetDouble(double d)
        {
            if (d == 0.0d) return string.Empty; // special handling for zero
            return d.ToString(CultureInfo.InvariantCulture);
        }

        private static double ConvDouble(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0.0;
            else return Convert.ToDouble(s, CultureInfo.InvariantCulture);
        }

    }
}
