
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Globalization;
using System.Runtime.Serialization;

namespace SolidShineUi.Utils
{
    // from https://stackoverflow.com/questions/3372303/dropshadow-for-wpf-borderless-window/6313576#6313576

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
            catch (Exception ex)
            {
                // Probably dwmapi.dll not found (incompatible OS)
                return false;
            }
        }
    }

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

        //
        // Summary:
        //     Gets or sets the left margin width, in hundredths of an inch.
        //
        // Returns:
        //     The left margin width, in hundredths of an inch.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The System.Drawing.Printing.Margins.Left property is set to a value that is less
        //     than 0.
        public int Left
        {
            get
            {
                return left;
            }
            set
            {
                CheckMargin(value, "Left");
                left = value;
                doubleLeft = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the right margin width, in hundredths of an inch.
        //
        // Returns:
        //     The right margin width, in hundredths of an inch.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The System.Drawing.Printing.Margins.Right property is set to a value that is
        //     less than 0.
        public int Right
        {
            get
            {
                return right;
            }
            set
            {
                CheckMargin(value, "Right");
                right = value;
                doubleRight = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the top margin width, in hundredths of an inch.
        //
        // Returns:
        //     The top margin width, in hundredths of an inch.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The System.Drawing.Printing.Margins.Top property is set to a value that is less
        //     than 0.
        public int Top
        {
            get
            {
                return top;
            }
            set
            {
                CheckMargin(value, "Top");
                top = value;
                doubleTop = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the bottom margin, in hundredths of an inch.
        //
        // Returns:
        //     The bottom margin, in hundredths of an inch.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The System.Drawing.Printing.Margins.Bottom property is set to a value that is
        //     less than 0.
        public int Bottom
        {
            get
            {
                return bottom;
            }
            set
            {
                CheckMargin(value, "Bottom");
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

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Printing.Margins class with
        //     1-inch wide margins.
        public Margins()
            : this(100, 100, 100, 100)
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Printing.Margins class with
        //     the specified left, right, top, and bottom margins.
        //
        // Parameters:
        //   left:
        //     The left margin, in hundredths of an inch.
        //
        //   right:
        //     The right margin, in hundredths of an inch.
        //
        //   top:
        //     The top margin, in hundredths of an inch.
        //
        //   bottom:
        //     The bottom margin, in hundredths of an inch.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The left parameter value is less than 0. -or- The right parameter value is less
        //     than 0. -or- The top parameter value is less than 0. -or- The bottom parameter
        //     value is less than 0.
        public Margins(int left, int right, int top, int bottom)
        {
            CheckMargin(left, "left");
            CheckMargin(right, "right");
            CheckMargin(top, "top");
            CheckMargin(bottom, "bottom");
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

        private void CheckMargin(int margin, string name)
        {
            if (margin < 0)
            {
                throw new ArgumentException($"value {name} cannot be below 0; inputted value: {margin}");
            }
        }

        //
        // Summary:
        //     Retrieves a duplicate of this object, member by member.
        //
        // Returns:
        //     A duplicate of this object.
        public object Clone()
        {
            return MemberwiseClone();
        }

        //
        // Summary:
        //     Compares this System.Drawing.Printing.Margins to the specified System.Object
        //     to determine whether they have the same dimensions.
        //
        // Parameters:
        //   obj:
        //     The object to which to compare this System.Drawing.Printing.Margins.
        //
        // Returns:
        //     true if the specified object is a System.Drawing.Printing.Margins and has the
        //     same System.Drawing.Printing.Margins.Top, System.Drawing.Printing.Margins.Bottom,
        //     System.Drawing.Printing.Margins.Right and System.Drawing.Printing.Margins.Left
        //     values as this System.Drawing.Printing.Margins; otherwise, false.
        public override bool Equals(object obj)
        {
            Margins margins = obj as Margins;
            if (margins == this)
            {
                return true;
            }

            if (margins == null)
            {
                return false;
            }

            if (margins.Left == Left && margins.Right == Right && margins.Top == Top)
            {
                return margins.Bottom == Bottom;
            }

            return false;
        }

        //
        // Summary:
        //     Calculates and retrieves a hash code based on the width of the left, right, top,
        //     and bottom margins.
        //
        // Returns:
        //     A hash code based on the left, right, top, and bottom margins.
        public override int GetHashCode()
        {
            uint num = (uint)Left;
            uint num2 = (uint)Right;
            uint num3 = (uint)Top;
            uint num4 = (uint)Bottom;
            return (int)(num ^ ((num2 << 13) | (num2 >> 19)) ^ ((num3 << 26) | (num3 >> 6)) ^ ((num4 << 7) | (num4 >> 25)));
        }

        //
        // Summary:
        //     Compares two System.Drawing.Printing.Margins to determine if they have the same
        //     dimensions.
        //
        // Parameters:
        //   m1:
        //     The first System.Drawing.Printing.Margins to compare for equality.
        //
        //   m2:
        //     The second System.Drawing.Printing.Margins to compare for equality.
        //
        // Returns:
        //     true to indicate the System.Drawing.Printing.Margins.Left, System.Drawing.Printing.Margins.Right,
        //     System.Drawing.Printing.Margins.Top, and System.Drawing.Printing.Margins.Bottom
        //     properties of both margins have the same value; otherwise, false.
        public static bool operator ==(Margins m1, Margins m2)
        {
            if ((object)m1 == null != ((object)m2 == null))
            {
                return false;
            }

            if ((object)m1 != null)
            {
                if (m1.Left == m2.Left && m1.Top == m2.Top && m1.Right == m2.Right)
                {
                    return m1.Bottom == m2.Bottom;
                }

                return false;
            }

            return true;
        }

        //
        // Summary:
        //     Compares two System.Drawing.Printing.Margins to determine whether they are of
        //     unequal width.
        //
        // Parameters:
        //   m1:
        //     The first System.Drawing.Printing.Margins to compare for inequality.
        //
        //   m2:
        //     The second System.Drawing.Printing.Margins to compare for inequality.
        //
        // Returns:
        //     true to indicate if the System.Drawing.Printing.Margins.Left, System.Drawing.Printing.Margins.Right,
        //     System.Drawing.Printing.Margins.Top, or System.Drawing.Printing.Margins.Bottom
        //     properties of both margins are not equal; otherwise, false.
        public static bool operator !=(Margins m1, Margins m2)
        {
            return !(m1 == m2);
        }

        //
        // Summary:
        //     Converts the System.Drawing.Printing.Margins to a string.
        //
        // Returns:
        //     A System.String representation of the System.Drawing.Printing.Margins.
        public override string ToString()
        {
            return "[Margins Left=" + Left.ToString(CultureInfo.InvariantCulture) + " Right=" + Right.ToString(CultureInfo.InvariantCulture) + " Top=" + Top.ToString(CultureInfo.InvariantCulture) + " Bottom=" + Bottom.ToString(CultureInfo.InvariantCulture) + "]";
        }
    }
}
