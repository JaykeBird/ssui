using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A menu control that can display and organize commands within various drop-down elements. This differs from the WPF menu with a visual style that matches other Solid Shine UI controls.
    /// </summary>
    public class Menu : Avalonia.Controls.Menu
    {

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(SsuiTheme):
                    OnSsuiThemeChanged(change);
                    break;
                case nameof(UseLightBorder):
                    OnUseLightBorderChange(change);
                    break;
                case nameof(UseAccentTheme):
                    OnUseAccentThemeChange(change);
                    break;
                case nameof(ColorScheme):
                    ApplyColorScheme(change.GetNewValue<ColorScheme>());
                    ColorSchemeChanged?.Invoke(this, change);
                    break;
            }
        }

        #region ColorScheme

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        [Category("Appearance")]
        public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ColorScheme> ColorSchemeProperty
            = AvaloniaProperty.Register<Menu, ColorScheme>(nameof(ColorScheme), new ColorScheme());

        /// <summary>
        /// Raised when the <see cref="ColorScheme"/> property has changed.
        /// </summary>
        public event EventHandler<AvaloniaPropertyChangedEventArgs>? ColorSchemeChanged;

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
            PopupBorderBrush = cs.BorderColor.ToBrush();
            HighlightBrush = cs.ThirdHighlightColor.ToBrush();
            HighlightSubitemBrush = cs.ThirdHighlightColor.ToBrush();
            ClickBrush = cs.SecondHighlightColor.ToBrush();
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
        /// Get or set the brush used for the background of the menu's drop-down area. This is different from the Background brush, which set the background of the top menu bar.
        /// </summary>
        [Category("Brushes")]
        public IBrush? MenuBackground { get => GetValue(MenuBackgroundProperty); set => SetValue(MenuBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> MenuBackgroundProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(MenuBackground), new SolidColorBrush(Colors.White));

        /// <summary>
        /// Get or set the brush used for the foreground of menu items that are disabled.
        /// </summary>
        [Category("Brushes")]
        public IBrush? DisabledBrush { get => GetValue(DisabledBrushProperty); set => SetValue(DisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> DisabledBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(DisabledBrush), new SolidColorBrush(Colors.Gray));

        /// <summary>
        /// Get or set the brush used for the border of the menu's drop-down area.
        /// </summary>
        [Category("Brushes")]
        public IBrush? PopupBorderBrush { get => GetValue(PopupBorderBrushProperty); set => SetValue(PopupBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="PopupBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> PopupBorderBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(PopupBorderBrush), new SolidColorBrush(Colors.Gray));

        /// <summary>
        /// Get or set the brush used for menu items that aren't top-level on the menu bar, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightSubitemBrush { get => GetValue(HighlightSubitemBrushProperty); set => SetValue(HighlightSubitemBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightSubitemBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightSubitemBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(HighlightSubitemBrush), new SolidColorBrush(Colors.LightGray));

        /// <summary>
        /// Get or set the brush used for the top-level menu items in the menu bar, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(HighlightBrush), new SolidColorBrush(Colors.LightGray));

        /// <summary>
        /// Get or set the brush to use for the check highlight for checked menu items.
        /// </summary>
        [Category("Brushes")]
        public IBrush? CheckedBrush { get => GetValue(CheckedBrushProperty); set => SetValue(CheckedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckedBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(CheckedBrush), new SolidColorBrush(Colors.LightGray));

        /// <summary>
        /// Get or set the brush to use for the check highlight for checked menu items.
        /// </summary>
        [Category("Brushes")]
        public IBrush? ClickBrush { get => GetValue(ClickBrushProperty); set => SetValue(ClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ClickBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(ClickBrush), new SolidColorBrush(Colors.Gainsboro));

        /// <summary>
        /// Get or set the brush used for the foreground elements of the top-level menu items in the menu bar, 
        /// when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightForegroundBrush { get => GetValue(HighlightForegroundBrushProperty); set => SetValue(HighlightForegroundBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightForegroundBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightForegroundBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(HighlightForegroundBrush), new SolidColorBrush(Colors.Black));

        /// <summary>
        /// Get or set the brush used for the foreground elements of menu items that aren't top-level on the menu bar, 
        /// when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightSubitemForegroundBrush { get => GetValue(HighlightSubitemForegroundBrushProperty); set => SetValue(HighlightSubitemForegroundBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightSubitemForegroundBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightSubitemForegroundBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(HighlightSubitemForegroundBrush), new SolidColorBrush(Colors.Black));

        /// <summary>
        /// Get or set the brush used for the border around the top-level menu items in the menu bar, 
        /// when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightBorderBrush { get => GetValue(HighlightBorderBrushProperty); set => SetValue(HighlightBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBorderBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(HighlightBorderBrush), new SolidColorBrush(Colors.Transparent));

        /// <summary>
        /// Get or set the brush used for the border around the menu items that aren't top-level on the menu bar, 
        /// when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightSubitemBorderBrush { get => GetValue(HighlightSubitemBorderBrushProperty); set => SetValue(HighlightSubitemBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightSubitemBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightSubitemBorderBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(HighlightSubitemBorderBrush), new SolidColorBrush(Colors.Transparent));

        /// <summary>
        /// Get or set the brush used for the border of the menu's drop-down area.
        /// </summary>
        [Category("Brushes")]
        public IBrush? MenuBorderBrush { get => GetValue(MenuBorderBrushProperty); set => SetValue(MenuBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> MenuBorderBrushProperty
            = AvaloniaProperty.Register<Menu, IBrush?>(nameof(MenuBorderBrush), new SolidColorBrush(Colors.Black));

        #endregion

        #region Corner Radius

        /// <summary>
        /// Get or set the corner radius to apply to the opened menu drop-downs.
        /// </summary>
        public CornerRadius MenuCornerRadius { get => GetValue(MenuCornerRadiusProperty); set => SetValue(MenuCornerRadiusProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuCornerRadius"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CornerRadius> MenuCornerRadiusProperty
            = AvaloniaProperty.Register<Menu, CornerRadius>(nameof(MenuCornerRadius), new CornerRadius(0));

        /// <summary>
        /// Get or set the corner radius to use for the corners of the menu items within this menu. A value higher than 0 for any corner will make that corner a rounded corner.
        /// </summary>
        public CornerRadius MenuItemCornerRadius { get => GetValue(MenuItemCornerRadiusProperty); set => SetValue(MenuItemCornerRadiusProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuItemCornerRadius"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CornerRadius> MenuItemCornerRadiusProperty
            = AvaloniaProperty.Register<Menu, CornerRadius>(nameof(MenuItemCornerRadius), new CornerRadius(0));

        #endregion

        #region SsuiTheme

        #region SsuiTheme Property

        /// <summary>
        /// Get or set the theme to apply to the appearance of this control. This will bind the control's brushes to this theme.
        /// </summary>
        /// <remarks>
        /// This theme can be used to centrally apply a consistent appearance to all Menus. Once a theme is set, the control's brushes are bound to the relevant
        /// values in this SsuiTheme, allowing you to update the control's appearance by changing the relevant values in the SsuiTheme. 
        /// This does not have the issue present in the WPF version where setting a theme would overwrite your locally-set brush values.
        /// Instead, setting a theme will operate similarly to applying a style to this control.
        /// <para/>
        /// This is set to null by default, so that brushes are not automatically bound to any SsuiTheme.
        /// </remarks>
        [Category("Appearance")]
        [Description("Get or set the theme to apply to the appearance of this control.")]
        public SsuiTheme? SsuiTheme { get => GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }

        /// <summary>The backing styled property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<SsuiTheme?> SsuiThemeProperty
            = ThemedControl.SsuiThemeProperty.AddOwner<Menu>();

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent<RoutedPropertyChangedEventArgs> SsuiThemeChangedEvent =
            RoutedEvent.Register<Menu, RoutedPropertyChangedEventArgs>(nameof(SsuiThemeChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the <see cref="SsuiTheme"/> property is changed.
        /// </summary>
        public event EventHandler<RoutedPropertyChangedEventArgs>? SsuiThemeChanged
        {
            add => AddHandler(SsuiThemeChangedEvent, value);
            remove => RemoveHandler(SsuiThemeChangedEvent, value);
        }

        /// <summary>
        /// Raised when the <see cref="SsuiTheme"/> property is changed.
        /// </summary>
        /// <param name="e">the event argument object containing information about the change</param>
        protected virtual void OnSsuiThemeChanged(AvaloniaPropertyChangedEventArgs e)
        {
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);

            RaiseEvent(new RoutedPropertyChangedEventArgs(SsuiThemeChangedEvent, e));
        }

        #endregion

        #region UseLightBorder

        /// <summary>
        /// Get or set if the <see cref="SsuiTheme.LightBorderBrush"/> should be used for the border of this control when applying a SsuiTheme, rather than the <see cref="SsuiTheme.BorderBrush"/>.
        /// </summary>
        [Category("Appearance"), DefaultValue(false)]
        [Description("Get or set if the LightBorderBrush should be used for the border of this control, when applying a SsuiTheme.")]
        public bool UseLightBorder { get => GetValue(UseLightBorderProperty); set => SetValue(UseLightBorderProperty, value); }

        /// <summary>The backing styled property for <see cref="UseLightBorder"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseLightBorderProperty
            = ThemedControl.UseLightBorderProperty.AddOwner<Menu>();

        private void OnUseLightBorderChange(AvaloniaPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region UseAccentTheme

        /// <summary>
        /// Get or set if an accent theme should be used rather than the standard theme for this control. The accent theme, when used in moderation,
        /// can be used to help particular controls stand out in the UI.
        /// </summary>
        /// <remarks>
        /// The value of the <see cref="SsuiTheme"/> property needs to be of an <see cref="SsuiAppTheme"/> type, or otherwise this property does nothing.
        /// </remarks>
        [Category("Appearance"), DefaultValue(false)]
        [Description("Get or set if the SsuiTheme's accent theme should be used rather than the standard theme for this control.")]
        public bool UseAccentTheme { get => GetValue(UseAccentThemeProperty); set => SetValue(UseAccentThemeProperty, value); }

        /// <summary>The backing styled property for <see cref="UseAccentTheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseAccentThemeProperty
            = ThemedControl.UseAccentThemeProperty.AddOwner<Menu>();

        private void OnUseAccentThemeChange(AvaloniaPropertyChangedEventArgs e)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region UseTitleBarThemeBrush

        /// <summary>
        /// Get or set if the <see cref="SsuiAppTheme.WindowTitleBackground"/> brush should be used for the background of the menu,
        /// when the <see cref="SsuiTheme"/> property is set and brushes applied.
        /// </summary>
        /// <remarks>
        /// This is set to <c>true</c> by default; this has the effect of making the menu appear to blend in with the title bar, if it's put
        /// at the top of the window next to the title bar. Sometimes this effect is undesirable, so set this to <c>false</c> to disable
        /// this effect, and the background of the menu will instead use <see cref="SsuiTheme.BaseBackground"/> when the theme is applied.
        /// <para/>
        /// For this to work, the <see cref="SsuiTheme"/> property needs to be set to an object of type <see cref="SsuiAppTheme"/>,
        /// and its <see cref="SsuiAppTheme.AllowTitleBarBrushWithMenus"/> property needs to also be set to <c>true</c>.
        /// If this menu is being placed in a <see cref="ThemedWindow"/>, then setting the window's
        /// <c>SsuiTheme</c> property should suffice, as then this control will inherit from that window's property.
        /// <para/>
        /// If you do not use <see cref="SsuiTheme"/> or use a SSUI-themed window, then this property has no effect.
        /// </remarks>
        public bool UseTitleBarThemeBrush { get => GetValue(UseTitleBarThemeBrushProperty); set => SetValue(UseTitleBarThemeBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="UseTitleBarThemeBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseTitleBarThemeBrushProperty
            = AvaloniaProperty.Register<Menu, bool>(nameof(UseTitleBarThemeBrush), false);

        #endregion

        #region ApplySsuiTheme

        bool _skipReapply = false;

        /// <summary>
        /// Check the control's properties to make sure they match these values. If not, this will update the properties and then re-call the ApplySsuiTheme function.
        /// </summary>
        /// <param name="ssuiTheme">the inputted SsuiTheme value</param>
        /// <param name="useLightBorder">the inputted UseLightBorder value</param>
        /// <param name="useAccentTheme">the inputted UseAccentTheme value</param>
        bool CheckAndUpdateProperties(SsuiTheme? ssuiTheme, bool useLightBorder, bool useAccentTheme)
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
        public void ApplySsuiTheme(SsuiTheme? ssuiTheme)
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
        public void ApplySsuiTheme(SsuiTheme? ssuiTheme, bool useLightBorder, bool useAccentTheme)
        {
            if (!CheckAndUpdateProperties(ssuiTheme, useLightBorder, useAccentTheme)) return;

            if (ssuiTheme != null) OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            RaiseEvent(new RoutedEventArgs(SsuiThemeAppliedEvent));
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
        protected virtual void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            if (ssuiTheme is SsuiAppTheme sat)
            {
                if (UseTitleBarThemeBrush && sat.AllowTitleBarBrushWithMenus)
                {
                    ApplyThemeBinding(BackgroundProperty, SsuiAppTheme.WindowTitleBackgroundProperty, sat);
                }
                else if (useAccentTheme)
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
                    ApplyThemeBinding(HighlightForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty, sat.SubitemTheme);
                    ApplyThemeBinding(HighlightSubitemForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty, sat.SubitemTheme);
                    // ApplyThemeBinding(HighlightSubitemBorderBrushProperty, SsuiTheme.HighlightBorderBrushProperty, sat.SubitemTheme);
                    ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, sat.SubitemTheme);

                    ApplyTheme(useAccentTheme ? sat.AccentTheme : ssuiTheme);
                }
                else if (useAccentTheme)
                {
                    ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty, sat.AccentTheme);
                    ApplyThemeBinding(HighlightForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty, sat.AccentTheme);
                    ApplyThemeBinding(HighlightSubitemForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty, sat.AccentTheme);
                    // ApplyThemeBinding(HighlightSubitemBorderBrushProperty, SsuiTheme.HighlightBorderBrushProperty, sat.AccentTheme);
                    ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, sat.AccentTheme);

                    ApplyTheme(sat.AccentTheme);
                }
                else
                {
                    ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty);
                    ApplyThemeBinding(HighlightForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty);
                    ApplyThemeBinding(HighlightSubitemForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty);
                    // ApplyThemeBinding(HighlightSubitemBorderBrushProperty, SsuiTheme.HighlightBorderBrushProperty);
                    ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty);

                    ApplyTheme(ssuiTheme);
                }
            }
            else
            {
                ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty);
                ApplyThemeBinding(HighlightForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty);
                ApplyThemeBinding(HighlightSubitemForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty);
                // ApplyThemeBinding(HighlightSubitemBorderBrushProperty, SsuiTheme.HighlightBorderBrushProperty);
                ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty);
                ApplyThemeBinding(BackgroundProperty, SsuiTheme.BaseBackgroundProperty);

                ApplyTheme(ssuiTheme);
            }

            void ApplyTheme(SsuiTheme? theme)
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, theme);
                ApplyThemeBinding(MenuBorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, theme);

                ApplyThemeBinding(MenuBackgroundProperty, SsuiTheme.PanelBackgroundProperty, theme);
                ApplyThemeBinding(HighlightBrushProperty, SsuiTheme.HighlightBrushProperty, theme);
                ApplyThemeBinding(ClickBrushProperty, SsuiTheme.ClickBrushProperty, theme);
                ApplyThemeBinding(DisabledBrushProperty, SsuiTheme.DisabledForegroundProperty, theme);
                // ApplyThemeBinding(HighlightBorderBrushProperty, SsuiTheme.HighlightBorderBrushProperty, theme);
                ApplyThemeBinding(MenuItemCornerRadiusProperty, SsuiTheme.CornerRadiusProperty, theme);
                ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, theme);
            }
        }

        ///// <remarks>
        ///// The base method will by default set the <see cref="TemplatedControl.BorderBrush"/> value to match the theme; if a different
        ///// border brush is needed, then update it again in your override method.
        ///// </remarks>

        /// <summary>
        /// The backing routed event for <see cref="SsuiThemeApplied"/>. See the related event for details.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> SsuiThemeAppliedEvent =
            RoutedEvent.Register<Menu, RoutedEventArgs>(nameof(SsuiThemeApplied), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised whenenver the SsuiTheme's values have been applied to this control.
        /// </summary>
        public event EventHandler<RoutedEventArgs>? SsuiThemeApplied
        {
            add => AddHandler(SsuiThemeAppliedEvent, value);
            remove => RemoveHandler(SsuiThemeAppliedEvent, value);
        }

        /// <summary>
        /// Clear the <see cref="SsuiTheme"/> property and remove all bindings.
        /// </summary>
        public void ClearSsuiTheme()
        {
            foreach (var item in _themeBindings)
            {
                item.Value.Dispose();
            }

            _themeBindings.Clear();

            SsuiTheme = null;
        }

        #endregion

        #region Bindings

        private Dictionary<string, IDisposable> _themeBindings = [];

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="controlProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or a class that inherits from SsuiTheme
        /// </exception>
        /// <remarks>
        /// If <see cref="SsuiTheme"/> is <c>null</c>, then instead the binding is cleared, and <c>null</c> is returned.
        /// </remarks>
        protected BindingExpressionBase? ApplyThemeBinding(AvaloniaProperty controlProperty, AvaloniaProperty ssuiThemeProperty)
        {
            return ApplyThemeBinding(controlProperty, ssuiThemeProperty, SsuiTheme);
        }

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="controlProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the name of the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <param name="source">the specific SsuiTheme object to bind to (such as <see cref="SsuiAppTheme"/>'s AccentTheme or SubitemTheme)</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or a class that inherits from SsuiTheme
        /// </exception>
        /// <remarks>
        /// If <paramref name="source"/> is <c>null</c>, then instead the binding is cleared, and <c>null</c> is returned.
        /// </remarks>
        protected BindingExpressionBase? ApplyThemeBinding(AvaloniaProperty controlProperty, AvaloniaProperty ssuiThemeProperty, SsuiTheme? source)
        {

            // if the theme value provided is null, then we'll just undo any binding that exists (which should undo it to the previous SsuiTheme)
            if (source == null)
            {
                ClearThemeBinding(controlProperty);
                return null;
            }
            else
            {
                // if we already have an existing binding, let's clear it first
                if (_themeBindings.TryGetValue(controlProperty.Name, out var existingBinding))
                {
                    existingBinding.Dispose();
                }

                BindingExpressionBase binding = Bind(controlProperty, SsuiTheme.CreateBinding(ssuiThemeProperty, source));
                _themeBindings[controlProperty.Name] = binding;

                return binding;
            }
        }

        /// <summary>
        /// Disconnect a binding on a property in this control that was previously bound to a SsuiTheme property using
        /// <c>ApplyThemeBinding</c>. This will reset the property to its default value, unless a lower priority binding
        /// or value was set.
        /// </summary>
        /// <param name="propertyName">The name of the property on this control to remove the binding for</param>
        /// <returns><c>true</c> if the binding was cleared and removed; <c>false</c> if no binding was found</returns>
        protected bool ClearThemeBinding(string propertyName)
        {
            if (_themeBindings.TryGetValue(propertyName, out var binding))
            {
                binding.Dispose();
                _themeBindings.Remove(propertyName);
                return true;
            }
            else return false;

        }

        /// <summary>
        /// Disconnect a binding on a property in this control that was previously bound to a SsuiTheme property using
        /// <c>ApplyThemeBinding</c>. This will reset the property to its default value, unless a lower priority binding
        /// or value was set.
        /// </summary>
        /// <param name="property">The property on this control to remove the binding for</param>
        /// <returns><c>true</c> if the binding was cleared and removed; <c>false</c> if no binding was found</returns>
        protected bool ClearThemeBinding(AvaloniaProperty property)
        {
            return ClearThemeBinding(property.Name);
        }

        #endregion

        #endregion
    }
}
