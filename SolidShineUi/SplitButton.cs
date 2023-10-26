using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A control that displays two buttons: a main button on the left which activates like a normal button, 
    /// and a secondary button on the right that displays a menu of additional options.
    /// </summary>
    public class SplitButton : ContentControl
    {
        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        /// <summary>
        /// Create a new SplitButton.
        /// </summary>
        public SplitButton()
        {

            ColorSchemeChanged += OnColorSchemeChanged;
        }

        private void OnColorSchemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (Menu != null) Menu.ApplyColorScheme(cs);
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(SplitButton),
            new PropertyMetadata(Colors.White.ToBrush()));

        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(SplitButton),
            new PropertyMetadata(Colors.Gainsboro.ToBrush()));

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush", typeof(Brush), typeof(SplitButton),
            new PropertyMetadata(Colors.WhiteSmoke.ToBrush()));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(SplitButton),
            new PropertyMetadata(Colors.LightGray.ToBrush()));

        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(SplitButton),
            new PropertyMetadata(Colors.Gray.ToBrush()));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(SplitButton),
            new PropertyMetadata(Colors.DarkGray.ToBrush()));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(SplitButton),
            new PropertyMetadata(Colors.Black.ToBrush()));

        public static readonly DependencyProperty BorderHighlightBrushProperty = DependencyProperty.Register(
            "BorderHighlightBrush", typeof(Brush), typeof(SplitButton),
            new PropertyMetadata(Colors.Black.ToBrush()));

        public static readonly DependencyProperty BorderSelectedBrushProperty = DependencyProperty.Register(
            "BorderSelectedBrush", typeof(Brush), typeof(SplitButton),
            new PropertyMetadata(Colors.DimGray.ToBrush()));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(SplitButton),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
                if (d is SplitButton f)
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty TransparentBackProperty
            = DependencyProperty.Register("TransparentBack", typeof(bool), typeof(SplitButton),
            new PropertyMetadata(false, new PropertyChangedCallback(OnTransparentBackChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
                if (d is SplitButton f)
                {
                    f.ApplyColorScheme(f.ColorScheme, tb, f.UseAccentColors);
                }
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty UseAccentColorsProperty
            = DependencyProperty.Register("UseAccentColors", typeof(bool), typeof(SplitButton),
            new PropertyMetadata(false, new PropertyChangedCallback(OnUseAccentColorsChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
                if (d is SplitButton f)
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

            runApply = false;

            if (cs.IsHighContrast)
            {
                Background = cs.BackgroundColor.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                SelectedBrush = cs.HighlightColor.ToBrush();
                BorderHighlightBrush = cs.BorderColor.ToBrush();
                BorderSelectedBrush = cs.BorderColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                SeparatorBrush = cs.BorderColor.ToBrush();
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
                    SeparatorBrush = cs.AccentBorderColor.ToBrush();
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
                    SeparatorBrush = cs.BorderColor.ToBrush();
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
                    SeparatorBrush = cs.AccentBorderColor.ToBrush();
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
                    SeparatorBrush = cs.BorderColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.ThirdHighlightColor.ToBrush();
                }
            }

            runApply = true;
        }

        #endregion

        #region Border

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public new static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness", typeof(Thickness), typeof(SplitButton),
            new PropertyMetadata(new Thickness(1)));

        public static readonly DependencyProperty BorderSelectionThicknessProperty = DependencyProperty.Register(
            "BorderSelectionThickness", typeof(Thickness), typeof(SplitButton),
            new PropertyMetadata(new Thickness(2)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(SplitButton),
            new PropertyMetadata(new CornerRadius(0)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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

        #region Separator Border

        /// <summary>
        /// Get or set if a separator bar should be shown between the main and menu buttons. Without the separator bar, the buttons look more connected, 
        /// but they can be hard to discern as two separate clickable buttons without mousing over them.
        /// </summary>
        public bool ShowSeparator { get => (bool)GetValue(ShowSeparatorProperty); set => SetValue(ShowSeparatorProperty, value); }

        /// <summary>The backing dependency property for <see cref="ShowSeparator"/>. See the related property for details.</summary>
        public static DependencyProperty ShowSeparatorProperty
            = DependencyProperty.Register("ShowSeparator", typeof(bool), typeof(SplitButton),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set the brush to use for the separator bar between the main and menu buttons.
        /// </summary>
        public Brush SeparatorBrush { get => (Brush)GetValue(SeparatorBrushProperty); set => SetValue(SeparatorBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SeparatorBrush"/>. See the related property for details.</summary>
        public static DependencyProperty SeparatorBrushProperty
            = DependencyProperty.Register("SeparatorBrush", typeof(Brush), typeof(SplitButton),
            new FrameworkPropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>
        /// Get or set how distant the separator bar should be from the edges of the button. The higher the number, the bigger the space between
        /// the edges of the button and the ends of the separator bar. A value of 0 will bring the separator bar right up to the edges.
        /// </summary>
        public double SeparatorEdgeMargin { get => (double)GetValue(SeparatorEdgeMarginProperty); set => SetValue(SeparatorEdgeMarginProperty, value); }

        /// <summary>The backing dependency property for <see cref="SeparatorEdgeMargin"/>. See the related property for details.</summary>
        public static DependencyProperty SeparatorEdgeMarginProperty
            = DependencyProperty.Register("SeparatorEdgeMargin", typeof(double), typeof(SplitButton),
            new FrameworkPropertyMetadata(4.0));

        /// <summary>
        /// Get or set how wide the separator bar should be. A value of 0 will make it invisible.
        /// </summary>
        public double SeparatorThickness { get => (double)GetValue(SeparatorThicknessProperty); set => SetValue(SeparatorThicknessProperty, value); }

        /// <summary>The backing dependency property for <see cref="SeparatorThickness"/>. See the related property for details.</summary>
        public static DependencyProperty SeparatorThicknessProperty
            = DependencyProperty.Register("SeparatorThickness", typeof(double), typeof(SplitButton),
            new FrameworkPropertyMetadata(1.0));

        // TODO: fix by also linking width/height to this value as well

        #endregion

        #endregion

        #region Menu

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register("Menu", typeof(ContextMenu), typeof(SplitButton),
            new PropertyMetadata(null));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#if NETCOREAPP
        /// <summary>
        /// Get or set the menu that appears when the button is clicked.
        /// </summary>
        [Category("Common")]
        public ContextMenu? Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// This event is raised when this SplitButtons's menu is closed.
        /// </summary>
        public EventHandler? MenuClosed;
#else
        /// <summary>
        /// Get or set the menu that appears when the button is clicked.
        /// </summary>
        [Category("Common")]
        public ContextMenu Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// This event is raised when this SplitButtons's menu is closed.
        /// </summary>
        public EventHandler MenuClosed;
#endif

        /// <summary>
        /// Get or set if the menu should close automatically. Remember to set the <c>StaysOpenOnClick</c> property for child menu items as well.
        /// </summary>
        [Category("Common")]
        public bool StaysOpen
        {
            get
            {
                if (Menu != null) return Menu.StaysOpen;
                else return false;
            }
            set
            {
                if (Menu != null) Menu.StaysOpen = value;
            }
        }

        #endregion

        #region Placement

        /// <summary>
        /// Get or set the placement mode for the SplitButton's menu.
        /// </summary>
        public PlacementMode MenuPlacement { get => (PlacementMode)GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static DependencyProperty MenuPlacementProperty
            = DependencyProperty.Register("MenuPlacement", typeof(PlacementMode), typeof(SplitButton),
            new FrameworkPropertyMetadata(PlacementMode.Bottom));


        /// <summary>
        /// Get or set the placement target for the SplitButton's menu. Set to <c>null</c> to set the target to this SplitButton.
        /// </summary>
#if NETCOREAPP
        public UIElement? MenuPlacementTarget { get => (UIElement)GetValue(MenuPlacementTargetProperty); set => SetValue(MenuPlacementTargetProperty, value); }
#else
        public UIElement MenuPlacementTarget { get => (UIElement)GetValue(MenuPlacementTargetProperty); set => SetValue(MenuPlacementTargetProperty, value); }
#endif

        /// <summary>The backing dependency property for <see cref="MenuPlacementTarget"/>. See the related property for details.</summary>
        public static DependencyProperty MenuPlacementTargetProperty
            = DependencyProperty.Register("MenuPlacementTarget", typeof(UIElement), typeof(SplitButton),
            new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Get or set the placement rectangle for the SplitButton's menu. This sets the area relative to the button that the menu is positioned.
        /// </summary>
        public Rect MenuPlacementRectangle { get => (Rect)GetValue(MenuPlacementRectangleProperty); set => SetValue(MenuPlacementRectangleProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacementRectangle"/>. See the related property for details.</summary>
        public static DependencyProperty MenuPlacementRectangleProperty
            = DependencyProperty.Register("MenuPlacementRectangle", typeof(Rect), typeof(SplitButton),
            new FrameworkPropertyMetadata(Rect.Empty));

        #endregion

        /// <summary>
        /// Get or set where the menu button should be placed in relation to the main button. Default is <c>Right</c>.
        /// </summary>
        public PlacementDirection MenuButtonPlacement { get => (PlacementDirection)GetValue(MenuButtonPlacementProperty); set => SetValue(MenuButtonPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuButtonPlacement"/>. See the related property for details.</summary>
        public static DependencyProperty MenuButtonPlacementProperty
            = DependencyProperty.Register("MenuButtonPlacement", typeof(PlacementDirection), typeof(SplitButton),
            new FrameworkPropertyMetadata(PlacementDirection.Right));

        #region Click Handling

        #region Routed Events

        /// <summary>
        /// The backing value for the <see cref="Click"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButton));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="MenuClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent MenuClickEvent = EventManager.RegisterRoutedEvent(
            "MenuClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButton));

        /// <summary>
        /// Raised when the user clicks on the menu button (not the main button), via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler MenuClick
        {
            add { AddHandler(MenuClickEvent, value); }
            remove { RemoveHandler(MenuClickEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="RightClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            "RightClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButton));

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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected static readonly DependencyPropertyKey IsMouseDownPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsMouseDown",
            typeof(bool), typeof(SplitButton), new FrameworkPropertyMetadata(false));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get if there is a mouse button currently being pressed, while the mouse cursor is over this control.
        /// </summary>
        public static readonly DependencyProperty IsMouseDownProperty = IsMouseDownPropertyKey.DependencyProperty;

        /// <summary>
        /// Set the IsMouseDown property for a SplitButton.
        /// </summary>
        /// <param name="obj">The SplitButton to apply the property change to.</param>
        /// <param name="value">The new value to set for the property.</param>
        protected static void SetIsMouseDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseDownPropertyKey, value);
        }

        /// <summary>
        /// Get the IsMouseDown property for a SplitButton.
        /// </summary>
        /// <param name="obj">The SplitButton to get the property value from.</param>
        public static bool GetIsMouseDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDownProperty);
        }

        #endregion

        #region Variables/Properties

        //bool sel = false;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(SplitButton),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Perform an action when a property of an object has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool se)
            {
                if (d is SplitButton f)
                {
                    f.IsSelectedChanged?.Invoke(d, e);
                    //f.ApplyColorScheme(f.ColorScheme, tb, f.UseAccentColors);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this button is selected. This property (combined with <c>SelectOnClick</c>) allows the button to function like a ToggleButton.
        /// </summary>
        /// <remarks>
        /// A selected button will have a slightly different visual appearance to differentiate it as being selected. This will include, by default, the border being a bit thicker.
        /// This can be changed via the <see cref="BorderSelectionThickness"/> property. You can also directly edit the brushes used via the <see cref="SelectedBrush"/> and
        /// <see cref="BorderSelectedBrush"/> properties.
        /// </remarks>
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty SelectOnClickProperty = DependencyProperty.Register(
            "SelectOnClick", typeof(bool), typeof(SplitButton),
            new PropertyMetadata(false));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.
        /// </summary>
        /// <remarks>
        /// While SelectOnClick is true, the button will toggle between <see cref="IsSelected"/> being true and false (similar to a ToggleButton). A selected button will, by default, have some visual
        /// differences to help make it look distinct from unselected buttons. The button's Click event will still be raised while this property is set to <c>true</c>, but the event occurs after the
        /// IsSelected property has already changed. While you could use the Click event to check when the button's IsSelected property is changed, it is better to use the IsSelectedChanged event,
        /// in case of situations where IsSelected is changed via methods other than clicking, such as programmatically or via WPF binding.
        /// </remarks>
        [Category("Common")]
        public bool SelectOnClick
        {
            get => (bool)GetValue(SelectOnClickProperty);
            set => SetValue(SelectOnClickProperty, value);
        }

        #endregion

        // If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        void PerformRightClick()
        {
            RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
            RaiseEvent(rre);
        }

        /// <summary>
        /// Perform a click pn the main button programmatically. The button responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            OnClick();
        }

        /// <summary>
        /// Defines the actions the button performs when it is clicked.
        /// </summary>
        protected void OnClick()
        {
            if (SelectOnClick)
            {
                IsSelected = !IsSelected;
            }

            RoutedEventArgs rre = new RoutedEventArgs(ClickEvent);
            RaiseEvent(rre);
        }

        #endregion

        #region Template IO
        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();

            if (itemsLoaded)
            {
                btnMain.Click += btnMain_Click;
                btnMenu.Click += btnMenu_Click;

                btnMain.RightClick += btnMain_RightClick;
                btnMenu.RightClick += btnMenu_RightClick;
            }
        }

        private void btnMenu_RightClick(object sender, RoutedEventArgs e)
        {
            PerformRightClick();
        }

        private void btnMain_RightClick(object sender, RoutedEventArgs e)
        {
            PerformRightClick();
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            if (Menu != null)
            {
                Menu.Placement = MenuPlacement;
                Menu.PlacementTarget = MenuPlacementTarget ?? this;
                Menu.PlacementRectangle = MenuPlacementRectangle;
                Menu.HorizontalOffset = 0;
                Menu.VerticalOffset = -1;
                Menu.IsOpen = true;
                Menu.Closed += Menu_Closed;
            }

            RoutedEventArgs rre = new RoutedEventArgs(MenuClickEvent);
            RaiseEvent(rre);
        }

        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            OnClick();
        }

        bool itemsLoaded = false;

        //bool _internalAction = false;

#if NETCOREAPP
        FlatButton? btnMain = null;
        FlatButton? btnMenu = null;
#else
        FlatButton btnMain = null;
        FlatButton btnMenu = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                btnMain = (FlatButton)GetTemplateChild("PART_Main");
                btnMenu = (FlatButton)GetTemplateChild("PART_Menu");

                if (btnMain != null && btnMenu != null)
                {
                    itemsLoaded = true;
                }
            }
        }
        #endregion

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

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            MenuClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
