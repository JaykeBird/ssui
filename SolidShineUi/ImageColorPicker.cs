﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SolidShineUi.NativeMethods;

namespace SolidShineUi
{
    /// <summary>
    /// Image element with the ability to pick out a pixel color value.
    /// </summary>
    public class ImageColorPicker : Image
    {
        public ImageColorPicker()
        {
            MouseDown += ImageColorPicker_MouseDown;

            MouseUp += ImageColorPicker_MouseUp;
            MouseMove += ImageColorPicker_MouseMove;
            MouseLeave += ImageColorPicker_MouseLeave;
        }

//#if NETCOREAPP
//        public event EventHandler? SelectedColorChanged;
//#else
//        public event EventHandler SelectedColorChanged;
//#endif

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageColorPicker));

        public event RoutedEventHandler SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }

        /// <summary>
        /// Get the color that is selected from the image.
        /// </summary>
        public Color? SelectedColor { get; private set; } = null;

        /// <summary>
        /// Get the position that is selected from the image. This position is relative to the control itself.
        /// </summary>
        public Point? SelectedPosition { get; private set; } = null;

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

                SelectedColor = GetPixelPointColor(p);
                SelectedPosition = s;

                RoutedEventArgs re = new RoutedEventArgs(SelectedColorChangedEvent);
                RaiseEvent(re);
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

                    SelectedColor = GetPixelPointColor(p);
                    SelectedPosition = s;

                    RoutedEventArgs re = new RoutedEventArgs(SelectedColorChangedEvent);
                    RaiseEvent(re);
                }
            }
        }

        private void ImageColorPicker_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ReleaseDC(IntPtr.Zero, dc);
            dc = IntPtr.Zero;
            dc_present = false;
        }

        private void ImageColorPicker_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ReleaseDC(IntPtr.Zero, dc);
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
    }
}
