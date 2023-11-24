using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A control to visually select two values between 0.0 and 1.0 in both the X (width) and Y (height) axes.
    /// </summary>
    public partial class TwoPositionsSelect : UserControl
    {

        /// <summary>
        /// Create a TwoPositionsSelect.
        /// </summary>
        public TwoPositionsSelect()
        {
            InitializeComponent();

            HorizontalSnapPoints.CollectionChanged += HorizontalSnapPoints_CollectionChanged;
            VerticalSnapPoints.CollectionChanged += VerticalSnapPoints_CollectionChanged;
            SizeChanged += TwoPositionsSelect_SizeChanged;
            GotKeyboardFocus += TwoPositionsSelect_GotKeyboardFocus;
            LostKeyboardFocus += TwoPositionsSelect_LostKeyboardFocus;
            PreviewKeyDown += TwoPositionsSelect_PreviewKeyDown;
            PreviewKeyUp += TwoPositionsSelect_PreviewKeyUp;
            KeyDown += TwoPositionsSelect_KeyDown;
            KeyUp += TwoPositionsSelect_KeyUp;
            IsEnabledChanged += TwoPositionsSelect_IsEnabledChanged;

            //KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Continue);
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Contained);
        }

        private void TwoPositionsSelect_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // might consider hiding snap lines while control is disabled
        }

        #region Color Scheme

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(TwoPositionsSelect),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is TwoPositionsSelect r)
            {
                r.ColorSchemeChanged?.Invoke(d, e);
                r.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this TwoPositionsSelect. For easier color scheme management, bind this to the window or larger control you're using.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs == null)
            {
                return;
            }
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            if (cs.IsHighContrast)
            {
                ControlBackground = cs.BackgroundColor.ToBrush();
                BackgroundDisabledBrush = cs.BackgroundColor.ToBrush();
                SelectorBrush = cs.ForegroundColor.ToBrush();
                SnapLineBrush = cs.BorderColor.ToBrush();
                KeyboardFocusHighlight = cs.SecondHighlightColor.ToBrush();
            }
            else
            {
                ControlBackground = cs.LightBackgroundColor.ToBrush();
                BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
                SelectorBrush = cs.HighlightColor.ToBrush();
                SnapLineBrush = cs.SecondaryColor.ToBrush();
                KeyboardFocusHighlight = cs.ThirdHighlightColor.ToBrush();
            }

            BorderBrush = cs.BorderColor.ToBrush();
            BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
            BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
            SelectorDisabledBrush = cs.DarkDisabledColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
        }
        #endregion

        #region Brushes / Brush Handling

        #region Brush properties
        /// <summary>
        /// Get or set the brush used for the background of the TwoPositionsSelect's box.
        /// </summary>
        [Category("Brushes")]
        public Brush ControlBackground
        {
            get => (Brush)GetValue(ControlBackgroundProperty);
            set => SetValue(ControlBackgroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the selector ellipses in the TwoPositionsSelect.
        /// </summary>
        [Category("Brushes")]
        public Brush SelectorBrush
        {
            get => (Brush)GetValue(SelectorBrushProperty);
            set => SetValue(SelectorBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the background of the TwoPositionsSelect's box when it is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BackgroundDisabledBrush
        {
            get => (Brush)GetValue(BackgroundDisabledBrushProperty);
            set => SetValue(BackgroundDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the border of the cotnrol, while the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get => (Brush)GetValue(BorderDisabledBrushProperty);
            set => SetValue(BorderDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the check mark when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush SelectorDisabledBrush
        {
            get => (Brush)GetValue(SelectorDisabledBrushProperty);
            set => SetValue(SelectorDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border of the TwoPositionsSelect's box.
        /// </summary>
        [Category("Brushes")]
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the snap point lines.
        /// </summary>
        [Category("Brushes")]
        public Brush SnapLineBrush
        {
            get => (Brush)GetValue(SnapLineBrushProperty);
            set => SetValue(SnapLineBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the outline when the control is keyboard focused.
        /// </summary>
        [Category("Brushes")]
        public Brush KeyboardFocusHighlight
        {
            get => (Brush)GetValue(KeyboardFocusHighlightProperty);
            set => SetValue(KeyboardFocusHighlightProperty, value);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ControlBackgroundProperty = DependencyProperty.Register(
            "ControlBackground", typeof(Brush), typeof(TwoPositionsSelect),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        public static readonly DependencyProperty SelectorBrushProperty = DependencyProperty.Register(
            "SelectorBrush", typeof(Brush), typeof(TwoPositionsSelect),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        public static readonly DependencyProperty BackgroundDisabledBrushProperty = DependencyProperty.Register(
            "BackgroundDisabledBrush", typeof(Brush), typeof(TwoPositionsSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(TwoPositionsSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty SelectorDisabledBrushProperty = DependencyProperty.Register(
            "SelectorDisabledBrush", typeof(Brush), typeof(TwoPositionsSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(TwoPositionsSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty SnapLineBrushProperty = DependencyProperty.Register(
            "SnapLineBrush", typeof(Brush), typeof(TwoPositionsSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray), OnSnapLineBrushChanged));

        public static readonly DependencyProperty KeyboardFocusHighlightProperty = DependencyProperty.Register(
            "KeyboardFocusHighlight", typeof(Brush), typeof(TwoPositionsSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray), OnKeyboardFocusHighlightBrushChanged));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion

        /// <summary>
        /// Perform an action when a property of an object has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnSnapLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TwoPositionsSelect r)
            {
                r.UpdateSnapLineBrush();
            }
        }

        /// <summary>
        /// Updates the visuals of the snap lines in the control to match the SnapLineBrush property.
        /// </summary>
        internal protected void UpdateSnapLineBrush()
        {
#if NETCOREAPP
            foreach (UIElement? item in canVertical.Children)
#else
            foreach (UIElement item in canVertical.Children)
#endif
            {
                if (item != null && item is Border b)
                {
                    b.BorderBrush = SnapLineBrush;
                }
            }
#if NETCOREAPP
            foreach (UIElement? item in canHorizontal.Children)
#else
            foreach (UIElement item in canHorizontal.Children)
#endif
            {
                if (item != null && item is Border b)
                {
                    b.BorderBrush = SnapLineBrush;
                }
            }
        }

        /// <summary>
        /// Perform an action when a property of an object has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnKeyboardFocusHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TwoPositionsSelect r)
            {
                r.UpdateKeyboardFocusHighlightBrush();
            }
        }

        /// <summary>
        /// An internal method to use for updating the brush when this control has keyboard focus.
        /// </summary>
        internal protected void UpdateKeyboardFocusHighlightBrush()
        {
            if (IsKeyboardFocused || HasEffectiveKeyboardFocus)
            {
                brdrKeyFocus.BorderBrush = KeyboardFocusHighlight;
            }
        }

        #endregion

        #region Snap Points

        /// <summary>
        /// Get or set if the selector should snap to the snap lines within the control.
        /// </summary>
        public bool SnapToSnapLines { get => (bool)GetValue(SnapToSnapLinesProperty); set => SetValue(SnapToSnapLinesProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static DependencyProperty SnapToSnapLinesProperty
            = DependencyProperty.Register("SnapToSnapLines", typeof(bool), typeof(TwoPositionsSelect),
            new FrameworkPropertyMetadata(true));


        /// <summary>
        /// The distance, in pixels, within which the selector should snap to the nearest snap line.
        /// The larger the distance, the further the selector can be away from a snap line before it snaps to the line.
        /// </summary>
        public double SnapDistance { get => (double)GetValue(SnapDistanceProperty); set => SetValue(SnapDistanceProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static DependencyProperty SnapDistanceProperty
            = DependencyProperty.Register("SnapDistance", typeof(double), typeof(TwoPositionsSelect),
            new FrameworkPropertyMetadata(3.0));


        #region ObservableCollection handling
        /// <summary>
        /// Get or set the list of snap points that are displayed along the horizontal (X) axis of the control.
        /// <c>0.0</c> represents the far left of the control, and <c>1.0</c> represents the far right of the control.
        /// </summary>
        public ObservableCollection<double> HorizontalSnapPoints { get; set; } = new ObservableCollection<double>();

        /// <summary>
        /// Get or set the list of snap points that are displayed along the vertical (Y) axis of the control.
        /// <c>0.0</c> represents the far top of the control, and <c>1.0</c> represents the far bottom of the control.
        /// </summary>
        public ObservableCollection<double> VerticalSnapPoints { get; set; } = new ObservableCollection<double>();

#if NETCOREAPP
        private void VerticalSnapPoints_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        private void VerticalSnapPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#endif
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (double? item in e.NewItems)
                        {
                            if (!item.HasValue) continue;
                            AddVerticalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (double? item in e.OldItems)
                        {
                            if (!item.HasValue) continue;
                            RemoveVerticalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // add one, remove one
                    if (e.NewItems != null)
                    {
                        foreach (double? item in e.NewItems)
                        {
                            if (!item.HasValue) continue;
                            AddVerticalSnapPoint(item.Value);
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (double? item in e.OldItems)
                        {
                            if (!item.HasValue) continue;
                            RemoveVerticalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    // requires nothing
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // clear all
                    canVertical.Children.Clear();
                    break;
                default:
                    // don't know? do nothing
                    break;
            }
        }

#if NETCOREAPP
        private void HorizontalSnapPoints_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        private void HorizontalSnapPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#endif
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (double? item in e.NewItems)
                        {
                            if (!item.HasValue) continue;
                            AddHorizontalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (double? item in e.OldItems)
                        {
                            if (!item.HasValue) continue;
                            RemoveHorizontalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                    {
                        foreach (double? item in e.NewItems)
                        {
                            if (!item.HasValue) continue;
                            AddHorizontalSnapPoint(item.Value);
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (double? item in e.OldItems)
                        {
                            if (!item.HasValue) continue;
                            RemoveHorizontalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    // requires nothing
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // clear all
                    canHorizontal.Children.Clear();
                    break;
                default:
                    // don't know? do nothing
                    break;
            }
        }
        #endregion

        #region Internal Add/Remove methods

        private void AddVerticalSnapPoint(double point)
        {
            //double sshalf = SelectorSize / 2;
            Border b = new Border();
            b.BorderThickness = new Thickness(0.75);
            b.BorderBrush = SnapLineBrush;
            b.Width = 1;
            b.Tag = point;
            b.HorizontalAlignment = HorizontalAlignment.Left;
            b.VerticalAlignment = VerticalAlignment.Stretch;
            b.Margin = new Thickness(grdSelArea.ActualWidth * point, 0, 0, 0);
            //b.Margin = new Thickness(grdSelArea.ActualWidth * point + sshalf, 0, 0, 0);
            b.SnapsToDevicePixels = true;
            b.UseLayoutRounding = false;

            canVertical.Children.Add(b);
        }

        private void RemoveVerticalSnapPoint(double point)
        {
            List<UIElement> toRemove = new List<UIElement>();

#if NETCOREAPP
            foreach (UIElement? item in canVertical.Children)
#else
            foreach (UIElement item in canVertical.Children)
#endif
            {
                if (item != null && item is Border b)
                {
                    if (b.Tag is double d)
                    {
                        if (d == point)
                        {
                            toRemove.Add(b);
                        }
                    }
                }
            }

            foreach (UIElement item in toRemove)
            {
                canVertical.Children.Remove(item);
            }
        }

        private void AddHorizontalSnapPoint(double point)
        {
            //double sshalf = SelectorSize / 2;
            Border b = new Border();
            b.BorderThickness = new Thickness(0.75);
            b.BorderBrush = SnapLineBrush;
            b.Height = 1;
            b.Tag = point;
            b.HorizontalAlignment = HorizontalAlignment.Stretch;
            b.VerticalAlignment = VerticalAlignment.Top;
            b.Margin = new Thickness(0, grdSelArea.ActualHeight * point, 0, 0);
            //b.Margin = new Thickness(0, grdSelArea.ActualHeight * point + sshalf, 0, 0);
            b.SnapsToDevicePixels = true;
            b.UseLayoutRounding = false;

            canHorizontal.Children.Add(b);
        }

        private void RemoveHorizontalSnapPoint(double point)
        {
            List<UIElement> toRemove = new List<UIElement>();

#if NETCOREAPP
            foreach (UIElement? item in canHorizontal.Children)
#else
            foreach (UIElement item in canHorizontal.Children)
#endif
            {
                if (item != null && item is Border b)
                {
                    if (b.Tag is double d)
                    {
                        if (d == point)
                        {
                            toRemove.Add(b);
                        }
                    }
                }
            }

            foreach (UIElement item in toRemove)
            {
                canHorizontal.Children.Remove(item);
            }
        }
        #endregion

        private void TwoPositionsSelect_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double sshalf = SelectorSize / 2;

            if (e.WidthChanged)
            {
#if NETCOREAPP
                foreach (UIElement? item in canVertical.Children)
#else
                foreach (UIElement item in canVertical.Children)
#endif
                {
                    if (item != null && item is Border b)
                    {
                        if (b.Tag is double d)
                        {
                            b.Margin = new Thickness(grdSelArea.ActualWidth * d, 0, 0, 0);
                            //b.Margin = new Thickness(grdSelArea.ActualWidth * d + sshalf, 0, 0, 0);
                        }
                    }
                }
                Canvas.SetLeft(ellSelect, (grdSelArea.ActualWidth * SelectedWidth1) - sshalf);
                Canvas.SetLeft(ellSelect2, (grdSelArea.ActualWidth * SelectedWidth2) - sshalf);
            }

            if (e.HeightChanged)
            {
#if NETCOREAPP
                foreach (UIElement? item in canHorizontal.Children)
#else
                foreach (UIElement item in canHorizontal.Children)
#endif
                {
                    if (item != null && item is Border b)
                    {
                        if (b.Tag is double d)
                        {
                            b.Margin = new Thickness(0, grdSelArea.ActualHeight * d, 0, 0);
                            //b.Margin = new Thickness(0, grdSelArea.ActualHeight * d + sshalf, 0, 0);
                        }
                    }
                }
                Canvas.SetTop(ellSelect, (grdSelArea.ActualHeight * SelectedHeight1) - sshalf);
                Canvas.SetTop(ellSelect2, (grdSelArea.ActualHeight * SelectedHeight2) - sshalf);
            }
        }

        #endregion

        #region Selector Handling (focus setting / visuals)
        static bool? SELECTOR_NONE = null;
        static bool? SELECTOR_1 = false;
        static bool? SELECTOR_2 = true;

        /// <summary>
        /// Set which selector is receiving key and mouse focus. <c>false</c> for selector 1, <c>true</c> for selector 2, and <c>null</c> for neither.
        /// </summary>
        /// <param name="focusValue">Value indicating which selector should have focus: <c>false</c> for selector 1, <c>true</c> for selector 2, and <c>null</c> for neither</param>
        void SetSelectorFocus(bool? focusValue)
        {
            selectorFocus = focusValue;
            if (focusValue == null)
            {
                // no focus
                ellSelect.Opacity = 1.0;
                ellSelect2.Opacity = 1.0;
            }
            else if (focusValue.Value == true)
            {
                // selector 2
                ellSelect.Opacity = 0.6;
                ellSelect2.Opacity = 1.0;
            }
            else
            {
                // selector 1
                ellSelect.Opacity = 1.0;
                ellSelect2.Opacity = 0.6;
            }
        }

        bool? selectorFocus = null;

        /// <summary>
        /// Get or set the size of the selector. The larger the selector, the easier it will be to see and also to click and drag, but also harder to visualize a particular value.
        /// </summary>
        public double SelectorSize
        {
            get { return ellSelect.Width; }
            set
            {
                ellSelect.Width = value;
                ellSelect.Height = value;
                ellSelect2.Width = value;
                ellSelect2.Height = value;

                double sshalf = value / 2;
                grdSelArea.Margin = new Thickness(sshalf);
                brdrKeyFocus.BorderThickness = new Thickness(sshalf);
                grdGuidelines.Margin = new Thickness(sshalf);
            }
        }
        #endregion  

        #region Keyboard Controls

        /// <summary>
        /// Get or set the amount the selector is moved each time an arrow key is pressed (while the control is focused).
        /// </summary>
        public double KeyMoveStep { get => (double)GetValue(KeyMoveStepProperty); set => SetValue(KeyMoveStepProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static DependencyProperty KeyMoveStepProperty
            = DependencyProperty.Register("KeyMoveStep", typeof(double), typeof(TwoPositionsSelect),
            new FrameworkPropertyMetadata(0.05));


        private void TwoPositionsSelect_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            brdrKeyFocus.BorderBrush = KeyboardFocusHighlight;
            SetSelectorFocus(SELECTOR_1);
        }

        private void TwoPositionsSelect_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            brdrKeyFocus.BorderBrush = Colors.Transparent.ToBrush();
            SetSelectorFocus(SELECTOR_NONE);
        }

        private void TwoPositionsSelect_KeyUp(object sender, KeyEventArgs e)
        {
            if ((selectorFocus ?? false) == SELECTOR_2)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        SelectedWidth2 -= KeyMoveStep;
                        break;
                    case Key.Right:
                        SelectedWidth2 += KeyMoveStep;
                        break;
                    case Key.Up:
                        SelectedHeight2 -= KeyMoveStep;
                        break;
                    case Key.Down:
                        SelectedHeight2 += KeyMoveStep;
                        break;
                    case Key.Home:
                        SelectedWidth2 = 0;
                        break;
                    case Key.End:
                        SelectedWidth2 = 1;
                        break;
                    case Key.PageUp:
                        SelectedHeight2 = 0;
                        break;
                    case Key.PageDown:
                        SelectedHeight2 = 1;
                        break;
                    case Key.LeftShift:
                    case Key.RightShift:
                        SetSelectorFocus(SELECTOR_1);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Left:
                        SelectedWidth1 -= KeyMoveStep;
                        break;
                    case Key.Right:
                        SelectedWidth1 += KeyMoveStep;
                        break;
                    case Key.Up:
                        SelectedHeight1 -= KeyMoveStep;
                        break;
                    case Key.Down:
                        SelectedHeight1 += KeyMoveStep;
                        break;
                    case Key.Home:
                        SelectedWidth1 = 0;
                        break;
                    case Key.End:
                        SelectedWidth1 = 1;
                        break;
                    case Key.PageUp:
                        SelectedHeight1 = 0;
                        break;
                    case Key.PageDown:
                        SelectedHeight1 = 1;
                        break;
                    case Key.LeftShift:
                    case Key.RightShift:
                        SetSelectorFocus(SELECTOR_2);
                        break;
                    default:
                        break;
                }
            }
        }

        private void TwoPositionsSelect_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void TwoPositionsSelect_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                case Key.Home:
                case Key.End:
                case Key.PageUp:
                case Key.PageDown:
                case Key.RightShift:
                case Key.LeftShift:
                    if (e.OriginalSource == this)
                    {
                        //e.Handled = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void TwoPositionsSelect_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                case Key.Home:
                case Key.End:
                case Key.PageUp:
                case Key.PageDown:
                case Key.RightShift:
                case Key.LeftShift:
                    if (e.OriginalSource == this)
                    {
                        e.Handled = true;
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Mouse Events
        bool selectMode = false;

        private void grdGuidelines_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //selectMode = true;

            //// get point from mouse
            //Point p = Mouse.GetPosition(grdSelArea);
            //SelectPointSelector1(p);
        }

        private void grdGuidelines_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectMode = false;
            if (!IsKeyboardFocusWithin)
            {
                SetSelectorFocus(SELECTOR_NONE);
            }
        }

        private void grdGuidelines_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectMode)
            {
                // get point from mouse
                Point p = Mouse.GetPosition(grdSelArea);
                if (selectorFocus == SELECTOR_1)
                {
                    SelectPointSelector1(p);
                }
                else
                {
                    SelectPointSelector2(p);
                }
            }
        }

        private void grdGuidelines_MouseLeave(object sender, MouseEventArgs e)
        {
            selectMode = false;
            if (!IsKeyboardFocusWithin)
            {
                SetSelectorFocus(SELECTOR_NONE);
            }
        }

        private void ellSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selectMode = true;
            SetSelectorFocus(SELECTOR_1);

            // get point from mouse
            Point p = Mouse.GetPosition(grdSelArea);
            SelectPointSelector1(p);
        }

        private void ellSelect2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selectMode = true;
            SetSelectorFocus(SELECTOR_2);

            // get point from mouse
            Point p = Mouse.GetPosition(grdSelArea);
            SelectPointSelector2(p);
        }
        #endregion

        #region Selector Positions
        private void SelectPointSelector1(Point p)
        {
            // make sure point is in bounds
            if (p.X < 0)
            {
                p.X = 0;
            }
            else if (p.X > grdSelArea.ActualWidth)
            {
                p.X = grdSelArea.ActualWidth;
            }

            if (p.Y < 0)
            {
                p.Y = 0;
            }
            else if (p.Y > grdSelArea.ActualHeight)
            {
                p.Y = grdSelArea.ActualHeight;
            }

            double sshalf = SelectorSize / 2;

            // check for snap positions
            if (SnapToSnapLines)
            {
                double widthMin = p.X - SnapDistance;
                double widthMax = p.X + SnapDistance;
                double heightMin = p.Y - SnapDistance;
                double heightMax = p.Y + SnapDistance;

                // check vertical snap points
#if NETCOREAPP
                foreach (UIElement? item in canVertical.Children)
#else
                foreach (UIElement item in canVertical.Children)
#endif
                {
                    if (item != null && item is Border b)
                    {
                        if (b.Margin.Left > widthMin && b.Margin.Left < widthMax)
                        {
                            p.X = b.Margin.Left;
                            //p.X = b.Margin.Left - sshalf;
                        }
                    }
                }

                // check horizontal snap points
#if NETCOREAPP
                foreach (UIElement? item in canHorizontal.Children)
#else
                foreach (UIElement item in canHorizontal.Children)
#endif
                {
                    if (item != null && item is Border b)
                    {
                        if (b.Margin.Top > heightMin && b.Margin.Top < heightMax)
                        {
                            p.Y = b.Margin.Top;
                            //p.Y = b.Margin.Top - sshalf;
                        }
                    }
                }
            }

            // move selector to this new point
            //ellSelect.Margin = new Thickness(p.X - sshalf, p.Y - sshalf, 0, 0);
            Canvas.SetLeft(ellSelect, p.X - sshalf);
            Canvas.SetTop(ellSelect, p.Y - sshalf);

            // update outputs
            oWidth = p.X / grdSelArea.ActualWidth;
            oHeight = p.Y / grdSelArea.ActualHeight;
            SelectedPositionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SelectPointSelector2(Point p)
        {
            // make sure point is in bounds
            if (p.X < 0)
            {
                p.X = 0;
            }
            else if (p.X > grdSelArea.ActualWidth)
            {
                p.X = grdSelArea.ActualWidth;
            }

            if (p.Y < 0)
            {
                p.Y = 0;
            }
            else if (p.Y > grdSelArea.ActualHeight)
            {
                p.Y = grdSelArea.ActualHeight;
            }

            double sshalf = SelectorSize / 2;

            // check for snap positions
            if (SnapToSnapLines)
            {
                double widthMin = p.X - SnapDistance;
                double widthMax = p.X + SnapDistance;
                double heightMin = p.Y - SnapDistance;
                double heightMax = p.Y + SnapDistance;

                // check vertical snap points
#if NETCOREAPP
                foreach (UIElement? item in canVertical.Children)
#else
                foreach (UIElement item in canVertical.Children)
#endif
                {
                    if (item != null && item is Border b)
                    {
                        if (b.Margin.Left > widthMin && b.Margin.Left < widthMax)
                        {
                            p.X = b.Margin.Left;
                            //p.X = b.Margin.Left - sshalf;
                        }
                    }
                }

                // check horizontal snap points
#if NETCOREAPP
                foreach (UIElement? item in canHorizontal.Children)
#else
                foreach (UIElement item in canHorizontal.Children)
#endif
                {
                    if (item != null && item is Border b)
                    {
                        if (b.Margin.Top > heightMin && b.Margin.Top < heightMax)
                        {
                            p.Y = b.Margin.Top;
                            //p.Y = b.Margin.Top - sshalf;
                        }
                    }
                }
            }

            // move selector to this new point
            //ellSelect.Margin = new Thickness(p.X - sshalf, p.Y - sshalf, 0, 0);
            Canvas.SetLeft(ellSelect2, p.X - sshalf);
            Canvas.SetTop(ellSelect2, p.Y - sshalf);

            // update outputs
            qWidth = p.X / grdSelArea.ActualWidth;
            qHeight = p.Y / grdSelArea.ActualHeight;
            SelectedPositionChanged?.Invoke(this, EventArgs.Empty);
        }

        private double oWidth = 0.5;
        private double oHeight = 0.5;

        private double qWidth = 0.5;
        private double qHeight = 0.5;

        /// <summary>
        /// Get or set the first selected value on the horizontal (X) axis.
        /// This is how far from the left edge of the control that the selector is, on a relative scale from <c>0.0</c> to <c>1.0</c>.
        /// </summary>
        public double SelectedWidth1
        {
            get { return oWidth; }
            set
            {
                double val = value;
                if (val < 0)
                {
                    val = 0;
                }
                else if (val > 1)
                {
                    val = 1;
                }

                double sshalf = SelectorSize / 2;
                oWidth = val;
                //Thickness th = ellSelect.Margin;
                //ellSelect.Margin = new Thickness((grdSelArea.ActualWidth * val) - sshalf, th.Top, 0, 0);
                Canvas.SetLeft(ellSelect, (grdSelArea.ActualWidth * val) - sshalf);
                SelectedPositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get or set the first selected value on the vertical (Y) axis.
        /// This is how far from the top of the control that the selector is, on a relative scale from <c>0.0</c> to <c>1.0</c>.
        /// </summary>
        public double SelectedHeight1
        {
            get { return oHeight; }
            set
            {
                double val = value;
                if (val < 0)
                {
                    val = 0;
                }
                else if (val > 1)
                {
                    val = 1;
                }

                double sshalf = SelectorSize / 2;
                oHeight = val;
                //Thickness th = ellSelect.Margin;
                //ellSelect.Margin = new Thickness(th.Left, (grdSelArea.ActualHeight * val) - sshalf, 0, 0);
                Canvas.SetTop(ellSelect, (grdSelArea.ActualHeight * val) - sshalf);
                SelectedPositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get or set the second selected value on the horizontal (X) axis.
        /// This is how far from the left edge of the control that the selector is, on a relative scale from <c>0.0</c> to <c>1.0</c>.
        /// </summary>
        public double SelectedWidth2
        {
            get { return qWidth; }
            set
            {
                double val = value;
                if (val < 0)
                {
                    val = 0;
                }
                else if (val > 1)
                {
                    val = 1;
                }

                double sshalf = SelectorSize / 2;
                qWidth = val;
                //Thickness th = ellSelect.Margin;
                //ellSelect.Margin = new Thickness((grdSelArea.ActualWidth * val) - sshalf, th.Top, 0, 0);
                Canvas.SetLeft(ellSelect2, (grdSelArea.ActualWidth * val) - sshalf);
                SelectedPositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get or set the second selected value on the vertical (Y) axis.
        /// This is how far from the top of the control that the selector is, on a relative scale from <c>0.0</c> to <c>1.0</c>.
        /// </summary>
        public double SelectedHeight2
        {
            get { return qHeight; }
            set
            {
                double val = value;
                if (val < 0)
                {
                    val = 0;
                }
                else if (val > 1)
                {
                    val = 1;
                }

                double sshalf = SelectorSize / 2;
                qHeight = val;
                //Thickness th = ellSelect.Margin;
                //ellSelect.Margin = new Thickness(th.Left, (grdSelArea.ActualHeight * val) - sshalf, 0, 0);
                Canvas.SetTop(ellSelect2, (grdSelArea.ActualHeight * val) - sshalf);
                SelectedPositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised when any of the SelectedHeight or SelectedWidth properties change (i.e., when the selector was moved).
        /// </summary>
#if NETCOREAPP
        public event EventHandler? SelectedPositionChanged;
#else
        public event EventHandler SelectedPositionChanged;
#endif
        #endregion
    }
}
