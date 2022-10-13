using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Interaction logic for GradientStopItem.xaml
    /// </summary>
    public partial class GradientStopItem : UserControl
    {
        /// <summary>
        /// Create a GradientStopItem, with default property values.
        /// </summary>
        public GradientStopItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a GradientStopItem, with preset property values.
        /// </summary>
        /// <param name="offset">The offset from the gradient's start point where this stop appears.</param>
        /// <param name="color">The color of this gradient stop.</param>
        public GradientStopItem(double offset, Color color)
        {
            Offset = offset;
            Color = color;
            InitializeComponent();
        }

        /// <summary>
        /// Create a GradientStopItem, generated from a GradientStop.
        /// </summary>
        /// <param name="stop">The GradientStop to load property values from.</param>
        public GradientStopItem(GradientStop stop)
        {
            Offset = stop.Offset;
            Color = stop.Color;
            InitializeComponent();
        }

        public GradientStop GenerateStop()
        {
            return new GradientStop(Color, Offset);
        }

        public double Offset { get => (double)GetValue(OffsetProperty); set => SetValue(OffsetProperty, value); }

        public static DependencyProperty OffsetProperty
            = DependencyProperty.Register("Offset", typeof(double), typeof(GradientStopItem),
            new FrameworkPropertyMetadata(0.0));

        public Color Color { get => (Color)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

        public static DependencyProperty ColorProperty
            = DependencyProperty.Register("Color", typeof(Color), typeof(GradientStopItem),
            new FrameworkPropertyMetadata(Colors.Black));

        private void pathOutline_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void pathOutline_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void pathOutline_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void pathOutline_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void pathOutline_TouchEnter(object sender, TouchEventArgs e)
        {

        }

        private void pathOutline_TouchLeave(object sender, TouchEventArgs e)
        {

        }

        private void pathOutline_TouchDown(object sender, TouchEventArgs e)
        {

        }

        private void pathOutline_TouchUp(object sender, TouchEventArgs e)
        {

        }
    }
}
