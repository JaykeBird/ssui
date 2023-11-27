#if false
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Globalization;
using System.Runtime.Serialization;

// this was one of my original attempts to get shadows and window effects working again after I accidentally broke it
// now I've figured out what my error was and fixed it, so I no longer need this at all
// I'll keep this here and un-compiled in the 1.9.7 branch, because it's just nice to have as a reference if needed, this won't be brought forward to 2.0
// note that I will not provide any support or help with this code, you may have better luck at the stackoverflow link below

namespace SolidShineUi.Utils
{
    // from https://stackoverflow.com/questions/3372303/dropshadow-for-wpf-borderless-window/6313576#6313576

    /// <summary>
    /// A helper class to provide a drop shadow on windows (for when using Windows Vista or later).
    /// </summary>
    public static class DwmDropShadow
    {
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        /// <summary>
        /// Drops a standard shadow to a WPF Window, even if the window is borderless. Only works with DWM (Windows Vista or newer).
        /// This method is much more efficient than setting AllowsTransparency to true and using the DropShadow effect,
        /// as AllowsTransparency involves a huge performance issue (hardware acceleration is turned off for all the window).
        /// </summary>
        /// <param name="window">Window to which the shadow will be applied</param>
        public static void DropShadowToWindow(Window window)
        {
            if (!DropShadow(window))
            {
                window.SourceInitialized += new EventHandler(window_SourceInitialized);
            }
        }

        private static void window_SourceInitialized(object sender, EventArgs e)
        {
            Window window = (Window)sender;

            DropShadow(window);

            window.SourceInitialized -= new EventHandler(window_SourceInitialized);
        }

        /// <summary>
        /// The actual method that makes API calls to drop the shadow to the window
        /// </summary>
        /// <param name="window">Window to which the shadow will be applied</param>
        /// <returns>True if the method succeeded, false if not</returns>
        private static bool DropShadow(Window window)
        {
            try
            {
                WindowInteropHelper helper = new WindowInteropHelper(window);
                int val = 2;
                int ret1 = DwmSetWindowAttribute(helper.Handle, 2, ref val, 4);

                if (ret1 == 0)
                {
                    Margins m = new Margins { Bottom = 0, Left = 0, Right = 0, Top = 0 };
                    int ret2 = DwmExtendFrameIntoClientArea(helper.Handle, ref m);
                    return ret2 == 0;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // Probably dwmapi.dll not found (incompatible OS)
                return false;
            }
        }
    }

    /// <summary>
    /// A measurement of margins of an object, such as when placed upon a page.
    /// </summary>
    /// <remarks>
    /// This is directly copied from <c>System.Drawing.Printing.Margins</c>, since that library isn't included in recent versions of .NET.
    /// Since this is used in the native Windows API call that <see cref="DwmDropShadow"/> uses, this is why I had to copy this here.
    /// </remarks>
    public class Margins : ICloneable
    {
        private int left;

        private int right;

        private int top;

        private int bottom;

        [OptionalField]
        private double doubleLeft;

        [OptionalField]
        private double doubleRight;

        [OptionalField]
        private double doubleTop;

        [OptionalField]
        private double doubleBottom;

        /// <summary>
        /// Get or sets the left margin, in hundredths of an inch.
        /// </summary>
        /// <exception cref="ArgumentException">thrown if set to a value less than 0</exception>
        public int Left
        {
            get
            {
                return left;
            }
            set
            {
                Margins.CheckMargin(value, "Left");
                left = value;
                doubleLeft = value;
            }
        }

        /// <summary>
        /// Get or sets the right margin, in hundredths of an inch.
        /// </summary>
        /// <exception cref="ArgumentException">thrown if set to a value less than 0</exception>
        public int Right
        {
            get
            {
                return right;
            }
            set
            {
                Margins.CheckMargin(value, "Right");
                right = value;
                doubleRight = value;
            }
        }

        /// <summary>
        /// Get or sets the top margin, in hundredths of an inch.
        /// </summary>
        /// <exception cref="ArgumentException">thrown if set to a value less than 0</exception>
        public int Top
        {
            get
            {
                return top;
            }
            set
            {
                Margins.CheckMargin(value, "Top");
                top = value;
                doubleTop = value;
            }
        }

        /// <summary>
        /// Get or sets the bottom margin, in hundredths of an inch.
        /// </summary>
        /// <exception cref="ArgumentException">thrown if set to a value less than 0</exception>
        public int Bottom
        {
            get
            {
                return bottom;
            }
            set
            {
                Margins.CheckMargin(value, "Bottom");
                bottom = value;
                doubleBottom = value;
            }
        }

        internal double DoubleLeft
        {
            get
            {
                return doubleLeft;
            }
            set
            {
                Left = (int)Math.Round(value);
                doubleLeft = value;
            }
        }

        internal double DoubleRight
        {
            get
            {
                return doubleRight;
            }
            set
            {
                Right = (int)Math.Round(value);
                doubleRight = value;
            }
        }

        internal double DoubleTop
        {
            get
            {
                return doubleTop;
            }
            set
            {
                Top = (int)Math.Round(value);
                doubleTop = value;
            }
        }

        internal double DoubleBottom
        {
            get
            {
                return doubleBottom;
            }
            set
            {
                Bottom = (int)Math.Round(value);
                doubleBottom = value;
            }
        }

