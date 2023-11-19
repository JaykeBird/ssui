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
    public class SplitButton : ContentControl, IClickSelectableControl
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

            Menu?.ApplyColorScheme(cs);
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

        #endregion

        #endregion

        #region Menu

        /// <summary>
        /// The backing dependency property for <see cref="Menu"/>. Please see the related property for more details.
        /// </summary>
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register("Menu", typeof(ContextMenu), typeof(SplitButton),
            new PropertyMetadata(null));

#if NETCOREAPP
        /// <summary>
        /// Get or set the menu that appears when the menu button is clicked.
        /// </summary>
        [Category("Common")]
        public ContextMenu? Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// This event is raised when this SplitButton's menu is closed.
        /// </summary>
        public EventHandler? MenuClosed;
#else
        /// <summary>
        /// Get or set the menu that appears when the menu button is clicked.
        /// </summary>
        [Category("Common")]
        public ContextMenu Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// This event is raised when this SplitButton's menu is closed.
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

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            MenuClosed?.Invoke(this, EventArgs.Empty);
        }

        #region Placement

        /// <summary>
        /// Get or set the placement mode for the SplitButton's menu. Default is <c>Bottom</c>.
        /// </summary>
        public PlacementMode MenuPlacement { get => (PlacementMode)GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static DependencyProperty MenuPlacementProperty
            = DependencyProperty.Register("MenuPlacement", typeof(PlacementMode), typeof(SplitButton),
            new FrameworkPropertyMetadata(PlacementMode.Bottom));


        /// <summary>
        /// Get or set the placement target for the SplitButton's menu. Set to <c>null</c> to set the target to this SplitButton. Default is <c>null</c>.
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

        #endregion

        #region Button Handling

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

        #region Menu Button Layout

        /// <summary>
        /// Get or set where the menu button should be placed in relation to the main button. Default is <c>Right</c>.
        /// </summary>
        /// <remarks>
        /// If you want to set this value to <c>Top</c> or <c>Bottom</c>, also consider increasing the height of the button or decreasing the value of <see cref="MenuButtonSize"/>.
        /// If you set this value to <c>Hidden</c>, the menu button is entirely hidden (and no longer accessible via the mouse).
        /// </remarks>
        public PlacementDirection MenuButtonPlacement { get => (PlacementDirection)GetValue(MenuButtonPlacementProperty); set => SetValue(MenuButtonPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuButtonPlacement"/>. See the related property for details.</summary>
        public static DependencyProperty MenuButtonPlacementProperty
            = DependencyProperty.Register("MenuButtonPlacement", typeof(PlacementDirection), typeof(SplitButton),
            new FrameworkPropertyMetadata(PlacementDirection.Right));

        /// <summary>
        /// Get or set the width or height of the menu button. Larger values make the button wider or taller, and takes up a larger portion of the overall control. Default is <c>20</c>.
        /// </summary>
        /// <remarks>
        /// When <see cref="MenuButtonPlacement"/> is set to <c>Left</c> or <c>Right</c>, this affects the width of the menu button.
        /// When it is set to <c>Top</c> or <c>Bottom</c>, this affects the height of the menu button. When it is set to <c>Hidden</c>, this has no effect.
        /// </remarks>
        public double MenuButtonSize { get => (double)GetValue(MenuButtonSizeProperty); set => SetValue(MenuButtonSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuButtonSize"/>. See the related property for details.</summary>
        public static DependencyProperty MenuButtonSizeProperty
            = DependencyProperty.Register("MenuButtonSize", typeof(double), typeof(SplitButton),
            new FrameworkPropertyMetadata(20.0));

        #endregion

        #endregion

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
        bool _runSelChangeEvent = true;

        /// <summary>
        /// The backing dependency property for <see cref="IsSelected"/>. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(SplitButton),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool se)
            {
                bool old = Convert.ToBoolean(e.OldValue);

                if (d is SplitButton f)
                {
                    if (f._runSelChangeEvent)
                    {
                        ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, se, SelectionChangeTrigger.CodeUnknown, null);
                        f.RaiseEvent(re);
                    }
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
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// The backing dependency property for <see cref="SelectOnClick"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty SelectOnClickProperty = DependencyProperty.Register(
            "SelectOnClick", typeof(bool), typeof(SplitButton),
            new PropertyMetadata(false));

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

        /// <summary>
        /// The backing value for the <see cref="IsSelectedChanged"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent(
            "IsSelectedChanged", RoutingStrategy.Bubble, typeof(ItemSelectionChangedEventHandler), typeof(SelectableUserControl));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event ItemSelectionChangedEventHandler IsSelectedChanged
        {
            add { AddHandler(IsSelectedChangedEvent, value); }
            remove { RemoveHandler(IsSelectedChangedEvent, value); }
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

            ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, value, triggerMethod, triggerSource);
            RaiseEvent(re);
        }

        #endregion

        // If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        void PerformRightClick()
        {
            RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
            RaiseEvent(rre);
        }

        /// <summary>
        /// Perform a click on the main button programmatically. The button responds the same way as if it was clicked by the user.
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
                SetIsSelectedWithSource(!IsSelected, SelectionChangeTrigger.ControlClick, this);
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
                // the itemsLoaded value will only be true if both controls are not null
#if NETCOREAPP
                btnMain!.Click += btnMain_Click;
                btnMenu!.Click += btnMenu_Click;

                btnMain.RightClick += btnMain_RightClick;
                btnMenu.RightClick += btnMenu_RightClick;
#else
                btnMain.Click += btnMain_Click;
                btnMenu.Click += btnMenu_Click;

                btnMain.RightClick += btnMain_RightClick;
                btnMenu.RightClick += btnMenu_RightClick;
#endif
            }
        }

        bool itemsLoaded = false;

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

    }
}
