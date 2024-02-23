using System;
using System.Windows;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Some helper converter functions to use with <see cref="PartialCornerRadiusConverter"/> and <see cref="PartialThicknessConverter"/>.
    /// </summary>
    public static class PartialValueHelper
    {

        /// <summary>
        /// Get a byte value to indicate the corners to use with <see cref="PartialCornerRadiusConverter"/>.
        /// </summary>
        /// <param name="topLeft">indicate if the top-left corner value should be applied</param>
        /// <param name="topRight">indicate if the top-right corner value should be applied</param>
        /// <param name="bottomLeft">indicate if the bottom-left corner value should be applied</param>
        /// <param name="bottomRight">indicate if the bottom-right corner value should be applied</param>
        /// <returns>A byte that indicates the selected corner values should be applied when used with a <see cref="PartialCornerRadiusConverter"/>.</returns>
        /// <remarks>
        /// The specific byte values returned are not the only ones that are allowed by <see cref="PartialCornerRadiusConverter"/>, but are some of the easiest to calculate
        /// given any possible combination of inputs. These byte values indicate which corner radius values to flow through the converter from the inputted value into the result,
        /// by passing this byte in as a parameter. A string value can also be used as a parameter, with "TL", "TR", "BL", and "BR" representing the corners to allow.
        /// </remarks>
        public static byte EncodeCornerRadiusPartialValue(bool topLeft = false, bool topRight = false, bool bottomLeft = false, bool bottomRight = false)
        {
            byte b = 1;

            if (topLeft) { b *= 2; }
            if (topRight) { b *= 3; }
            if (bottomLeft) { b *= 5; }
            if (bottomRight) { b *= 7; }

            if (b == 1) return 0;

            return b;
        }

        /// <summary>
        /// Get a byte value to indicate the corners to use with <see cref="PartialCornerRadiusConverter"/>.
        /// </summary>
        /// <param name="s">The string value to use to indicate which corner values to apply; can contain any values of "TL", "TR", "BL", and/or "BR"</param>
        /// <returns>A byte that indicates the selected corner values should be applied when used with a <see cref="PartialCornerRadiusConverter"/>.</returns>
        /// <remarks>
        /// The specific byte values returned are not the only ones that are allowed by <see cref="PartialCornerRadiusConverter"/>, but are some of the easiest to calculate
        /// given any possible combination of inputs. These byte values indicate which corner radius values to flow through the converter from the inputted value into the result,
        /// by passing this byte in as a parameter. The string value inputted here can also be used directly as the parameter for the converter.
        /// </remarks>
        public static byte EncodeCornerRadiusPartialValue(string s)
        {
            byte b = 1;

            if (s.Contains("TL")) { b *= 2; }
            if (s.Contains("TR")) { b *= 3; }
            if (s.Contains("BL")) { b *= 5; }
            if (s.Contains("BR")) { b *= 7; }

            if (b == 1) return 0;

            return b;
        }

        /// <summary>
        /// Get a byte value to indicate the edges/borders to use with <see cref="PartialThicknessConverter"/>.
        /// </summary>
        /// <param name="left">indicate if the left edge should be applied</param>
        /// <param name="top">indicate if the top edge should be applied</param>
        /// <param name="right">indicate if the right edge should be applied</param>
        /// <param name="bottom">indicate if the bottom edge should be applied</param>
        /// <returns>A byte that indicates the selected corner values should be applied when used with a <see cref="PartialThicknessConverter"/>.</returns>
        /// <remarks>
        /// The specific byte values returned are not the only ones that are allowed by <see cref="PartialThicknessConverter"/>, but are some of the easiest to calculate
        /// given any possible combination of inputs. These byte values indicate which edges of the border/margins values to flow through the converter from the inputted value into the result,
        /// by passing this byte in as a parameter. A string value can also be used as a parameter, with "L", "T", "R", and "B" representing the edges to allow.
        /// </remarks>
        public static byte EncodeThicknessPartialValue(bool left = false, bool top = false, bool right = false, bool bottom = false)
        {
            byte b = 1;

            if (top) { b *= 2; }
            if (left) { b *= 3; }
            if (right) { b *= 5; }
            if (bottom) { b *= 7; }

            if (b == 1) return 0;

            return b;
        }

        /// <summary>
        /// Get a byte value to indicate the edges/borders to use with <see cref="PartialThicknessConverter"/>.
        /// </summary>
        /// <param name="s">The string value to use to indicate which corner values to apply; can contain any values of "L", "T", "R", and/or "B"</param>
        /// <returns>A byte that indicates the selected corner values should be applied when used with a <see cref="PartialThicknessConverter"/>.</returns>
        /// <remarks>
        /// The specific byte values returned are not the only ones that are allowed by <see cref="PartialThicknessConverter"/>, but are some of the easiest to calculate
        /// given any possible combination of inputs. These byte values indicate which edges of the border/margins values to flow through the converter from the inputted value into the result,
        /// by passing this byte in as a parameter. This string value inputted here can also be used directly as the parameter for the converter.
        /// </remarks>
        public static byte EncodeThicknessPartialValue(string s)
        {
            byte b = 1;

#if NETCOREAPP
            if (s.Contains('T')) { b *= 2; }
            if (s.Contains('L')) { b *= 3; }
            if (s.Contains('R')) { b *= 5; }
            if (s.Contains('B')) { b *= 7; }
#else
            if (s.Contains("T")) { b *= 2; }
            if (s.Contains("L")) { b *= 3; }
            if (s.Contains("R")) { b *= 5; }
            if (s.Contains("B")) { b *= 7; }
#endif

            if (b == 1) return 0;

            return b;
        }

        /// <summary>
        /// Decode a byte value used with <see cref="PartialCornerRadiusConverter"/> to indicate which corners will be applied when the converter is used.
        /// </summary>
        /// <param name="b">The byte representing the corners to apply</param>
        /// <returns>
        /// A tuple containing 4 booleans, representing the four corners/values of a CornerRadius; if a particular bool is <c>true</c>,
        /// this indicates that corner would be passed through when used with the converter.
        /// </returns>
        /// <remarks>
        /// This method is the direct inverse of <see cref="EncodeCornerRadiusPartialValue(bool, bool, bool, bool)"/>. This method is used by the converter
        /// to decode the inputted parameter and determine which corner radius values to pass through from the input value into the result.
        /// </remarks>
        public static (bool topLeft, bool topRight, bool bottomLeft, bool bottomRight) DecodeCornerRadiusPartialValue(byte b)
        {
            (bool topLeft, bool topRight, bool bottomLeft, bool bottomRight) res = (false, false, false, false);

            if (b % 2 == 0)
            {
                res.topLeft = true;
            }
            if (b % 3 == 0)
            {
                res.topRight = true;
            }
            if (b % 5 == 0)
            {
                res.bottomLeft = true;
            }
            if (b % 7 == 0)
            {
                res.bottomRight = true;
            }

            return res;
        }

        /// <summary>
        /// Decode a string value used with <see cref="PartialCornerRadiusConverter"/> to indicate which corners will be applied when the converter is used.
        /// </summary>
        /// <param name="s">The string representing the corners to apply</param>
        /// <returns>
        /// A tuple containing 4 booleans, representing the four corners/values of a CornerRadius; if a particular bool is <c>true</c>,
        /// this indicates that corner would be passed through when used with the converter.
        /// </returns>
        /// <remarks>
        /// This method is the inverse of <see cref="EncodeCornerRadiusPartialValue(bool, bool, bool, bool)"/>, and a complement to <see cref="EncodeCornerRadiusPartialValue(string)"/>.
        /// This method is used by the converter to decode the inputted parameter and determine which corner radius values to pass through from the input value into the result.
        /// </remarks>
        public static (bool topLeft, bool topRight, bool bottomLeft, bool bottomRight) DecodeCornerRadiusPartialValue(string s)
        {
            (bool topLeft, bool topRight, bool bottomLeft, bool bottomRight) res = (false, false, false, false);

            if (s.Contains("TL"))
            {
                res.topLeft = true;
            }
            if (s.Contains("TR"))
            {
                res.topRight = true;
            }
            if (s.Contains("BL"))
            {
                res.bottomLeft = true;
            }
            if (s.Contains("BR"))
            {
                res.bottomRight = true;
            }

            return res;
        }

        /// <summary>
        /// Decode a byte value used with <see cref="PartialThicknessConverter"/> to indicate which edges will be applied when the converter is used.
        /// </summary>
        /// <param name="b">The byte representing the edges to apply</param>
        /// <returns>
        /// A tuple containing 4 booleans, representing the four edges/values of a Thickness; if a particular bool is <c>true</c>,
        /// this indicates that edge would be passed through when used with the converter.
        /// </returns>
        /// <remarks>
        /// This method is the direct inverse of <see cref="EncodeThicknessPartialValue(bool, bool, bool, bool)"/>. This method is used by the converter
        /// to decode the inputted parameter and determine which edge/border values to pass through from the input value into the result.
        /// </remarks>
        public static (bool left, bool top, bool right, bool bottom) DecodeThicknessPartialValue(byte b)
        {
            (bool left, bool top, bool right, bool bottom) res = (false, false, false, false);

            if (b % 2 == 0)
            {
                res.top = true;
            }
            if (b % 3 == 0)
            {
                res.left = true;
            }
            if (b % 5 == 0)
            {
                res.right = true;
            }
            if (b % 7 == 0)
            {
                res.bottom = true;
            }

            return res;
        }

        /// <summary>
        /// Decode a string value used with <see cref="PartialThicknessConverter"/> to indicate which corners will be applied when the converter is used.
        /// </summary>
        /// <param name="s">The string representing the edges to apply</param>
        /// <returns>
        /// A tuple containing 4 booleans, representing the four edges/values of a Thickness; if a particular bool is <c>true</c>,
        /// this indicates that edge would be passed through when used with the converter.
        /// </returns>
        /// <remarks>
        /// This method is the inverse of <see cref="EncodeThicknessPartialValue(bool, bool, bool, bool)"/>, and a complement to <see cref="EncodeThicknessPartialValue(string)"/>.
        /// This method is used by the converter to decode the inputted parameter and determine which edge/border values to pass through from the input value into the result.
        /// </remarks>
        public static (bool left, bool top, bool right, bool bottom) DecodeThicknessPartialValue(string s)
        {
            (bool left, bool top, bool right, bool bottom) res = (false, false, false, false);

#if NETCOREAPP
            if (s.Contains('T'))
            {
                res.top = true;
            }
            if (s.Contains('L'))
            {
                res.left = true;
            }
            if (s.Contains('R'))
            {
                res.right = true;
            }
            if (s.Contains('B'))
            {
                res.bottom = true;
            }
#else
            if (s.Contains("T"))
            {
                res.top = true;
            }
            if (s.Contains("L"))
            {
                res.left = true;
            }
            if (s.Contains("R"))
            {
                res.right = true;
            }
            if (s.Contains("B"))
            {
                res.bottom = true;
            }
#endif

            return res;
        }

    }
}