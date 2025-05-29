using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SolidShineUi
{

    /// <summary>
    /// A class containing various brushes and other settings that can be used to set the appearance of various Solid Shine UI controls.
    /// </summary>
    /// <remarks>
    /// This can be used for most SSUI-themed controls; for SSUI-themed windows or more customized theming, use a <see cref="SsuiAppTheme"/>.
    /// </remarks>
    public class SsuiTheme : Freezable
    {
        /// <summary>
        /// Create a new SsuiTheme, with default brushes.
        /// </summary>
        public SsuiTheme()
        {

        }

        /// <summary>
        /// Create a new SsuiTheme, built around a single base color.
        /// </summary>
        /// <param name="baseColor">the base color to use for creating the theme</param>
        /// <remarks>For best results, use a color that is not too dark or too light.</remarks>
        public SsuiTheme(Color baseColor)
        {

        }

        /// <summary>
        /// Create a new SsuiTheme by adapting a Solid Shine UI 1.x ColorScheme object.
        /// </summary>
        /// <param name="cs">the ColorScheme object to adapt from</param>
        public SsuiTheme(ColorScheme cs)
        {

        }

        /// <summary>
        /// Get or set the brush to use for the background of most SSUI-themed controls.
        /// </summary>
        public Brush ControlBackground { get => (Brush)GetValue(ControlBackgroundProperty); set => SetValue(ControlBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlBackground"/>. See the related property for details.</summary>
        public static DependencyProperty ControlBackgroundProperty
            = DependencyProperty.Register(nameof(ControlBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of panel controls, such as <see cref="SelectPanel"/> and <see cref="TabControl"/>.
        /// </summary>
        public Brush PanelBackground { get => (Brush)GetValue(PanelBackgroundProperty); set => SetValue(PanelBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="PanelBackground"/>. See the related property for details.</summary>
        public static DependencyProperty PanelBackgroundProperty
            = DependencyProperty.Register(nameof(PanelBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for saturated backgrounds of certain controls.
        /// </summary>
        public Brush ControlSatBackground { get => (Brush)GetValue(ControlSatBackgroundProperty); set => SetValue(ControlSatBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlSatBackground"/>. See the related property for details.</summary>
        public static DependencyProperty ControlSatBackgroundProperty
            = DependencyProperty.Register(nameof(ControlSatBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for borders around the edges of most SSUI-themed controls.
        /// </summary>
        public Brush BorderBrush { get => (Brush)GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderBrush"/>. See the related property for details.</summary>
        public static DependencyProperty BorderBrushProperty
            = DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for foreground elements in SSUI-themed controls (such as text or certain symbols or icons).
        /// </summary>
        public Brush Foreground { get => (Brush)GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="Foreground"/>. See the related property for details.</summary>
        public static DependencyProperty ForegroundProperty
            = DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of disabled SSUI-themed controls.
        /// </summary>
        public Brush DisabledBackground { get => (Brush)GetValue(DisabledBackgroundProperty); set => SetValue(DisabledBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledBackground"/>. See the related property for details.</summary>
        public static DependencyProperty DisabledBackgroundProperty
            = DependencyProperty.Register(nameof(DisabledBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the borders around the edges of disabled SSUI-themed controls.
        /// </summary>
        public Brush DisabledBorderBrush { get => (Brush)GetValue(DisabledBorderBrushProperty); set => SetValue(DisabledBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledBorderBrush"/>. See the related property for details.</summary>
        public static DependencyProperty DisabledBorderBrushProperty
            = DependencyProperty.Register(nameof(DisabledBorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the foreground elements of disabled SSUI-themed controls.
        /// </summary>
        public Brush DisabledForeground { get => (Brush)GetValue(DisabledForegroundProperty); set => SetValue(DisabledForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledForeground"/>. See the related property for details.</summary>
        public static DependencyProperty DisabledForegroundProperty
            = DependencyProperty.Register(nameof(DisabledForeground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for when a SSUI-themed control is highlighted (e.g. mouse over, keyboard focus).
        /// </summary>
        public Brush HighlightBrush { get => (Brush)GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static DependencyProperty HighlightBrushProperty
            = DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for when a SSUI-themed control is being clicked/pressed.
        /// </summary>
        public Brush ClickBrush { get => (Brush)GetValue(ClickBrushProperty); set => SetValue(ClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ClickBrushProperty
            = DependencyProperty.Register(nameof(ClickBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for foreground elements of a SSUI-themed control while it is being highlighted.
        /// </summary>
        /// <remarks>
        /// This is useful for situations where the highlight brush is a fairly different color from the standard background brush,
        /// and so the foreground needs to be changed to keep a good contrast against the highlighted background.
        /// </remarks>
        public Brush HighlightForeground { get => (Brush)GetValue(HighlightForegroundProperty); set => SetValue(HighlightForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightForeground"/>. See the related property for details.</summary>
        public static DependencyProperty HighlightForegroundProperty
            = DependencyProperty.Register(nameof(HighlightForeground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of SSUI-themed controls while it is selected 
        /// (e.g. <see cref="IClickSelectableControl.IsSelected"/> is <c>true</c>).
        /// </summary>
        public Brush SelectedBackgroundBrush { get => (Brush)GetValue(SelectedBackgroundBrushProperty); set => SetValue(SelectedBackgroundBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedBackgroundBrush"/>. See the related property for details.</summary>
        public static DependencyProperty SelectedBackgroundBrushProperty
            = DependencyProperty.Register(nameof(SelectedBackgroundBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the borders around the edges of SSUI-themed controls where a lighter border color is used.
        /// </summary>
        public Brush LightBorderBrush { get => (Brush)GetValue(LightBorderBrushProperty); set => SetValue(LightBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="LightBorderBrush"/>. See the related property for details.</summary>
        public static DependencyProperty LightBorderBrushProperty
            = DependencyProperty.Register(nameof(LightBorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for checkmark symbols in certain SSUI-themed controls such as <see cref="CheckBox"/>.
        /// </summary>
        public Brush CheckBrush { get => (Brush)GetValue(CheckBrushProperty); set => SetValue(CheckBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckBrush"/>. See the related property for details.</summary>
        public static DependencyProperty CheckBrushProperty
            = DependencyProperty.Register(nameof(CheckBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for key elements in certain SSUI-themed controls that should be distinguished or stand out (or "pop")
        /// against the background (e.g. the selector in the <see cref="SolidShineUi.Utils.RelativePositionSelect"/>).
        /// </summary>
        public Brush ControlPopBrush { get => (Brush)GetValue(ControlPopBrushProperty); set => SetValue(ControlPopBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlPopBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ControlPopBrushProperty
            = DependencyProperty.Register(nameof(ControlPopBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the corner radius to use around the edges of many SSUI-themed controls.
        /// </summary>
        /// <remarks>
        /// A <see cref="System.Windows.CornerRadius"/> value of all 0s will result in completely square 90-degree angle corners; values above 0
        /// will result in increasingly more rounded corners.
        /// </remarks>
        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        /// <summary>The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static DependencyProperty CornerRadiusProperty
            = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(new CornerRadius(0)));

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new SsuiTheme();
        }

    }


    /// <summary>
    /// A class containing various brushes and other settings that can be used to set the appearance of SSUI-themed controls, windows, and entire applications.
    /// </summary>
    public class SsuiAppTheme : SsuiTheme
    {
        /// <summary>
        /// Create a new SsuiAppTheme, with default colors.
        /// </summary>
        public SsuiAppTheme() : base()
        {

        }

        /// <summary>
        /// Create a new SsuiAppTheme, built around a single base color.
        /// </summary>
        /// <param name="baseColor">the base color to use for creating the theme</param>
        /// <remarks>For best results, use a color that is not too dark or too light.</remarks>
        public SsuiAppTheme(Color baseColor) : base(baseColor)
        {

        }

        /// <summary>
        /// Create a new SsuiAppTheme, built around a base color, and a second color to use as an accent color/theme.
        /// </summary>
        /// <param name="baseColor">the base color to use for creating the theme</param>
        /// <param name="accentColor">the color to use for creating the accent theme</param>
        /// <remarks>
        /// The <paramref name="accentColor"/> will be used to create the <see cref="SsuiAppTheme.AccentTheme"/>.
        /// For best results, use colors that are not too dark or too light.
        /// </remarks>
        public SsuiAppTheme(Color baseColor, Color accentColor) : base(baseColor)
        {

        }

        /// <summary>
        /// Create a new SsuiAppTheme, built around a base color, a second color to use as an accent color/theme, and a 
        /// third color to use for subitems within certain controls.
        /// </summary>
        /// <param name="baseColor">the base color to use for creating the theme</param>
        /// <param name="accentColor">the color to use for creating the accent theme</param>
        /// <param name="subItemColor">the color to use for creating the subitem theme</param>
        /// <remarks>
        /// The <paramref name="accentColor"/> will be used to create the <see cref="SsuiAppTheme.AccentTheme"/>.
        /// The <paramref name="subItemColor"/> will be used to create the <see cref="SsuiAppTheme.SubitemTheme"/>.
        /// Make sure to also set the <c>UseSubitemThemeWith...</c> properties to control where to use the subitem theme.
        /// For best results, use colors that are not too dark or too light.
        /// </remarks>
        public SsuiAppTheme(Color baseColor, Color accentColor, Color subItemColor) : base(baseColor)
        {

        }

        /// <summary>
        /// Create a new SsuiTheme by adapting a Solid Shine UI 1.x ColorScheme object.
        /// </summary>
        /// <param name="cs">the ColorScheme object to adapt from</param>
        public SsuiAppTheme(ColorScheme cs) : base(cs)
        {

        }

        /// <summary>
        /// Get or set the brush to use for the title bar area of a SSUI-themed window.
        /// </summary>
        public Brush WindowTitleBackground { get => (Brush)GetValue(WindowTitleBackgroundProperty); set => SetValue(WindowTitleBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowTitleBackground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowTitleBackgroundProperty
            = DependencyProperty.Register(nameof(WindowTitleBackground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the foreground elements of the title bar area of a SSUI-themed window. (i.e. the window title text).
        /// </summary>
        public Brush WindowTitleForeground { get => (Brush)GetValue(WindowTitleForegroundProperty); set => SetValue(WindowTitleForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowTitleForeground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowTitleForegroundProperty
            = DependencyProperty.Register(nameof(WindowTitleForeground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the window caption controls in the top corner of a SSUI-themed window.
        /// </summary>
        public Brush WindowCaptionsBackground { get => (Brush)GetValue(WindowCaptionsBackgroundProperty); set => SetValue(WindowCaptionsBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsBackground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowCaptionsBackgroundProperty
            = DependencyProperty.Register(nameof(WindowCaptionsBackground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the symbols of the window caption controls in the top corner of a SSUI-themed window.
        /// </summary>
        public Brush WindowCaptionsForeground { get => (Brush)GetValue(WindowCaptionsForegroundProperty); set => SetValue(WindowCaptionsForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsForeground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowCaptionsForegroundProperty
            = DependencyProperty.Register(nameof(WindowCaptionsForeground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of a window caption control while it is highlighted (e.g. mouse over, keyboard focus).
        /// </summary>
        public Brush WindowCaptionsHighlight { get => (Brush)GetValue(WindowCaptionsHighlightProperty); set => SetValue(WindowCaptionsHighlightProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsHighlight"/>. See the related property for details.</summary>
        public static DependencyProperty WindowCaptionsHighlightProperty
            = DependencyProperty.Register(nameof(WindowCaptionsHighlight), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the symbols of a window caption control while it is highlighted (e.g. mouse over, keyboard focus).
        /// </summary>
        public Brush WindowCaptionsHighlightForeground { get => (Brush)GetValue(WindowCaptionsHighlightForegroundProperty); set => SetValue(WindowCaptionsHighlightForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsHighlightForeground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowCaptionsHighlightForegroundProperty
            = DependencyProperty.Register(nameof(WindowCaptionsHighlightForeground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of a window caption control while it is being clicked/pressed.
        /// </summary>
        public Brush WindowCaptionsClickBrush { get => (Brush)GetValue(WindowCaptionsClickBrushProperty); set => SetValue(WindowCaptionsClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsClickBrush"/>. See the related property for details.</summary>
        public static DependencyProperty WindowCaptionsClickBrushProperty
            = DependencyProperty.Register(nameof(WindowCaptionsClickBrush), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the content area of a SSUI-themed window.
        /// </summary>
        public Brush WindowBackground { get => (Brush)GetValue(WindowBackgroundProperty); set => SetValue(WindowBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowBackground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowBackgroundProperty
            = DependencyProperty.Register(nameof(WindowBackground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the theme to use for controls that are set to use accent brushes.
        /// </summary>
        /// <remarks>
        /// Using the accent theme on certain controls can help bring attention to those controls and make them stand out in the UI.
        /// </remarks>
        public SsuiTheme AccentTheme { get => (SsuiTheme)GetValue(AccentThemeProperty); set => SetValue(AccentThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="AccentTheme"/>. See the related property for details.</summary>
        public static DependencyProperty AccentThemeProperty
            = DependencyProperty.Register(nameof(AccentTheme), typeof(SsuiTheme), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(new SsuiTheme()));

        /// <summary>
        /// Get or set the theme to use for child items within certain SSUI container controls (such as <see cref="SelectPanel"/> or <see cref="Menu"/>).
        /// </summary>
        /// <remarks>
        /// This is purely a stylistic choice; use the <c>UseSubitemThemeWith...</c> properties to control which controls to use this theme for.
        /// </remarks>
        public SsuiTheme SubitemTheme { get => (SsuiTheme)GetValue(SubitemThemeProperty); set => SetValue(SubitemThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="SubitemTheme"/>. See the related property for details.</summary>
        public static DependencyProperty SubitemThemeProperty
            = DependencyProperty.Register(nameof(SubitemTheme), typeof(SsuiTheme), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(new SsuiTheme()));

        /// <summary>
        /// Get or set if the <see cref="SubitemTheme"/> should be used for <see cref="Menu"/> and <see cref="ContextMenu"/> controls.
        /// </summary>
        public bool UseSubitemThemeWithMenus { get => (bool)GetValue(UseSubitemThemeWithMenusProperty); set => SetValue(UseSubitemThemeWithMenusProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseSubitemThemeWithMenus"/>. See the related property for details.</summary>
        public static DependencyProperty UseSubitemThemeWithMenusProperty
            = DependencyProperty.Register(nameof(UseSubitemThemeWithMenus), typeof(bool), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set if the <see cref="SubitemTheme"/> should be used for <see cref="SelectPanel"/> and <see cref="PropertyList.PropertyList"/> controls.
        /// </summary>
        public bool UseSubitemThemeWithPanels { get => (bool)GetValue(UseSubitemThemeWithPanelsProperty); set => SetValue(UseSubitemThemeWithPanelsProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseSubitemThemeWithPanels"/>. See the related property for details.</summary>
        public static DependencyProperty UseSubitemThemeWithPanelsProperty
            = DependencyProperty.Register(nameof(UseSubitemThemeWithPanels), typeof(bool), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set if the <see cref="SubitemTheme"/> should be used for the <c>Ribbon</c> control.
        /// </summary>
        public bool UseSubitemThemeWithRibbons { get => (bool)GetValue(UseSubitemThemeWithRibbonsProperty); set => SetValue(UseSubitemThemeWithRibbonsProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseSubitemThemeWithRibbons"/>. See the related property for details.</summary>
        public static DependencyProperty UseSubitemThemeWithRibbonsProperty
            = DependencyProperty.Register(nameof(UseSubitemThemeWithRibbons), typeof(bool), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(true));

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new SsuiAppTheme();
        }

    }
}
