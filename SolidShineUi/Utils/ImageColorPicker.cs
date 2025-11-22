using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SolidShineUi.NativeMethods;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// An Image control where users can select a color by clicking on pixels of the image.
    /// </summary>
    /// <remarks>
    /// Note that if this is used in a WPF project that isn't DPI-Aware, this control will perform weirdly on screens with a DPI value other than 100%.
    /// </remarks>
    public class ImageColorPicker : Image
    {

        /// <summary>
        /// Create a ImageColorPicker.
        /// </summary>
        public ImageColorPicker()
        {
            MouseDown += ImageColorPicker_MouseDown;
            MouseUp += ImageColorPicker_MouseUp;
            MouseMove += ImageColorPicker_MouseMove;
            MouseLeave += ImageColorPicker_MouseLeave;

            StylusDown += imageColorPicker_StylusDown;
            StylusUp += imageColorPicker_StylusUp;
            StylusMove += imageColorPicker_StylusMove;
            StylusLeave += imageColorPicker_StylusLeave;
        }

        #region SelectedColor

        /// <summary>The backing routed event for <see cref="SelectedColorChanged"/>. See the related event for more details.</summary>
        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageColorPicker));

        /// <summary>
        /// Raised when the SelectedColor property is changed.
        /// </summary>
        public event RoutedEventHandler SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }

        /// <summary>
        /// Get the color that is selected from the image.
        /// </summary>
        public Color? SelectedColor { get => (Color?)GetValue(SelectedColorProperty); private set => SetValue(SelectedColorPropertyKey, value); }

        private static readonly DependencyPropertyKey SelectedColorPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SelectedColor), typeof(Color?), typeof(ImageColorPicker),
            new FrameworkPropertyMetadata(null, OnSelectedColorChanged));

        /// <summary>The backing dependency property for <see cref="SelectedColor"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedColorProperty = SelectedColorPropertyKey.DependencyProperty;

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageColorPicker o)
            {
                o.RaiseSelectedColorChangedEvent();
            }
        }

        private void RaiseSelectedColorChangedEvent()
        {
            RoutedEventArgs re = new RoutedEventArgs(SelectedColorChangedEvent);
            RaiseEvent(re);
        }

        #endregion

        #region SelectedPosition

        Point? selPoint = null;

        /// <summary>
        /// Get or set the position that is selected from the image. This position is relative to the control itself.
        /// </summary>
        public Point? SelectedPosition
        {
            get => selPoint;
            set
            {
                selPoint = value;
                if (selPoint != null)
                {
                    Point s = selPoint.Value;
                    Point p = PointToScreen(s);

                    if (!dc_present)
                    {
                        dc = GetDC(IntPtr.Zero);
                        dc_present = true;
                    }

                    SelectedColor = GetPixelPointColor(p);
                }
                else
                {
                    SelectedColor = null;
                }
            }
        }

        #endregion

        #region DisplayCanvas / Event Handlers

        bool dc_present = false;
        IntPtr dc = IntPtr.Zero;

        private void ImageColorPicker_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Source != null && e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Point s = e.GetPosition(this);
                Point p = PointToScreen(s);

                if (!dc_present)
                {
                    dc = GetDC(IntPtr.Zero);
                    dc_present = true;
                }

                selPoint = s;
                SelectedColor = GetPixelPointColor(p);
            }
        }

        private void ImageColorPicker_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (dc_present)
                {
                    Point s = e.GetPosition(this);
                    Point p = PointToScreen(s);

                    selPoint = s;
                    SelectedColor = GetPixelPointColor(p);
                }
            }
        }

        private void ImageColorPicker_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var _ = ReleaseDC(IntPtr.Zero, dc);
            dc = IntPtr.Zero;
            dc_present = false;
        }

        private void ImageColorPicker_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var _ = ReleaseDC(IntPtr.Zero, dc);
            dc = IntPtr.Zero;
            dc_present = false;
        }

        private void imageColorPicker_StylusDown(object sender, System.Windows.Input.StylusDownEventArgs e)
        {
            if (Source != null)
            {
                Point s = e.GetPosition(this);
                Point p = PointToScreen(s);

                if (!dc_present)
                {
                    dc = GetDC(IntPtr.Zero);
                    dc_present = true;
                }

                selPoint = s;
                SelectedColor = GetPixelPointColor(p);
            }
        }

        private void imageColorPicker_StylusMove(object sender, System.Windows.Input.StylusEventArgs e)
        {
            if (dc_present)
            {
                Point s = e.GetPosition(this);
                Point p = PointToScreen(s);

                selPoint = s;
                SelectedColor = GetPixelPointColor(p);
            }
        }

        private void imageColorPicker_StylusLeave(object sender, System.Windows.Input.StylusEventArgs e)
        {
            int _ = ReleaseDC(IntPtr.Zero, dc);
            dc = IntPtr.Zero;
            dc_present = false;
        }

        private void imageColorPicker_StylusUp(object sender, System.Windows.Input.StylusEventArgs e)
        {
            int _ = ReleaseDC(IntPtr.Zero, dc);
            dc = IntPtr.Zero;
            dc_present = false;
        }

        private Color? GetPixelPointColor(Point p)
        {
            // from https://stackoverflow.com/questions/753132/how-to-get-the-colour-of-a-pixel-at-x-y-using-c
            if (dc_present)
            {
                uint pixel = GetPixel(dc, (int)p.X, (int)p.Y);
                Color color = Color.FromRgb((byte)(pixel & 0x000000FF),
                                            (byte)((pixel & 0x0000FF00) >> 8),
                                            (byte)((pixel & 0x00FF0000) >> 16));
                return color;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
