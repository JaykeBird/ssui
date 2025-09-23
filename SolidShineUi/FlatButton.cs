using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace SolidShineUi
{
    /// <summary>
    /// A button with a custom, flat style and additional funcionality. Use <c>SelectOnClick</c> to have the button act like a toggle button.
    /// </summary>
    [DefaultEvent("Click")]
    [Localizability(LocalizationCategory.Button)]
    public class FlatButton : ButtonBase, IClickSelectableControl
    {
        static FlatButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlatButton), new FrameworkPropertyMetadata(typeof(FlatButton)));
        }

        /// <summary>
        /// Create a new FlatButton.
        /// </summary>
        public FlatButton()
        {
            MouseDown += UserControl_MouseDown;
            MouseUp += UserControl_MouseUp;
            //MouseEnter += UserControl_MouseEnter;
            MouseLeave += UserControl_MouseLeave;
            //TouchDown += UserControl_TouchDown;
            //TouchUp += UserControl_TouchUp;
            //StylusDown += UserControl_StylusDown;
            //StylusUp += UserControl_StylusUp;

            PreviewMouseDown += UserControl_PreviewMouseDown;
            PreviewMouseUp += UserControl_PreviewMouseUp;

            //GotFocus += UserControl_GotFocus;
            //GotKeyboardFocus += UserControl_GotKeyboardFocus;
            LostFocus += UserControl_LostFocus;
            LostKeyboardFocus += UserControl_LostKeyboardFocus;

            KeyDown += UserControl_KeyDown;
            KeyUp += UserControl_KeyUp;

            //IsEnabledChanged += fBtn_IsEnabledChanged;

            Focusable = true;
            KeyboardNavigation.SetIsTabStop(this, true);

            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;

            Padding = new Thickness(5, 0, 5, 0);

            //invalidTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            //invalidTimer.Tick += InvalidTimer_Tick;
        }


        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the control.
        /// </summary>
        [Category("Brushes")]
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse is clicking it.
        /// </summary>
        [Category("Brushes")]
        public Brush ClickBrush
        {
            get => (Brush)GetValue(ClickBrushProperty);
            set => SetValue(ClickBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of this button while it is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public Brush SelectedBrush
        {
            get => (Brush)GetValue(SelectedBrushProperty);
            set => SetValue(SelectedBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse is over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the control when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush DisabledBrush
        {
            get => (Brush)GetValue(DisabledBrushProperty);
            set => SetValue(DisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border of the control when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get => (Brush)GetValue(BorderDisabledBrushProperty);
            set => SetValue(BorderDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border around the edges of the control.
        /// </summary>
        [Category("Brushes")]
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border while the control has the mouse over it (or it has keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public Brush BorderHighlightBrush
        {
            get => (Brush)GetValue(BorderHighlightBrushProperty);
            set => SetValue(BorderHighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border while the control is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public Brush BorderSelectedBrush
        {
            get => (Brush)GetValue(BorderSelectedBrushProperty);
            set => SetValue(BorderSelectedBrushProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="Background"/>. See the related property for details.</summary>
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.White.ToBrush()));

        /// <summary>The backing dependency property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>The backing dependency property for <see cref="SelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.WhiteSmoke.ToBrush()));

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.LightGray.ToBrush()));

        /// <summary>The backing dependency property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.DarkGray.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BorderBrush"/>. See the related property for details.</summary>
        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderHighlightBrushProperty = DependencyProperty.Register(
            "BorderHighlightBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BorderSelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectedBrushProperty = DependencyProperty.Register(
            "BorderSelectedBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.DimGray.ToBrush()));

        #endregion

        #region ColorScheme/TransparentBack/UseAccentColors

        //DispatcherTimer invalidTimer = new DispatcherTimer();

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif


        /// <summary>The backing dependency property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(FlatButton),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));


        //bool use_transp = false;
        //bool use_accent = false;

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is FlatButton f)
                {
                    f.ApplyColorScheme(cs, f.TransparentBack, f.UseAccentColors);
                    f.ColorSchemeChanged?.Invoke(d, e);
                }
            }

//#if NETCOREAPP
//            ColorScheme cs = (e.NewValue as ColorScheme)!;
//#else
//            ColorScheme cs = e.NewValue as ColorScheme;
//#endif


        }

        /// <summary>
        /// Get or set the color scheme to apply to this button. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        [Category("Appearance")]
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        bool runApply = true;

        /// <summary>The backing dependency property for <see cref="TransparentBack"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TransparentBackProperty 
            = DependencyProperty.Register("TransparentBack", typeof(bool), typeof(FlatButton), 
            new PropertyMetadata(false, new PropertyChangedCallback(OnTransparentBackChanged)));

        /// <summary>
        /// Get or set whether the button should have a transparent background when the button is not focused.
        /// </summary>
        [Category("Common")]
        public bool TransparentBack
        {
            get => (bool)GetValue(TransparentBackProperty);
            set => SetValue(TransparentBackProperty, value);
        }

        /// <summary>
        /// Perform an action when a property of an object has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnTransparentBackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool tb)
            {
                if (d is FlatButton f)
                {
                    f.ApplyColorScheme(f.ColorScheme, tb, f.UseAccentColors);
                }
            }
        }

        /// <summary>The backing dependency property for <see cref="UseAccentColors"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseAccentColorsProperty
            = DependencyProperty.Register("UseAccentColors", typeof(bool), typeof(FlatButton),
            new PropertyMetadata(false, new PropertyChangedCallback(OnUseAccentColorsChanged)));

        /// <summary>
        /// Get or set if the button should use the accent colors of the color scheme, rather than the standard colors.
        /// </summary>
        [Category("Appearance")]
        public bool UseAccentColors
        {
            get => (bool)GetValue(UseAccentColorsProperty);
            set => SetValue(UseAccentColorsProperty, value);
        }

        /// <summary>
        /// Perform an action when a property of an object has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnUseAccentColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool ua)
            {
                if (d is FlatButton f)
                {
                    f.ApplyColorScheme(f.ColorScheme, f.TransparentBack, ua);
                }
            }
        }

        ///// <summary>
        ///// Get or set whether the button should have a transparent background when the button is not focused.
        ///// </summary>
        //public bool TransparentBack
        //{
        //    get
        //    {
        //        return use_transp;
        //    }
        //    set
        //    {
        //        use_transp = value;
        //        if (runApply) ApplyColorScheme(ColorScheme, value, use_accent);
        //    }
        //}

        ///// <summary>
        ///// Get or set if the button should use the accent colors of the color scheme, rather than the standard colors.
        ///// </summary>
        //public bool UseAccentColors
        //{
        //    get
        //    {
        //        return use_accent;
        //    }
        //    set
        //    {
        //        use_accent = value;
        //        if (runApply) ApplyColorScheme(ColorScheme, TransparentBack, value);
        //    }
        //}

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            ApplyColorScheme(cs, TransparentBack, UseAccentColors);
        }

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        /// <param name="transparentBack">Set if the button should have no background when not focused or highlighted. This can also be achieved with the <c>TransparentBack</c> property.</param>
        /// <param name="useAccentColors">Set if accent colors should be used for this button, rather than the main color scheme colors.
        /// This can also be achieved with the <c>UseAccentColors</c> property.
        /// </param>
        public void ApplyColorScheme(ColorScheme cs, bool transparentBack = false, bool useAccentColors = false)
        {
            if (runApply == false)
            {
                return;
            }

            if (cs == null)
            {
                return;
            }

            if (cs != ColorScheme)
            {
                runApply = false;
                TransparentBack = TransparentBack || transparentBack;
                UseAccentColors = UseAccentColors || useAccentColors;
                runApply = true;
                ColorScheme = cs;
                return;
            }

            //colScheme = cs;

            runApply = false;

            if (cs.IsHighContrast)
            {
                Background = cs.BackgroundColor.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                SelectedBrush = cs.HighlightColor.ToBrush();
                BorderHighlightBrush = cs.BorderColor.ToBrush();
                BorderSelectedBrush = cs.BorderColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                DisabledBrush = cs.BackgroundColor.ToBrush();
                Foreground = cs.ForegroundColor.ToBrush();
                ClickBrush = cs.ThirdHighlightColor.ToBrush();

                if (transparentBack || TransparentBack)
                {
                    BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();
                    TransparentBack = true;
                }
                else
                {
                    BorderBrush = cs.BorderColor.ToBrush();
                }
            }
            else if (transparentBack || TransparentBack)
            {
                Background = Color.FromArgb(1, 0, 0, 0).ToBrush();
                BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();

                if (UseAccentColors || useAccentColors)
                {
                    HighlightBrush = cs.AccentSecondHighlightColor.ToBrush();
                    SelectedBrush = cs.AccentThirdHighlightColor.ToBrush();
                    BorderHighlightBrush = cs.AccentHighlightColor.ToBrush();
                    BorderSelectedBrush = cs.AccentSelectionColor.ToBrush();
                    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                    DisabledBrush = cs.LightDisabledColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.AccentThirdHighlightColor.ToBrush();

                    UseAccentColors = true;
                }
                else
                {
                    HighlightBrush = cs.SecondHighlightColor.ToBrush();
                    DisabledBrush = cs.LightDisabledColor.ToBrush();
                    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                    SelectedBrush = cs.ThirdHighlightColor.ToBrush();
                    BorderHighlightBrush = cs.HighlightColor.ToBrush();
                    BorderSelectedBrush = cs.SelectionColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.ThirdHighlightColor.ToBrush();
                }

                TransparentBack = true;
            }
            else
            {
                if (UseAccentColors || useAccentColors)
                {
                    Background = cs.AccentSecondaryColor.ToBrush();
                    BorderBrush = cs.AccentBorderColor.ToBrush();
                    HighlightBrush = cs.AccentSecondHighlightColor.ToBrush();
                    DisabledBrush = cs.LightDisabledColor.ToBrush();
                    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                    SelectedBrush = cs.AccentThirdHighlightColor.ToBrush();
                    BorderHighlightBrush = cs.AccentHighlightColor.ToBrush();
                    BorderSelectedBrush = cs.AccentSelectionColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.AccentThirdHighlightColor.ToBrush();

                    UseAccentColors = true;
                }
                else
                {
                    Background = cs.SecondaryColor.ToBrush();
                    BorderBrush = cs.BorderColor.ToBrush();
                    HighlightBrush = cs.SecondHighlightColor.ToBrush();
                    DisabledBrush = cs.LightDisabledColor.ToBrush();
                    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                    SelectedBrush = cs.ThirdHighlightColor.ToBrush();
                    BorderHighlightBrush = cs.HighlightColor.ToBrush();
                    BorderSelectedBrush = cs.SelectionColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.ThirdHighlightColor.ToBrush();
                }
            }

            runApply = true;

            //InvalidateMeasure();
            //InvalidateVisual();
            //invalidTimer.Start();

            if (Template == null)
            {
                return;
            }

            // //** why is this section here?
            //Border border = (Border)Template.FindName("btn_Border", this);
            //if (border != null)
            //{
            //    if (IsSelected)
            //    {
            //        border.Background = SelectedBrush;
            //        border.BorderBrush = BorderSelectedBrush;
            //        border.BorderThickness = BorderSelectionThickness;
            //    }
            //    else
            //    {
            //        border.Background = Background;
            //        border.BorderBrush = BorderBrush;
            //        border.BorderThickness = BorderThickness;
            //    }
            //}


            //GetBindingExpression(ColorSchemeProperty).UpdateTarget();
            //fBtn_IsEnabledChanged(this, new DependencyPropertyChangedEventArgs());
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="hco">The high-contrast color scheme to apply.</param>
        /// <param name="transparentBack">Set if the button should have no background when not focused or highlighted. 
        /// This can also be achieved with the <c>TransparentBack</c> property.</param>
        /// <remarks>
        /// This method will be removed in version 2.0. Instead, use <see cref="ApplyColorScheme(ColorScheme)"/> and use
        /// <see cref="ColorScheme.GetHighContrastScheme(HighContrastOption)"/> to acquire the high contrast theme.
        /// </remarks>
        [Obsolete("This overload of the ApplyColorScheme method will be removed in the future. Please use the other ApplyColorScheme method, " +
            "and use ColorScheme.GetHighContrastScheme to get the desired high-contrast scheme.", false)]
        public void ApplyColorScheme(HighContrastOption hco, bool transparentBack = false)
        {
            ColorScheme cs = ColorScheme.GetHighContrastScheme(hco);
            ApplyColorScheme(cs, transparentBack, false);
            //if (cs != ColorScheme)
            //{
            //    ColorScheme = cs;
            //    TransparentBack = transparentBack;
            //    return;
            //}

            //if (transparentBack || TransparentBack)
            //{
            //    Background = Color.FromArgb(1, 0, 0, 0).ToBrush();
            //    BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();
            //    HighlightBrush = cs.HighlightColor.ToBrush();
            //    DisabledBrush = cs.LightDisabledColor.ToBrush();
            //    BorderDisabledBrush = cs.BorderColor.ToBrush();
            //    SelectedBrush = cs.HighlightColor.ToBrush();
            //    BorderHighlightBrush = cs.BorderColor.ToBrush();
            //    BorderSelectedBrush = cs.BorderColor.ToBrush();
            //    Foreground = cs.ForegroundColor.ToBrush();
            //    TransparentBack = true;
            //}
            //else
            //{
            //    Background = cs.BackgroundColor.ToBrush();
            //    BorderBrush = cs.BorderColor.ToBrush();
            //    HighlightBrush = cs.HighlightColor.ToBrush();
            //    DisabledBrush = cs.LightDisabledColor.ToBrush();
            //    BorderDisabledBrush = cs.BorderColor.ToBrush();
            //    SelectedBrush = cs.HighlightColor.ToBrush();
            //    BorderHighlightBrush = cs.BorderColor.ToBrush();
            //    BorderSelectedBrush = cs.BorderColor.ToBrush();
            //    Foreground = cs.ForegroundColor.ToBrush();
            //}

            ////fBtn_IsEnabledChanged(this, new DependencyPropertyChangedEventArgs());
        }

        //private void InvalidTimer_Tick(object sender, EventArgs e)
        //{
        //    if (Application.Current != null)
        //    {
        //        Dispatcher.Invoke(() =>
        //        {
        //            InvalidateMeasure();
        //            InvalidateVisual();
        //        }, DispatcherPriority.Render);
        //    }

        //    invalidTimer.IsEnabled = false;
        //}

        #endregion

        #region Border

        /// <summary>The backing dependency property for <see cref="BorderThickness"/>. See the related property for details.</summary>
        public new static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness", typeof(Thickness), typeof(FlatButton),
            new PropertyMetadata(new Thickness(1)));

        /// <summary>The backing dependency property for <see cref="BorderSelectionThickness"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectionThicknessProperty = DependencyProperty.Register(
            "BorderSelectionThickness", typeof(Thickness), typeof(FlatButton),
            new PropertyMetadata(new Thickness(2)));

        /// <summary>The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(FlatButton),
            new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// Get or set the thickness of the border around the button.
        /// </summary>
        [Category("Appearance")]
        public new Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        /// <summary>
        /// Get or set the thickness of the border around the button, while the button is in a selected (<c>IsSelected</c>) state.
        /// </summary>
        [Category("Appearance")]
        public Thickness BorderSelectionThickness
        {
            get => (Thickness)GetValue(BorderSelectionThicknessProperty);
            set => SetValue(BorderSelectionThicknessProperty, value);
        }

        /// <summary>
        /// Get or set the corner radius (or radii) to use for the button and its border. Can be used to create a rounded button.
        /// </summary>
        [Category("Appearance")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region HighlightOnKeyboardFocus

        /// <summary>
        /// Get or set if the button should be highlighted (using the <see cref="HighlightBrush"/> and <see cref="BorderHighlightBrush"/>)
        /// when it has keyboard focus. If <c>false</c>, only the keyboard focus outline appears, and highlighting only occurs on mouse/stylus over.
        /// </summary>
        public bool HighlightOnKeyboardFocus { get => (bool)GetValue(HighlightOnKeyboardFocusProperty); set => SetValue(HighlightOnKeyboardFocusProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightOnKeyboardFocus"/>. See the related property for details.</summary>
        public static DependencyProperty HighlightOnKeyboardFocusProperty
            = DependencyProperty.Register(nameof(HighlightOnKeyboardFocus), typeof(bool), typeof(FlatButton),
            new FrameworkPropertyMetadata(false));

        #endregion

        #region Click / Selection Handling

        #region Routed Events

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            nameof(RightClick), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FlatButton));

        /// <summary>
        /// Raised when the user right-clicks on the button, via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler RightClick
        {
            add { AddHandler(RightClickEvent, value); }
            remove { RemoveHandler(RightClickEvent, value); }
        }

        #endregion

        #region IsMouseDown

        // from https://stackoverflow.com/questions/10667545/why-ismouseover-is-recognized-and-mousedown-isnt-wpf-style-trigger

        /// <summary>
        /// The internal dependency property for <see cref="IsMouseDown"/>. See that property for more details.
        /// </summary>
        protected static readonly DependencyPropertyKey IsMouseDownPropertyKey = DependencyProperty.RegisterAttachedReadOnly(nameof(IsMouseDown),
            typeof(bool), typeof(FlatButton), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get if there is a mouse button currently being pressed, while the mouse cursor is over this control.
        /// </summary>
        public static readonly DependencyProperty IsMouseDownProperty = IsMouseDownPropertyKey.DependencyProperty;

        /// <summary>
        /// Set the IsMouseDown property for a FlatButton.
        /// </summary>
        /// <param name="obj">The FlatButton to apply the property change to.</param>
        /// <param name="value">The new value to set for the property.</param>
        protected static void SetIsMouseDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseDownPropertyKey, value);
        }

        /// <summary>
        /// Get the IsMouseDown property for a FlatButton.
        /// </summary>
        /// <param name="obj">The Flatbutton to get the property value from.</param>
        public static bool GetIsMouseDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDownProperty);
        }

        /// <summary>
        /// Get if a mouse button is currently being pressed while the cursor is over this FlatButton.
        /// </summary>
        public bool IsMouseDown
        {
            get => (bool)GetValue(IsMouseDownProperty);
            protected set => SetValue(IsMouseDownPropertyKey, value);
        }

        #endregion

        #region Variables/Properties

        bool initiatingClick = false;
        bool _runSelChangeEvent = true;

        /// <summary>
        /// Raised if the button's <see cref="IsSelected"/> value is changed. If using this button as a toggle button, you should listen to this event 
        /// (rather than <c>Click</c>) to monitor changes to the <c>IsSelected</c> value.
        /// </summary>
