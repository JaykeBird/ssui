using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A control used to display a <see cref="System.Windows.Media.GradientStop"/> within a <see cref="GradientBar"/> control.
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

        GradientStop _stop = new GradientStop(Colors.Black, 0.0);

        /// <summary>
        /// Create a GradientStopItem, with preset property values.
        /// </summary>
        /// <param name="offset">The offset from the gradient's start point where this stop appears.</param>
        /// <param name="color">The color of this gradient stop.</param>
        public GradientStopItem(double offset, Color color)
        {
            Offset = offset;
            Color = color;
            _stop = new GradientStop(Color, Offset);
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
            _stop = stop;
            InitializeComponent();
        }

        /// <summary>
        /// Get or set the gradient stop for this GradientStopItem.
        /// </summary>
        public GradientStop GradientStop
        {
            get
            {
                if (_stop.Color != Color) _stop.Color = Color;
                if (_stop.Offset != Offset) _stop.Offset = Offset;

                return _stop;
            }
            set
            {
                Offset = value.Offset;
                Color = value.Color;
                _stop = value;
            }
        }

        //private void Setup()
        //{
        //    //Click += control_Click;
        //}

        /// <summary>
        /// Get or set the offset value for this gradient stop. The offset determines how far along the gradient this stop occurs.
        /// </summary>
        public double Offset { get => (double)GetValue(OffsetProperty); set => SetValue(OffsetProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty
            = DependencyProperty.Register("Offset", typeof(double), typeof(GradientStopItem),
            new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Get or set the color for this gradient stop.
        /// </summary>
        public Color Color { get => (Color)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ColorProperty
            = DependencyProperty.Register("Color", typeof(Color), typeof(GradientStopItem),
            new FrameworkPropertyMetadata(Colors.Black));

        /// <summary>
        /// Get or set whether this gradient stop is selected for editing.
        /// </summary>
        public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty
            = DependencyProperty.Register("IsSelected", typeof(bool), typeof(GradientStopItem),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set the brush used for the background of the gradient stop glyph.
        /// </summary>
        [Category("Brushes")]
        public Brush StopFill
        {
            get => (Brush)GetValue(StopFillProperty);
            set => SetValue(StopFillProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the stop glyph while it is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public Brush StopSelectedFill
        {
            get => (Brush)GetValue(StopSelectedFillProperty);
            set => SetValue(StopSelectedFillProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="StopFill"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty StopFillProperty = DependencyProperty.Register(
            nameof(StopFill), typeof(Brush), typeof(GradientStopItem),
            new PropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// The backing dependency property for <see cref="StopSelectedFill"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty StopSelectedFillProperty = DependencyProperty.Register(
            nameof(StopSelectedFill), typeof(Brush), typeof(GradientStopItem),
            new PropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the border around the edge of the control.
        /// </summary>
        [Category("Brushes")]
        public Brush StopBorderBrush { get => (Brush)GetValue(StopBorderBrushProperty); set => SetValue(StopBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="StopBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty StopBorderBrushProperty
            = DependencyProperty.Register(nameof(StopBorderBrush), typeof(Brush), typeof(GradientStopItem),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the border, while the control is highlighted (focus, mouse-over, etc.).
        /// </summary>
        [Category("Brushes")]
        public Brush StopBorderHighlightBrush { get => (Brush)GetValue(StopBorderHighlightBrushProperty); set => SetValue(StopBorderHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="StopBorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty StopBorderHighlightBrushProperty
            = DependencyProperty.Register(nameof(StopBorderHighlightBrush), typeof(Brush), typeof(GradientStopItem),
            new FrameworkPropertyMetadata(Colors.DimGray.ToBrush()));


        private void Highlight()
        {
            pathOutline.Stroke = Colors.DimGray.ToBrush();
        }

        private void Unhighlight()
        {
            pathOutline.Stroke = Colors.Black.ToBrush();
        }

        private void pathOutline_MouseEnter(object sender, MouseEventArgs e)
        {
            Highlight();
        }

        private void pathOutline_MouseLeave(object sender, MouseEventArgs e)
        {
            Unhighlight();
        }

        private void pathOutline_TouchEnter(object sender, TouchEventArgs e)
        {
            Highlight();
        }

        private void pathOutline_TouchLeave(object sender, TouchEventArgs e)
        {
            Unhighlight();
        }

        private void pathOutline_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            brdrFocus.Visibility = Visibility.Visible;
        }

        private void pathOutline_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            brdrFocus.Visibility = Visibility.Collapsed;
        }

        #region Click Handling

        #region Routed Events

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GradientStopItem));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Raised when the check box is clicked.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            "RightClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GradientStopItem));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Raised when the check box is right-clicked.
        /// </summary>
        public event RoutedEventHandler RightClick
        {
            add { AddHandler(RightClickEvent, value); }
            remove { RemoveHandler(RightClickEvent, value); }
        }

        #endregion

        #region Variables/Properties
        bool initiatingClick = false;
        /// <summary>
        /// Gets or sets whether the Click event should be raised when the checkbox is pressed, rather than when it is released.
        /// </summary>
        public bool ClickOnPress { get; set; } = false;

        #endregion

        /// <summary>
        /// Sets up the button to be clicked. This must be run before PerformClick.
        /// </summary>
        /// <param name="rightClick">Determine whether this should be treated as a right click (which usually invokes a context menu).</param>
        void PerformPress(bool rightClick = false)
        {
            initiatingClick = true;

            if (ClickOnPress)
            {
                PerformClick(rightClick);
            }
        }

        /// <summary>
        /// If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        /// </summary>
        /// <param name="rightClick">Determine whether this should be treated as a right click (which usually invokes a context menu).</param>
        void PerformClick(bool rightClick = false)
        {
            if (initiatingClick)
            {
                if (rightClick)
                {
                    RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
                    RaiseEvent(rre);
                    initiatingClick = false;
                    return;
                }

                RoutedEventArgs re = new RoutedEventArgs(ClickEvent);
                RaiseEvent(re);
                initiatingClick = false;
            }
        }

        /// <summary>
        /// Perform a click programattically. The checkbox responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            PerformPress();
            PerformClick();
        }

        private void pathOutline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PerformPress(e.ChangedButton == MouseButton.Right);
        }

        private void pathOutline_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PerformClick(e.ChangedButton == MouseButton.Right);
            e.Handled = true;
        }

#if NETCOREAPP
        private void pathOutline_TouchDown(object? sender, TouchEventArgs e)
        {
            PerformPress();
        }

        private void pathOutline_TouchUp(object? sender, TouchEventArgs e)
        {
            PerformClick();
        }

#else
        private void pathOutline_TouchDown(object sender, TouchEventArgs e)
        {
            PerformPress();
        }

        private void pathOutline_TouchUp(object sender, TouchEventArgs e)
        {
            PerformClick();
        }
#endif

        private void pathOutline_StylusDown(object sender, StylusDownEventArgs e)
        {
            PerformPress();
        }

        private void pathOutline_StylusUp(object sender, StylusEventArgs e)
        {
            PerformClick();
        }

        private void pathOutline_StylusEnter(object sender, StylusEventArgs e)
        {
            Highlight();
        }

        private void pathOutline_StylusLeave(object sender, StylusEventArgs e)
        {
            Unhighlight();
        }

        private void pathOutline_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                PerformPress();
            }
        }

        private void pathOutline_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                PerformClick();
            }
            else if (e.Key == Key.Apps)
            {
                PerformClick(true);
            }
        }
        #endregion
    }
}
