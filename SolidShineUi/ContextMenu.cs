using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using SolidShineUi.Utils;

namespace SolidShineUi
{
    /// <summary>
    /// A popup menu that can display items specific to the context it's invoked by, with a visual style that matches the rest of the Solid Shine UI controls.
    /// </summary>
    [Localizability(LocalizationCategory.Menu)]
    public class ContextMenu : System.Windows.Controls.ContextMenu
    {

        // this class inherits all the logic from the WPF standard ContextMenu control
        // the only differences is the addition of various brush properties and Color Scheme integration
        // as well as a custom theme defined in Generic.xaml

        static ContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(typeof(ContextMenu)));
        }

        #region ColorScheme
        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        /// <summary>The backing dependency property for <see cref="ColorScheme"/>. See that related property for details.</summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(ContextMenu),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

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
            if (d is ContextMenu s)
            {
                s.ColorSchemeChanged?.Invoke(d, e);
                s.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        [Category("Appearance")]
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
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            Background = cs.MainColor.ToBrush();
            MenuBackground = cs.LightBackgroundColor.ToBrush();
            DisabledBrush = cs.DarkDisabledColor.ToBrush();
            BorderBrush = cs.BorderColor.ToBrush();
            HighlightBrush = cs.ThirdHighlightColor.ToBrush();
            HighlightSubitemBrush = cs.ThirdHighlightColor.ToBrush();
            CheckedBrush = cs.SecondaryColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();

            if (cs.IsHighContrast)
            {
                DisabledBrush = cs.LightDisabledColor.ToBrush();
                CheckedBrush = cs.HighlightColor.ToBrush();
            }
            else if (cs.MenusUseAccent)
            {
                HighlightSubitemBrush = cs.AccentThirdHighlightColor.ToBrush();
                CheckedBrush = cs.AccentSecondaryColor.ToBrush();
            }
        }
        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the menu's drop-down area. This is different from the Background brush, which is not used in a ContextMenu.
        /// </summary>
        [Category("Brushes")]
        public Brush MenuBackground
        {
            get
            {
                return (Brush)GetValue(MenuBackgroundProperty);
            }
            set
            {
                SetValue(MenuBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the foreground/text of a child menu item when it is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush DisabledBrush
        {
            get
            {
                return (Brush)GetValue(DisabledBrushProperty);
            }
            set
            {
                SetValue(DisabledBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the top-level menu items in the menu bar. This is not used in a ContextMenu; instead look at the HighlightSubitemBrush property.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get
            {
                return (Brush)GetValue(HighlightBrushProperty);
            }
            set
            {
                SetValue(HighlightBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for menu items that aren't top-level on the menu bar, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightSubitemBrush
        {
            get
            {
                return (Brush)GetValue(HighlightSubitemBrushProperty);
            }
            set
            {
                SetValue(HighlightSubitemBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush to use for the check highlight for checked menu items.
        /// </summary>
        [Category("Brushes")]
        public Brush CheckedBrush
        {
            get
            {
                return (Brush)GetValue(CheckedBrushProperty);
            }
            set
            {
                SetValue(CheckedBrushProperty, value);
            }
        }


        /// <summary>The backing dependency property for <see cref="MenuBackground"/>. See that related property for details.</summary>
        public static readonly DependencyProperty MenuBackgroundProperty = DependencyProperty.Register(
            nameof(MenuBackground), typeof(Brush), typeof(ContextMenu),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        /// <summary>The backing dependency property for <see cref="DisabledBrush"/>. See that related property for details.</summary>
        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            nameof(DisabledBrush), typeof(Brush), typeof(ContextMenu),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>The backing dependency property for <see cref="HighlightSubitemBrush"/>. See that related property for details.</summary>
        public static readonly DependencyProperty HighlightSubitemBrushProperty = DependencyProperty.Register(
            nameof(HighlightSubitemBrush), typeof(Brush), typeof(ContextMenu),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See that related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            nameof(HighlightBrush), typeof(Brush), typeof(ContextMenu),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>The backing dependency property for <see cref="CheckedBrush"/>. See that related property for details.</summary>
        public static readonly DependencyProperty CheckedBrushProperty = DependencyProperty.Register(
            nameof(CheckedBrush), typeof(Brush), typeof(ContextMenu),
            new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        #endregion

        #region Other Appearance Properties

        /// <summary>
        /// Get or set if the context menu has rounded corners. When true, each corner has a corner radius of 4.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set if the context menu has rounded corners. When true, each corner has a corner radius of 4.")]
        public bool RoundedCorners
        {
            get
            {
                return (bool)GetValue(RoundedCornersProperty);
            }
            set
            {
                SetValue(RoundedCornersProperty, value);
            }
        }

        /// <summary>
        /// Get or set if a vertical scroll bar is visible for the context menu.
        /// <para/>
        /// If the context menu is longer than the screen, the scroll bar can be used.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set if a vertical scroll bar is visible for the context menu.")]
        public System.Windows.Controls.ScrollBarVisibility VerticalScrollBarVisibility
        {
            get
            {
                return (System.Windows.Controls.ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            }
            set
            {
                SetValue(VerticalScrollBarVisibilityProperty, value);
            }
        }


        /// <summary>The backing dependency property for <see cref="RoundedCorners"/>. See that related property for details.</summary>
        public static readonly DependencyProperty RoundedCornersProperty = DependencyProperty.Register(
            nameof(RoundedCorners), typeof(bool), typeof(ContextMenu),
            new PropertyMetadata(false));

        /// <summary>The backing dependency property for <see cref="VerticalScrollBarVisibility"/>. See that related property for details.</summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(
            nameof(VerticalScrollBarVisibility), typeof(System.Windows.Controls.ScrollBarVisibility), typeof(ContextMenu),
            new PropertyMetadata(System.Windows.Controls.ScrollBarVisibility.Auto));

        #endregion

        #region SsuiTheme

        /// <summary>
        /// Get or set the theme to apply to the appearance of this control. This will bind the control's brushes to this theme.
        /// </summary>
        /// <remarks>
        /// This theme can be used to centrally apply a consistent appearance to all ThemedControls. Once a theme is set, the control's brushes are bound to the relevant
        /// values in this SsuiTheme, allowing you to update the control's appearance by changing the relevant values in the SsuiTheme. 
        /// This has the downside of overwriting any preset brush values set via XAML; to circumvent this, listen to the <see cref="SsuiThemeChanged"/> event and 
        /// then reapply the brushes in that event handler, or include that brush property's name in this control's <see cref="ThemeValueExclude"/>.
        /// <para/>
        /// This is set to null by default, so that brushes are not automatically bound to any SsuiTheme (allowing you to set the brushes via XAML).
        /// </remarks>
        [Category("Appearance")]
#if NETCOREAPP
        public SsuiTheme? SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#else
        public SsuiTheme SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#endif

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SsuiThemeProperty = ThemedControl.SsuiThemeProperty.AddOwner(typeof(ContextMenu),
            new FrameworkPropertyMetadata(OnSsuiThemeChanged));

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeChangedEvent = ThemedControl.SsuiThemeChangedEvent.AddOwner(typeof(ContextMenu));
        //EventManager.RegisterRoutedEvent(nameof(SsuiThemeChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ThemedContentControl));

        /// <summary>
        /// Raised after the <see cref="SsuiTheme"/> property has been changed and applied.
        /// </summary>
        public event RoutedEventHandler SsuiThemeChanged
        {
            add { AddHandler(SsuiThemeChangedEvent, value); }
            remove { RemoveHandler(SsuiThemeChangedEvent, value); }
        }

        private static void OnSsuiThemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ContextMenu tc)
            {
                tc.ApplyAndRaiseTheme();
            }
        }

        /// <summary>
        /// Update the control's appearance by using <see cref="ApplySsuiTheme(SsuiTheme)"/> and then raise the <see cref="SsuiThemeChanged"/> event.
        /// </summary>
        protected internal void ApplyAndRaiseTheme()
        {
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeChangedEvent, this);
            RaiseEvent(re);
        }

        #region UseLightBorder

        /// <summary>
        /// Get or set if the <see cref="SsuiTheme.LightBorderBrush"/> should be used for the border of this control, rather than the <see cref="SsuiTheme.BorderBrush"/>.
        /// </summary>
        public bool UseLightBorder { get => (bool)GetValue(UseLightBorderProperty); set => SetValue(UseLightBorderProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseLightBorder"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseLightBorderProperty = ThemedControl.UseLightBorderProperty.AddOwner(typeof(ContextMenu),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ContextMenu>((o) => o.OnUseLightBorderChange(o, e))));

        void OnUseLightBorderChange(object sender, DependencyPropertyChangedEventArgs _)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region UseAccentColors

        /// <summary>
        /// Get or set if an accent theme should be used rather than the standard theme for this control. The accent theme, when used in moderation,
        /// can be used to help particular controls stand out in the UI.
        /// </summary>
        /// <remarks>
        /// The value of the <see cref="SsuiTheme"/> property needs to be of an <see cref="SsuiAppTheme"/> type, or otherwise this property does nothing.
        /// This should already be taken care of if you're putting this control in a <see cref="ThemedWindow"/> and inheriting from that SsuiTheme property.
        /// </remarks>
        public bool UseAccentTheme { get => (bool)GetValue(UseAccentThemeProperty); set => SetValue(UseAccentThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseAccentTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseAccentThemeProperty = ThemedControl.UseAccentThemeProperty.AddOwner(typeof(ContextMenu),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ContextMenu>((o) => o.OnUseAccentColorsChange(o, e))));

        void OnUseAccentColorsChange(object sender, DependencyPropertyChangedEventArgs _)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region ThemeValueExclude

        /// <summary>
        /// Get or set the properties to not apply the theme values to when applying a SsuiTheme to this control.
        /// <para/>
        /// Use a comma-separated list for multiple properties; for each property, the property's value will not be changed when the SsuiTheme is applied.
        /// </summary>
        /// <remarks>
        /// This can be used when you have certain brushes or values set for a particular control and applying a SsuiTheme ends up resetting that value.
        /// If only one property's value needs to be kept unchanged, then just set this to the name of that property. If multiple properties need this,
        /// then provide this as a comma-separated list of all the properties' names in a single string (e.g., <c>"Background,Foreground,BorderBrush"</c>).
        /// When the control is applying a SsuiTheme (such as set via a property directly, or inheriting from a parent), the control will skip over setting
        /// properties that match one of the names on this list.
        /// <para/>
        /// Note that this functionality is designed for comparing dependency properties' names (<see cref="DependencyProperty.Name"/>) 
        /// to the list provided here. This will also likely not have any effect on any property that isn't a dependency property.
        /// </remarks>
        public string ThemeValueExclude { get => (string)GetValue(ThemeValueExcludeProperty); set => SetValue(ThemeValueExcludeProperty, value); }

        /// <summary>The backing dependency property for <see cref="ThemeValueExclude"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ThemeValueExcludeProperty = ThemedControl.ThemeValueExcludeProperty.AddOwner(typeof(ContextMenu));

        #endregion

        #region ApplySsuiTheme

        bool _skipReapply = false;

        /// <summary>
        /// Check the control's properties to make sure they match these values. If not, this will update the properties and then re-call the ApplySsuiTheme function.
        /// </summary>
        /// <param name="ssuiTheme">the inputted SsuiTheme value</param>
        /// <param name="useLightBorder">the inputted UseLightBorder value</param>
        /// <param name="useAccentTheme">the inputted UseAccentTheme value</param>
#if NETCOREAPP
        bool CheckAndUpdateProperties(SsuiTheme? ssuiTheme, bool useLightBorder, bool useAccentTheme)
#else
        bool CheckAndUpdateProperties(SsuiTheme ssuiTheme, bool useLightBorder, bool useAccentTheme)
#endif
        {
            _skipReapply = true;

            if (UseLightBorder != useLightBorder) UseLightBorder = useLightBorder;
            if (UseAccentTheme != useAccentTheme) UseAccentTheme = useAccentTheme;

            _skipReapply = false;

            if (ssuiTheme != SsuiTheme)
            {
                SsuiTheme = ssuiTheme;
                return false; // ApplySsuiTheme will be called again once the property is changed
            }

            return true;
        }

        /// <summary>
        /// Apply a <see cref="SsuiTheme"/> to this control. This applies a consistent appearance to this control and child controls.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        public void ApplySsuiTheme(SsuiTheme ssuiTheme)
        {
            ApplySsuiTheme(ssuiTheme, UseLightBorder, UseAccentTheme);
        }

        /// <summary>
        /// Apply a <see cref="SsuiTheme"/> to this control. This applies a consistent appearance to this control and child controls.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        /// <param name="useLightBorder">whether a light border color should be used in place of the standard one</param>
        /// <param name="useAccentTheme">
        /// whether the accent theme should be used in place of the standard one; for this to apply, <paramref name="ssuiTheme"/>
        /// should be of type <see cref="SsuiAppTheme"/>
        /// </param>
#if NETCOREAPP
        public void ApplySsuiTheme(SsuiTheme? ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
#else
        public void ApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
#endif
        {
            if (!CheckAndUpdateProperties(ssuiTheme, useLightBorder, useAccentTheme)) return;

            if (ssuiTheme != null) OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);
            // TODO: if SsuiTheme is null, instead call a ClearBindings method that prompts the control to instead clear all of the bindings to the SsuiTheme

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeAppliedEvent, this);
            RaiseEvent(re);
        }

        /// <summary>
        /// Raised when the SsuiTheme value is changed, or when <see cref="ApplySsuiTheme(SsuiTheme, bool, bool)"/> is called.
        /// Inheriting controls should override this to update their brushes to reflect this new theme.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        /// <param name="useLightBorder">whether a light border color should be used in place of the standard one</param>
        /// <param name="useAccentTheme">
        /// whether the accent theme should be used in place of the standard one; for this to apply, <paramref name="ssuiTheme"/>
        /// should be of type <see cref="SsuiAppTheme"/>
        /// </param>
        /// <remarks>
        /// The base method will by default set the <see cref="System.Windows.Controls.Control.BorderBrush"/> value to match the theme; if a custom
        /// border brush is needed, then update it again after this is called.
        /// </remarks>
        protected virtual void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {

            if (ssuiTheme is SsuiAppTheme sat)
            {
                if (useAccentTheme)
                {
                    ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, sat.AccentTheme);
                    ApplyTheme(sat.AccentTheme);
                }
                else
                {
                    ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty);
                    ApplyTheme(ssuiTheme);
                }

                if (useAccentTheme)
                {
                    ApplyThemeBinding(BackgroundProperty, SsuiTheme.BaseBackgroundProperty, sat.AccentTheme);
                }
                else
                {
                    ApplyThemeBinding(BackgroundProperty, SsuiTheme.BaseBackgroundProperty, sat);
                }

                if (sat.UseSubitemThemeWithMenus)
                {
                    ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty, sat.SubitemTheme);
                    ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, sat.SubitemTheme);
                }
                else if (useAccentTheme)
                {
                    ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty, sat.AccentTheme);
                    ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, sat.AccentTheme);
                }
                else
                {
                    ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty);
                    ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty);
                }
            }
            else
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty);

                ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty);
                ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty);
                ApplyThemeBinding(BackgroundProperty, SsuiTheme.BaseBackgroundProperty);
                ApplyTheme(ssuiTheme);
            }

            void ApplyTheme(SsuiTheme theme)
            {
                ApplyThemeBinding(MenuBackgroundProperty, SsuiTheme.PanelBackgroundProperty, theme);
                ApplyThemeBinding(HighlightBrushProperty, SsuiTheme.HighlightBrushProperty, theme);
                ApplyThemeBinding(DisabledBrushProperty, SsuiTheme.DisabledForegroundProperty, theme);
                ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, theme);
            }
        }

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="brushProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or a class that inherits from SsuiTheme
        /// </exception>
        /// <remarks>
        /// If <see cref="SsuiTheme"/> is <c>null</c>, then instead the binding is cleared, and <c>null</c> is returned.
        /// </remarks>