#if NETCOREAPP
        public event ItemSelectionChangedEventHandler? IsSelectedChanged;
#else
        public event ItemSelectionChangedEventHandler IsSelectedChanged;
#endif


        /// <summary>
        /// The backing dependency property object for the <see cref="IsSelected"/> property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected), typeof(bool), typeof(FlatButton), new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Perform an action when a property of an object has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool se)
            {
                bool old = Convert.ToBoolean(e.OldValue);

                if (d is FlatButton f)
                {
                    if (f._runSelChangeEvent)
                    {
                        f.IsSelectedChanged?.Invoke(f, new ItemSelectionChangedEventArgs(old, se, IsSelectedProperty, SelectionChangeTrigger.CodeUnknown, null));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this button is selected. This property (combined with <see cref="SelectOnClick"/>) allows the button to function like a ToggleButton.
        /// </summary>
        /// <remarks>
        /// A selected button will have a slightly different visual appearance to differentiate it as being selected. This will include, by default, the border being a bit thicker.
        /// This can be changed via the <see cref="BorderSelectionThickness"/> property. You can also directly edit the brushes used via the <see cref="SelectedBrush"/> and
        /// <see cref="BorderSelectedBrush"/> properties.
        /// <para />
        /// To listen to changes to this property, use <see cref="IsSelectedChanged"/>, rather than listening to the <c>Click</c> event, as other actions could change this 
        /// property rather than just clicking it.
        /// </remarks>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Set the <see cref="IsSelected"/> value of this control, while also defining how the selection was changed.
        /// </summary>
        /// <param name="value">The value to set <see cref="IsSelected"/> to.</param>
        /// <param name="triggerMethod">The source or method used to trigger the change in selection.</param>
        /// <param name="triggerSource">The object that triggered the change.</param>
#if NETCOREAPP
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger triggerMethod, object? triggerSource = null)
#else
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger triggerMethod, object triggerSource = null)
#endif
        {
            bool old = IsSelected;

            _runSelChangeEvent = false;
            IsSelected = value;
            _runSelChangeEvent = true;

            IsSelectedChanged?.Invoke(this, new ItemSelectionChangedEventArgs(old, value, IsSelectedProperty, triggerMethod, triggerSource));
        }
        
        /// <summary>
        /// The backing dependency property object for the <see cref="SelectOnClick"/> property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectOnClickProperty = DependencyProperty.Register(
            nameof(SelectOnClick), typeof(bool), typeof(FlatButton), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.
        /// </summary>
        /// <remarks>
        /// While SelectOnClick is true, the button will toggle between <see cref="IsSelected"/> being true and false (similar to a ToggleButton).<para/>
        /// The button's Click event will still be raised while this property is set to <c>true</c>, but the event occurs after the
        /// IsSelected property has already changed. Do not use Click event to check when the button's IsSelected property is changed, but instead the IsSelectedChanged event,
        /// in case of situations where IsSelected is changed via methods other than clicking, such as programmatically or via WPF binding.
        /// </remarks>
        [Category("Common")]
        public bool SelectOnClick
        {
            get => (bool)GetValue(SelectOnClickProperty);
            set => SetValue(SelectOnClickProperty, value);
        }

        #endregion

        #region Base Click Functions

        // Sets up the button to be clicked. This must be run before PerformClick.
        void PressRightClick()
        {
            initiatingClick = true;
        }

        // If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        void PerformRightClick()
        {
            if (initiatingClick)
            {
                RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
                RaiseEvent(rre);
            }
        }

        /// <summary>
        /// Perform a click programmatically. The button responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            OnClick();
        }

        /// <summary>
        /// Defines the actions the button performs when it is clicked.
        /// </summary>
        protected override void OnClick()
        {
            if (SelectOnClick)
            {
                SetIsSelectedWithSource(!IsSelected, SelectionChangeTrigger.ControlClick, this);
            }

            base.OnClick();
        }

        #endregion

        #region Event handlers

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Right)
            //{
            //    PressRightClick();
            //}
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Right)
            //{
            //    PerformRightClick();
            //}
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (ClickMode == ClickMode.Press && e.Key == Key.Apps)
            {
                PressRightClick();
                PerformRightClick();
            }
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (ClickMode != ClickMode.Press && e.Key == Key.Apps)
            {
                PressRightClick();
                PerformRightClick();
            }
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                PressRightClick();
            }
            SetIsMouseDown(this, true);
        }

        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                PerformRightClick();
            }
            SetIsMouseDown(this, false);
        }


        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            initiatingClick = false;
        }

        private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            initiatingClick = false;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            SetIsMouseDown(this, false);
            initiatingClick = false;
        }


        #endregion

        #endregion

        #region IsDefault

        // OnDefault code adapted from .NET Core WPF repository
        // https://github.com/dotnet/wpf/blob/master/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/Button.cs

        // unfortunately, I'm unable to actually achieve the "IsDefault" functionality as is present in WPF
        // the reason is that the WPF button accesses properties and methods marked as "internal" - which means it only works within that library
        // the biggest culprits are the KeyboardNavigation instance in FrameworkElement, and the FocusChanged event in KeyboardNavigation
        // if those two were accessible, then I would be able to mirror the code exactly and set up my buttons as capable of being a "default" button
        // but now, that's a feature that's only exclusive to WPF's own Buttons, which really sucks
        // there's a 0% chance Microsoft will change how .NET Framework's WPF acts, and I find it unlikely they'll entertain a PR to make those values public
        // (since there's probably concerns they have about malicious/imcompetent coders misusing or abusing the KeyboardNavigation instance)
        // so I'd be left with just creating my own instance of a Button and hijacking it, but I'm concerned that might not work either

