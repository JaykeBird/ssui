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
    public class FlatButton : ButtonBase
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

#if NETCOREAPP
        /// <summary>
        /// Raised if the button's IsSelected value is changed. This can be used to have the button act as a ToggleButton.
        /// </summary>
        public event DependencyPropertyChangedEventHandler? IsSelectedChanged;
#else
        ///// <summary>
        ///// Raised when the user clicks on the button. Please use the new Click event instead.
        ///// </summary>
        //[Obsolete("Please transition to the new Click event, which is a routed event. This will be removed in a later version.", false)]
        //public event EventHandler UnroutedClick;

        /// <summary>
        /// Raised if the button's IsSelected value is changed. This can be used to have the button act as a ToggleButton.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsSelectedChanged;
#endif


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

        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.White.ToBrush()));

        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Gainsboro.ToBrush()));

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.WhiteSmoke.ToBrush()));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.LightGray.ToBrush()));

        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Gray.ToBrush()));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.DarkGray.ToBrush()));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Black.ToBrush()));

        public static readonly DependencyProperty BorderHighlightBrushProperty = DependencyProperty.Register(
            "BorderHighlightBrush", typeof(Brush), typeof(FlatButton),
            new PropertyMetadata(Colors.Black.ToBrush()));

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

        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(FlatButton),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        //bool use_transp = false;
        //bool use_accent = false;

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is FlatButton f)
                {
                    f.ColorSchemeChanged?.Invoke(d, e);
                    f.ApplyColorScheme(cs, f.TransparentBack, f.UseAccentColors);
                }
            }

//#if NETCOREAPP
//            ColorScheme cs = (e.NewValue as ColorScheme)!;
//#else
//            ColorScheme cs = e.NewValue as ColorScheme;
//#endif


        }

        /// <summary>
        /// Get or set the color scheme to use with the button.
        /// </summary>
        [Category("Appearance")]
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        bool runApply = true;

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
        /// <param name="transparentBack">Set if the button should have no background when not focused or highlighted. This can also be achieved with the <c>TransparentBack</c> property.</param>
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

        public new static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness", typeof(Thickness), typeof(FlatButton),
            new PropertyMetadata(new Thickness(1)));

        public static readonly DependencyProperty BorderSelectionThicknessProperty = DependencyProperty.Register(
            "BorderSelectionThickness", typeof(Thickness), typeof(FlatButton),
            new PropertyMetadata(new Thickness(2)));

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

        #region Click Handling

        #region Routed Events

        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            "RightClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FlatButton));

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

        protected static readonly DependencyPropertyKey IsMouseDownPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsMouseDown",
            typeof(bool), typeof(FlatButton), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsMouseDownProperty = IsMouseDownPropertyKey.DependencyProperty;

        protected static void SetIsMouseDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseDownPropertyKey, value);
        }

        public static bool GetIsMouseDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDownProperty);
        }

        #endregion

        #region Variables/Properties
        bool initiatingClick = false;

        //bool sel = false;

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(FlatButton),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        public static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool se)
            {
                if (d is FlatButton f)
                {
                    f.IsSelectedChanged?.Invoke(d, e);
                    //f.ApplyColorScheme(f.ColorScheme, tb, f.UseAccentColors);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this button is selected. This property (combined with <c>SelectOnClick</c>) allows the button to function like a ToggleButton.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                //bool sel = value;
                SetValue(IsSelectedProperty, value);

                //if (Template != null)
                //{
                //    Border border = (Border)Template.FindName("btn_Border", this);
                //    if (border != null)
                //    {
                //        if (sel)
                //        {
                //            border.Background = SelectedBrush;
                //        }
                //        else
                //        {
                //            border.Background = Background;
                //        }
                //    }
                //}

                //SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public static readonly DependencyProperty SelectOnClickProperty = DependencyProperty.Register(
            "SelectOnClick", typeof(bool), typeof(FlatButton),
            new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.
        /// </summary>
        [Category("Common")]
        public bool SelectOnClick
        {
            get => (bool)GetValue(SelectOnClickProperty);
            set => SetValue(SelectOnClickProperty, value);
        }

        #endregion

        /// <summary>
        /// Sets up the button to be clicked. This must be run before PerformClick.
        /// </summary>
        void PressRightClick()
        {
            initiatingClick = true;
        }

        /// <summary>
        /// If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        /// </summary>
        void PerformRightClick()
        {
            if (initiatingClick)
            {
                RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
                RaiseEvent(rre);
            }
        }

        /// <summary>
        /// Perform a click programattically. The button responds the same way as if it was clicked by the user.
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
                IsSelected = !IsSelected;
            }

            base.OnClick();
        }

        #region Event handlers

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                PressRightClick();
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                PerformRightClick();
            }
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
            //e.Handled = false;
            if (e.ChangedButton == MouseButton.Right)
            {
                PressRightClick();
            }
            SetIsMouseDown(this, true);
            //IsPressed = true;
            //base.OnPreviewMouseDown(e);
        }

        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = false;
            if (e.ChangedButton == MouseButton.Right)
            {
                PerformRightClick();
            }
            SetIsMouseDown(this, false);
            //IsPressed = false;
            //base.OnPreviewMouseUp(e);
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

        public static readonly DependencyProperty IsDefaultProperty
            = DependencyProperty.Register("IsDefault", typeof(bool), typeof(FlatButton),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDefaultChanged)));

        public bool IsDefault
        {
            get => (bool)GetValue(IsDefaultProperty);
            set => SetValue(IsDefaultProperty, value);
        }

        private static void OnIsDefaultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlatButton b)
            {
                if ((bool)e.NewValue)
                {
                    AccessKeyManager.Register("\x000D", b);
                    b.UpdateIsDefaulted(System.Windows.Input.Keyboard.FocusedElement);
                }
                else
                {
                    AccessKeyManager.Unregister("\x000D", b);
                    b.UpdateIsDefaulted(null);
                }
            }
        }

        /// <summary>
        ///     The key needed set a read-only property.
        /// </summary>
        private static readonly DependencyPropertyKey IsDefaultedPropertyKey
            = DependencyProperty.RegisterReadOnly("IsDefaulted", typeof(bool), typeof(FlatButton),
            new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsDefaultedProperty = IsDefaultedPropertyKey.DependencyProperty;

        /// <summary>
        /// Specifies whether or not this button is the button that would be invoked when Enter is pressed.
        /// </summary>
        public bool IsDefaulted
        {
            get
            {
                return (bool)GetValue(IsDefaultedProperty);
            }
        }

        private void OnFocusChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            UpdateIsDefaulted(System.Windows.Input.Keyboard.FocusedElement);
        }

#if NETCOREAPP
        private void UpdateIsDefaulted(IInputElement? focus)
#else
        private void UpdateIsDefaulted(IInputElement focus)
#endif
        {
            // If it's not a default button, or nothing is focused, or it's disabled then it's not defaulted.
            if (!IsDefault || focus == null || !IsEnabled)
            {
                SetValue(IsDefaultedPropertyKey, false);
                return;
            }

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