#if NETCOREAPP
        protected BindingExpressionBase? ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty)
#else
        protected BindingExpressionBase ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty)
#endif
        {
            return ApplyThemeBinding(brushProperty, ssuiThemeProperty, SsuiTheme);
        }

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="brushProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the name of the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <param name="source">the specific SsuiTheme object to bind to (such as <see cref="SsuiAppTheme"/>'s AccentTheme or SubitemTheme)</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or a class that inherits from SsuiTheme
        /// </exception>
        /// <remarks>
        /// If <paramref name="source"/> is <c>null</c>, then instead the binding is cleared, and <c>null</c> is returned.
        /// </remarks>
#if NETCOREAPP
        protected BindingExpressionBase? ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty, SsuiTheme? source)
#else
        protected BindingExpressionBase ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty, SsuiTheme source)
#endif
        {
            // if ThemeValueExclude includes this property's name (as part of a comma-separated list), then we'll not do anything here
            if (!string.IsNullOrEmpty(ThemeValueExclude))
            {
                if (ThemeValueExclude.Split(',').Contains(brushProperty.Name)) return null;
            }

            // if the theme value provided is null, then we'll just undo any binding that exists (which should undo it to the previous SsuiTheme)
            if (source == null)
            {
                BindingOperations.ClearBinding(this, brushProperty);
                return null;
            }
            else
            {
                return SetBinding(brushProperty, SsuiTheme.CreateBinding(ssuiThemeProperty, source));
            }
        }

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeAppliedEvent = ThemedControl.SsuiThemeAppliedEvent.AddOwner(typeof(ContextMenu));
        // EventManager.RegisterRoutedEvent(nameof(SsuiThemeApplied), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ThemedContentControl));

        /// <summary>
        /// Raised after <see cref="ApplySsuiTheme(SsuiTheme, bool, bool)"/> has completed, and the theme has been applied to the control.
        /// </summary>
        /// <remarks>
        /// If creating your own control that inherits from this class, use this event or override the <see cref="OnApplySsuiTheme(SsuiTheme, bool, bool)"/>
        /// method to apply any updated brushes from the <see cref="SsuiTheme"/>.
        /// <para/>
        /// This event precedes the <see cref="SsuiThemeChanged"/> event.
        /// </remarks>
        public event RoutedEventHandler SsuiThemeApplied
        {
            add { AddHandler(SsuiThemeAppliedEvent, value); }
            remove { RemoveHandler(SsuiThemeAppliedEvent, value); }
        }

        #endregion

        #endregion
    }
}