//#if NETCOREAPP
//        private KeyboardFocusChangedEventHandler? FocusChangedHandler = null;
//#else
//        private KeyboardFocusChangedEventHandler FocusChangedHandler = null;
//#endif

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty IsDefaultProperty
            = DependencyProperty.Register("IsDefault", typeof(bool), typeof(FlatButton),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDefaultChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if this button is the default button in the dialog or window it is located in. Due to limitations in WPF, this actually does not work.
        /// </summary>
        [Obsolete("Due to limitations with WPF, this property actually does not work. I apologize, but you will need to explore other options.")]
        public bool IsDefault
        {
            get => (bool)GetValue(IsDefaultProperty);
            set => SetValue(IsDefaultProperty, value);
        }

        private static void OnIsDefaultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlatButton)
            {
                //if (b.FocusChangedHandler == null)
                //{
                //    b.FocusChangedHandler = new KeyboardFocusChangedEventHandler(b.OnFocusChanged);
                //}


                //if ((bool)e.NewValue)
                //{
                //    AccessKeyManager.Register("\x000D", b);
                //    KeyboardNavigation.FocusChanged += b.FocusChangedHandler;
                //    b.UpdateIsDefaulted(Keyboard.FocusedElement);
                //}
                //else
                //{
                //    AccessKeyManager.Unregister("\x000D", b);
                //    KeyboardNavigation.FocusChanged -= b.FocusChangedHandler;
                //    b.UpdateIsDefaulted(null);
                //}
            }
        }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey IsDefaultedPropertyKey
            = DependencyProperty.RegisterReadOnly("IsDefaulted", typeof(bool), typeof(FlatButton),
            new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsDefaultedProperty = IsDefaultedPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Specifies whether or not this button is the button that would be invoked when Enter is pressed. Due to limitations in WPF, this actually does not work.
        /// </summary>
        [Obsolete("Due to limitations with WPF, this property actually does not work. I apologize, but you will need to explore other options.")]
        public bool IsDefaulted
        {
            get
            {
                return (bool)GetValue(IsDefaultedProperty);
            }
        }

        private void OnFocusChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            UpdateIsDefaulted(Keyboard.FocusedElement);
        }