        /// <summary>
        /// Create a new Margins with 1-inch wide margins (each value is set to 100).
        /// </summary>
        public Margins()
            : this(100, 100, 100, 100)
        {
        }

        /// <summary>
        /// Create a new Margins with the specified left, right, top, and bottom margins.
        /// </summary>
        /// <param name="left">the left margin, in hundredths of an inch</param>
        /// <param name="right">the right margin, in hundredths of an inch</param>
        /// <param name="top">the top margin, in hundredths of an inch</param>
        /// <param name="bottom">the bottom margin, in hundredths of an inch</param>
        /// <exception cref="ArgumentException">if any of the parameters are less than 0</exception>
        public Margins(int left, int right, int top, int bottom)
        {
            Margins.CheckMargin(left, "left");
            Margins.CheckMargin(right, "right");
            Margins.CheckMargin(top, "top");
            Margins.CheckMargin(bottom, "bottom");
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
            doubleLeft = left;
            doubleRight = right;
            doubleTop = top;
            doubleBottom = bottom;
        }

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            if (doubleLeft == 0.0 && left != 0)
            {
                doubleLeft = left;
            }

            if (doubleRight == 0.0 && right != 0)
            {
                doubleRight = right;
            }

            if (doubleTop == 0.0 && top != 0)
            {
                doubleTop = top;
            }

            if (doubleBottom == 0.0 && bottom != 0)
            {
                doubleBottom = bottom;
            }
        }

        /// <summary>
        /// Check if a margin value is below 0. If so, this throws an <see cref="ArgumentException"/>.
        /// </summary>
        /// <param name="margin">the margin value to check</param>
        /// <param name="name">the name of the value to provide in the ArgumentException, if thrown</param>
        /// <exception cref="ArgumentException">thrown if <paramref name="margin"/> is less than 0</exception>
        private static void CheckMargin(int margin, string name)
        {
            if (margin < 0)
            {
                throw new ArgumentException($"value {name} cannot be below 0; inputted value: {margin}");
            }
        }

        /// <summary>
        /// Retrieves a duplicate of this object, member by member.
        /// </summary>
        /// <returns>A duplicate of this object.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Compares this Margins object to the specified <see cref="object"/> to determine whether they have the same dimensions.
        /// </summary>
        /// <param name="obj">the object to which to compare this Margins</param>
        /// <returns>
        /// <c>true</c> if the object is a Margins, and has the same <see cref="Top"/>, <see cref="Bottom"/>, <see cref="Right"/>, and <see cref="Left"/> values as this;
        /// otherwise, <c>false</c>
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            else if (obj is Margins margins)
            {
                if (margins == this)
                {
                    return true;
                }

                if (margins.Left == Left && margins.Right == Right && margins.Top == Top)
                {
                    return margins.Bottom == Bottom;
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Calculates and retrieves a hash code based on the width of the left, right, top, and bottom margins.
        /// </summary>
        /// <returns>A hash code based on the left, right, top, and bottom margins.</returns>
        public override int GetHashCode()
        {
            uint num = (uint)Left;
            uint num2 = (uint)Right;
            uint num3 = (uint)Top;
            uint num4 = (uint)Bottom;
            return (int)(num ^ ((num2 << 13) | (num2 >> 19)) ^ ((num3 << 26) | (num3 >> 6)) ^ ((num4 << 7) | (num4 >> 25)));
        }

        /// <summary>
        /// Compares two Margins to determine if they have the same dimensions.
        /// </summary>
        /// <param name="m1">The first Margins to compare for equality</param>
        /// <param name="m2">The second Margins to compare for equality</param>
        /// <returns>
        /// <c>true</c> to indiciate the <see cref="Left"/>, <see cref="Right"/>, <see cref="Top"/>, and <see cref="Bottom"/> properties of both margins have the same value;
        /// otherwise, <c>false</c>
        /// </returns>
        public static bool operator ==(Margins m1, Margins m2)
        {
            if (m1 is null != (m2 is null))
            {
                return false;
            }

            if (!(m1 is null) && !(m2 is null))
            {
                if (m1.Left == m2.Left && m1.Top == m2.Top && m1.Right == m2.Right)
                {
                    return m1.Bottom == m2.Bottom;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Compares two Margins to determine whether they are of unequal width.
        /// </summary>
        /// <param name="m1">The first Margins to compare for inequality.</param>
        /// <param name="m2">The second Margins to compare for inequality.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Left"/>, <see cref="Right"/>, <see cref="Top"/>, or <see cref="Bottom"/> properties of both margins are not equal;
        /// otherwise, <c>false</c>
        /// </returns>
        public static bool operator !=(Margins m1, Margins m2)
        {
            return !(m1 == m2);
        }

        /// <summary>
        /// Converts the Margins to a <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of the Margins.</returns>
        public override string ToString()
        {
            return "[Margins Left=" + Left.ToString(CultureInfo.InvariantCulture) + " Right=" + Right.ToString(CultureInfo.InvariantCulture) + 
                " Top=" + Top.ToString(CultureInfo.InvariantCulture) + " Bottom=" + Bottom.ToString(CultureInfo.InvariantCulture) + "]";
        }
    }
}
#endif