#if NETCOREAPP
        private void UpdateIsDefaulted(IInputElement? focus)
#else
        private void UpdateIsDefaulted(IInputElement focus)
#endif
        {
            // If it's not a default button, or nothing is focused, or it's disabled then it's not defaulted.
#pragma warning disable CS0618 // Type or member is obsolete
            if (!IsDefault || focus == null || !IsEnabled)
            {
                SetValue(IsDefaultedPropertyKey, false);
                return;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            if (focus is DependencyObject focusDO)
            {
                object thisScope, focusScope;

                // If the focused thing is not in this scope then IsDefaulted = false
                AccessKeyPressedEventArgs e;

                object isDefaulted = false;
                try
                {
                    // Step 1: Determine the AccessKey scope from currently focused element
                    e = new AccessKeyPressedEventArgs();
                    focus.RaiseEvent(e);
                    focusScope = e.Scope;

                    // Step 2: Determine the AccessKey scope from this button
                    e = new AccessKeyPressedEventArgs();
                    this.RaiseEvent(e);
                    thisScope = e.Scope;

                    // Step 3: Compare scopes
                    if (thisScope == focusScope && (focusDO == null || (bool)focusDO.GetValue(KeyboardNavigation.AcceptsReturnProperty) == false))
                    {
                        isDefaulted = true;
                    }
                }
                finally
                {
                    SetValue(IsDefaultedPropertyKey, isDefaulted);
                }
            }
        }

#endregion
    }
}
