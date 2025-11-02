using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SolidShineUi
{

    /// <summary>
    /// A class containing various brushes and other settings that can be used to set the appearance of various Solid Shine UI controls.
    /// </summary>
    /// <remarks>
    /// This can be used in all SSUI-themed controls; for SSUI-themed windows or to apply across an entire application, use a <see cref="SsuiAppTheme"/>.
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
            CreatePalette(baseColor);
        }

        /// <summary>
        /// Create a new SsuiTheme by adapting a Solid Shine UI 1.x ColorScheme object.
        /// </summary>
        /// <param name="cs">the ColorScheme object to adapt from</param>
        public SsuiTheme(ColorScheme cs)
        {
            // TODO: detect if it's a high contrast theme
            // if so, bring out a SsuiTheme high contrast theme

            BaseBackground = cs.BackgroundColor.ToBrush();
            BorderBrush = cs.BorderColor.ToBrush();
            CheckBrush = cs.ForegroundColor.ToBrush();
            ClickBrush = cs.SecondHighlightColor.ToBrush();
            ControlBackground = cs.SecondaryColor.ToBrush();
            ControlPopBrush = cs.SelectionColor.ToBrush();
            ControlSatBackground = cs.MainColor.ToBrush();
            DisabledBackground = cs.LightDisabledColor.ToBrush();
            DisabledBorderBrush = cs.DarkDisabledColor.ToBrush();
            DisabledForeground = cs.DarkDisabledColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
            HighlightBrush = cs.ThirdHighlightColor.ToBrush();
            HighlightBorderBrush = cs.HighlightColor.ToBrush();
            HighlightForeground = cs.ForegroundColor.ToBrush();
            LightBorderBrush = cs.HighlightColor.ToBrush();
            PanelBackground = cs.LightBackgroundColor.ToBrush();
            SelectedBackgroundBrush = cs.ThirdHighlightColor.ToBrush();
            SelectedBorderBrush = cs.SelectionColor.ToBrush();
        }

        void CreatePalette(Color baseColor)
        {
            // this palette building system works by pretty much taking the HSV values of the base color
            // and making alternate colors by changing the value by various amounts
            // and then creating clamps/bounds on saturation to prevent colors going too light

            ColorsHelper.ToHSV(baseColor, out double h, out double s, out double v);

            // the amount to decrease the base color's value by for creating some other key colors
            double vc1 = -0.12;
            double vc2 = -0.2;
            double vc3 = -0.4;

            double t1 = 0.15;
            double t2 = 0.45;
            double t3 = 0.27;
            double t4 = 0.9;

            // some saturation bounds to prevent colors becoming too light or weird
            double ssc = 0.3;
            double sec = 0.12;
            double stc = 0.16;
            double sc = 0.059;
            double sbc = 0.03;

            ControlSatBackground = baseColor.ToBrush();
            //MainColor = baseColor;
            //WindowTitleBarColor = baseColor;
            //WindowInactiveColor = baseColor;
            HighlightBorderBrush = AddValue(h, s, v, vc1).ToBrush();
            SelectedBorderBrush = AddValue(h, s, v, vc2).ToBrush();
            ControlPopBrush = AddValue(h, s, v, vc2).ToBrush();
            BorderBrush = AddValue(h, s, v, vc3).ToBrush();
            LightBorderBrush = AddValue(h, s, v, vc1).ToBrush();

            // for the disabled brushes, we'll use the default values of the dependency properties

            // next, we'll create a couple brushes by making sure the saturation is within a few bounds
            // and then using either the actual saturation or the bounded value for these brushes

            if (s > ssc)
            {
                ControlBackground = AddValue(h, ssc, v, t1).ToBrush(); // 4 - Secondary Color
            }
            else
            {
                ControlBackground = AddValue(h, s, v, t1).ToBrush(); // 4
            }

            if (s > sc)
            {
                BaseBackground = AddValue(h, sc, v, t2).ToBrush(); // 5 - Background Color
            }
            else
            {
                BaseBackground = AddValue(h, s, v, t2).ToBrush(); // 5
            }

            if (s > sec)
            {
                SelectedBackgroundBrush = AddValue(h, sec, v, t3).ToBrush(); // 6 - Second Highlight Color
                ClickBrush = AddValue(h, sec, v, t3).ToBrush();
            }
            else
            {
                SelectedBackgroundBrush = AddValue(h, s, v, t3).ToBrush(); // 6
                ClickBrush = AddValue(h, s, v, t3).ToBrush();
            }

            if (s > stc)
            {
                HighlightBrush = AddValue(h, stc, v, t3).ToBrush(); // 7 - Menu Highlight Color
            }
            else
            {
                HighlightBrush = AddValue(h, s, v, t3).ToBrush(); // 7
            }

            if (s > sbc)
            {
                PanelBackground = AddValue(h, sbc, v, t4).ToBrush(); // 8 - Menu Background Color

            }
            else
            {
                PanelBackground = AddValue(h, s, v, t4).ToBrush(); // 8
            }

            // finally, we'll want to check the foreground against the base background,
            float bBase = GetColorBrightness(((SolidColorBrush)BaseBackground).Color);
            Foreground = (bBase < 0.55 ? Colors.White : Colors.Black).ToBrush();

            // the highlight foreground against the highlight brush,
            float bHigh = GetColorBrightness(((SolidColorBrush)HighlightBrush).Color);
            HighlightForeground = (bHigh < 0.55 ? Colors.White : Colors.Black).ToBrush();

            // and the check brush against the panel background
            float bPane = GetColorBrightness(((SolidColorBrush)PanelBackground).Color);
            CheckBrush = (bPane < 0.55 ? Colors.White : Colors.Black).ToBrush();

            Color AddValue(double ch, double cs, double cv, double add)
            {
                if (cv + add < 0)
                {
                    cv = 0;
                }
                else if (cv + add > 1)
                {
                    cv = 1;
                }
                else
                {
                    cv += add;
                }

                return ColorsHelper.CreateFromHSV(ch, cs, cv);
            }

            // from https://stackoverflow.com/a/50541212/2987285
            float GetColorBrightness(Color c)
            {
                return (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f;
            }
        }

        /// <summary>
        /// Get or set the brush to use for the background of most SSUI-themed controls.
        /// </summary>
        public Brush ControlBackground { get => (Brush)GetValue(ControlBackgroundProperty); set => SetValue(ControlBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ControlBackgroundProperty
            = DependencyProperty.Register(nameof(ControlBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of panel controls, such as <see cref="SelectPanel"/> and <see cref="TabControl"/>,
        /// that seem to "sink in" to the background, rather than "popping out".
        /// </summary>
        public Brush PanelBackground { get => (Brush)GetValue(PanelBackgroundProperty); set => SetValue(PanelBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="PanelBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty PanelBackgroundProperty
            = DependencyProperty.Register(nameof(PanelBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of an entire underlying surface, pane, window, or area. This can also be used for
        /// the background of controls that should neither "pop out" or "sink in" to the underlying background.
        /// </summary>
        public Brush BaseBackground { get => (Brush)GetValue(BaseBackgroundProperty); set => SetValue(BaseBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="BaseBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BaseBackgroundProperty
            = DependencyProperty.Register(nameof(BaseBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));


        /// <summary>
        /// Get or set the brush to use for saturated backgrounds of certain controls.
        /// </summary>
        /// <remarks>
        /// If this SsuiTheme was created by using a base color, this brush will also be set to that base color.
        /// </remarks>
        public Brush ControlSatBackground { get => (Brush)GetValue(ControlSatBackgroundProperty); set => SetValue(ControlSatBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlSatBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ControlSatBackgroundProperty
            = DependencyProperty.Register(nameof(ControlSatBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for borders around the edges of most SSUI-themed controls.
        /// </summary>
        public Brush BorderBrush { get => (Brush)GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderBrushProperty
            = DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for foreground elements in SSUI-themed controls (such as text or certain symbols or icons).
        /// </summary>
        public Brush Foreground { get => (Brush)GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="Foreground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ForegroundProperty
            = DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of disabled SSUI-themed controls.
        /// </summary>
        public Brush DisabledBackground { get => (Brush)GetValue(DisabledBackgroundProperty); set => SetValue(DisabledBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledBackgroundProperty
            = DependencyProperty.Register(nameof(DisabledBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(ColorsHelper.CreateFromHex("F5F5F5").ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the borders around the edges of disabled SSUI-themed controls.
        /// </summary>
        public Brush DisabledBorderBrush { get => (Brush)GetValue(DisabledBorderBrushProperty); set => SetValue(DisabledBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledBorderBrushProperty
            = DependencyProperty.Register(nameof(DisabledBorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(ColorsHelper.CreateFromHex("AAAAAF").ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the foreground elements of disabled SSUI-themed controls.
        /// </summary>
        public Brush DisabledForeground { get => (Brush)GetValue(DisabledForegroundProperty); set => SetValue(DisabledForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledForegroundProperty
            = DependencyProperty.Register(nameof(DisabledForeground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(ColorsHelper.CreateFromHex("AAAAAF").ToBrush()));

        /// <summary>
        /// Get or set the brush to use for when a SSUI-themed control is highlighted (e.g. mouse over, keyboard focus).
        /// </summary>
        public Brush HighlightBrush { get => (Brush)GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty
            = DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the border around the edges of a SSUI-themed control while it is higlighted
        /// (e.g. mouse over, keyboard focus).
        /// </summary>
        public Brush HighlightBorderBrush { get => (Brush)GetValue(HighlightBorderBrushProperty); set => SetValue(HighlightBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBorderBrushProperty
            = DependencyProperty.Register(nameof(HighlightBorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for when a SSUI-themed control is being clicked/pressed.
        /// </summary>
        public Brush ClickBrush { get => (Brush)GetValue(ClickBrushProperty); set => SetValue(ClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ClickBrushProperty
            = DependencyProperty.Register(nameof(ClickBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.LightGray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for foreground elements of a SSUI-themed control while it is being highlighted.
        /// </summary>
        /// <remarks>
        /// This is useful for situations where the highlight brush is a fairly different color from the standard background brush,
        /// and so the foreground needs to be changed to keep a good contrast against the highlighted background.
        /// </remarks>
        public Brush HighlightForeground { get => (Brush)GetValue(HighlightForegroundProperty); set => SetValue(HighlightForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightForegroundProperty
            = DependencyProperty.Register(nameof(HighlightForeground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of SSUI-themed controls while they are selected 
        /// (e.g. <see cref="IClickSelectableControl.IsSelected"/> is <c>true</c>).
        /// </summary>
        public Brush SelectedBackgroundBrush { get => (Brush)GetValue(SelectedBackgroundBrushProperty); set => SetValue(SelectedBackgroundBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedBackgroundBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedBackgroundBrushProperty
            = DependencyProperty.Register(nameof(SelectedBackgroundBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the border around the edges of SSUI-themed controls while they are selected
        /// (e.g. <see cref="IClickSelectableControl.IsSelected"/> is <c>true</c>).
        /// </summary>
        public Brush SelectedBorderBrush { get => (Brush)GetValue(SelectedBorderBrushProperty); set => SetValue(SelectedBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedBorderBrushProperty
            = DependencyProperty.Register(nameof(SelectedBorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        /// <summary>
        /// Get or set the brush to use for the borders around the edges of SSUI-themed controls where a lighter border color is used.
        /// </summary>
        public Brush LightBorderBrush { get => (Brush)GetValue(LightBorderBrushProperty); set => SetValue(LightBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="LightBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty LightBorderBrushProperty
            = DependencyProperty.Register(nameof(LightBorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.DarkGray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for checkmark symbols in certain SSUI-themed controls such as <see cref="CheckBox"/>.
        /// </summary>
        public Brush CheckBrush { get => (Brush)GetValue(CheckBrushProperty); set => SetValue(CheckBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CheckBrushProperty
            = DependencyProperty.Register(nameof(CheckBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for key elements in certain SSUI-themed controls that should be distinguished or stand out (or "pop")
        /// against the background (e.g. the selector in the <see cref="RelativePositionSelect"/>).
        /// </summary>
        public Brush ControlPopBrush { get => (Brush)GetValue(ControlPopBrushProperty); set => SetValue(ControlPopBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlPopBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ControlPopBrushProperty
            = DependencyProperty.Register(nameof(ControlPopBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.DarkGray.ToBrush()));

        /// <summary>
        /// Get or set the corner radius to use around the edges of many SSUI-themed controls.
        /// </summary>
        /// <remarks>
        /// A <see cref="System.Windows.CornerRadius"/> value of all 0s will result in completely square 90-degree angle corners; values above 0
        /// will result in increasingly more rounded corners.
        /// </remarks>
        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        /// <summary>The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CornerRadiusProperty
            = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// Get or set the variation to use for image-based icons in various SSUI-themed controls, such as <see cref="PropertyList.PropertyList"/>.
        /// </summary>
        public IconVariation IconVariation { get => (IconVariation)GetValue(IconVariationProperty); set => SetValue(IconVariationProperty, value); }

        /// <summary>The backing dependency property for <see cref="IconVariation"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IconVariationProperty
            = DependencyProperty.Register(nameof(IconVariation), typeof(IconVariation), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(IconVariation.Color));


        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new SsuiTheme();
        }


        /// <summary>
        /// Create a <see cref="Binding"/> for a property in the SsuiTheme.
        /// </summary>
        /// <param name="ssuiThemeProperty">the SsuiTheme or SsuiAppTheme property to bind to</param>
        /// <param name="source">the SsuiTheme object holding the value to bind</param>
        /// <returns>A <see cref="Binding"/> object that can be used to bind other controls' properties to this property.</returns>
        /// <exception cref="ArgumentException">thrown if <paramref name="ssuiThemeProperty"/> is not a SsuiTheme property, or a property from a class that inherits from SsuiTheme</exception>
        public static Binding CreateBinding(DependencyProperty ssuiThemeProperty, SsuiTheme source)
        {
            if (ssuiThemeProperty.OwnerType != typeof(SsuiTheme) && !ssuiThemeProperty.OwnerType.IsSubclassOf(typeof(SsuiTheme)))
            {
                throw new ArgumentException("This property is not an SsuiTheme property", nameof(ssuiThemeProperty));
            }
            return new Binding(ssuiThemeProperty.Name) { Source = source };

            // in the future, I could create a small cache of Binding objects that the SsuiTheme itself stores and returns, rather than creating a new binding each time
            // what I don't know, though, is whether each WPF binding expression requires a unique Binding object, or if I can reuse Binding objects
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
            CreateWindowPalette(baseColor);
            AccentTheme = new SsuiTheme(baseColor);
            SubitemTheme = new SsuiTheme(baseColor);
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
            CreateWindowPalette(baseColor);
            AccentTheme = new SsuiTheme(accentColor);
            SubitemTheme = new SsuiTheme(baseColor);
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
            CreateWindowPalette(baseColor);
            AccentTheme = new SsuiTheme(accentColor);
            SubitemTheme = new SsuiTheme(subItemColor);
        }

        /// <summary>
        /// Create a new SsuiTheme by adapting a Solid Shine UI 1.x ColorScheme object.
        /// </summary>
        /// <param name="cs">the ColorScheme object to adapt from</param>
        public SsuiAppTheme(ColorScheme cs) : base(cs)
        {
            WindowTitleBackground = cs.WindowTitleBarColor.ToBrush();
            WindowInactiveBackground = cs.WindowInactiveColor.ToBrush();
            WindowTitleForeground = cs.WindowTitleBarTextColor.ToBrush();
            WindowCaptionsBackground = cs.WindowTitleBarColor.ToBrush();
            WindowCaptionsForeground = cs.WindowTitleBarTextColor.ToBrush();
            WindowCaptionsHighlight = cs.HighlightColor.ToBrush();
            WindowCaptionsClickBrush = cs.SelectionColor.ToBrush();
            WindowCaptionsHighlightForeground = cs.WindowTitleBarTextColor.ToBrush();
            WindowBackground = cs.BackgroundColor.ToBrush();

            AccentTheme = new SsuiTheme(cs.AccentMainColor);
            SubitemTheme = new SsuiTheme(cs.MainColor);
        }

        void CreateWindowPalette(Color baseColor)
        {
            // this palette building system works by pretty much taking the HSV values of the base color
            // and making alternate colors by changing the value by various amounts
            // and then creating clamps/bounds on saturation to prevent colors going too light

            ColorsHelper.ToHSV(baseColor, out double h, out double s, out double v);

            // the amount to decrease the base color's value by for creating some other key colors
            double vc1 = -0.12;
            double vc2 = -0.2;

            double t2 = 0.45;

            // some saturation bounds to prevent colors becoming too light or weird
            double sc = 0.059;

            WindowTitleBackground = baseColor.ToBrush();
            WindowCaptionsBackground = baseColor.ToBrush();
            WindowInactiveBackground = baseColor.ToBrush();
            WindowCaptionsHighlight = AddValue(h, s, v, vc1).ToBrush();
            WindowCaptionsClickBrush = AddValue(h, s, v, vc2).ToBrush();

            // finally, we'll want to check the foreground against the base background,
            float bBase = GetColorBrightness(baseColor);
            Color fore = (bBase < 0.55 ? Colors.White : Colors.Black);

            WindowTitleForeground = fore.ToBrush();
            WindowCaptionsForeground = fore.ToBrush();
            WindowCaptionsHighlightForeground = fore.ToBrush();

            // next, we'll create a couple brushes by making sure the saturation is within a few bounds
            // and then using either the actual saturation or the bounded value for these brushes

            if (s > sc)
            {
                WindowBackground = AddValue(h, sc, v, t2).ToBrush(); // 5 - Background Color
            }
            else
            {
                WindowBackground = AddValue(h, s, v, t2).ToBrush(); // 5
            }

            Color AddValue(double ch, double cs, double cv, double add)
            {
                if (cv + add < 0)
                {
                    cv = 0;
                }
                else if (cv + add > 1)
                {
                    cv = 1;
                }
                else
                {
                    cv += add;
                }

                return ColorsHelper.CreateFromHSV(ch, cs, cv);
            }

            float GetColorBrightness(Color c)
            {
                return (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f;
            }
        }

        /// <summary>
        /// Get or set the brush to use for the title bar area of a SSUI-themed window.
        /// </summary>
        public Brush WindowTitleBackground { get => (Brush)GetValue(WindowTitleBackgroundProperty); set => SetValue(WindowTitleBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowTitleBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty WindowTitleBackgroundProperty
            = DependencyProperty.Register(nameof(WindowTitleBackground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.LightGray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the foreground elements of the title bar area of a SSUI-themed window. (e.g. the window title text).
        /// </summary>
        public Brush WindowTitleForeground { get => (Brush)GetValue(WindowTitleForegroundProperty); set => SetValue(WindowTitleForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowTitleForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty WindowTitleForegroundProperty
            = DependencyProperty.Register(nameof(WindowTitleForeground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the title bar area of a SSUI-themed window while it is inactive (not focused).
        /// </summary>
        public Brush WindowInactiveBackground { get => (Brush)GetValue(WindowInactiveBackgroundProperty); set => SetValue(WindowInactiveBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowInactiveBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty WindowInactiveBackgroundProperty
            = DependencyProperty.Register(nameof(WindowInactiveBackground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the foreground elements of the title bar area of a SSUI-themed window while it is inactive (not focused).
        /// </summary>
        public Brush WindowInactiveForeground { get => (Brush)GetValue(WindowInactiveForegroundProperty); set => SetValue(WindowInactiveForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowInactiveForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty WindowInactiveForegroundProperty
            = DependencyProperty.Register(nameof(WindowInactiveForeground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(ColorsHelper.CreateFromHex("#505050").ToBrush()));


        /// <summary>
        /// Get or set the brush to use for the background of the window caption controls in the top corner of a SSUI-themed window.
        /// </summary>
        public Brush WindowCaptionsBackground { get => (Brush)GetValue(WindowCaptionsBackgroundProperty); set => SetValue(WindowCaptionsBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty WindowCaptionsBackgroundProperty
            = DependencyProperty.Register(nameof(WindowCaptionsBackground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.LightGray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the symbols of the window caption controls in the top corner of a SSUI-themed window.
        /// </summary>
        public Brush WindowCaptionsForeground { get => (Brush)GetValue(WindowCaptionsForegroundProperty); set => SetValue(WindowCaptionsForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty WindowCaptionsForegroundProperty
            = DependencyProperty.Register(nameof(WindowCaptionsForeground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of a window caption control while it is highlighted (e.g. mouse over, keyboard focus).
        /// </summary>
        public Brush WindowCaptionsHighlight { get => (Brush)GetValue(WindowCaptionsHighlightProperty); set => SetValue(WindowCaptionsHighlightProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsHighlight"/>. See the related property for details.</summary>
        public static readonly DependencyProperty WindowCaptionsHighlightProperty
            = DependencyProperty.Register(nameof(WindowCaptionsHighlight), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the symbols of a window caption control while it is highlighted (e.g. mouse over, keyboard focus).
        /// </summary>
        public Brush WindowCaptionsHighlightForeground { get => (Brush)GetValue(WindowCaptionsHighlightForegroundProperty); set => SetValue(WindowCaptionsHighlightForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsHighlightForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty WindowCaptionsHighlightForegroundProperty
            = DependencyProperty.Register(nameof(WindowCaptionsHighlightForeground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of a window caption control while it is being clicked/pressed.
        /// </summary>
        public Brush WindowCaptionsClickBrush { get => (Brush)GetValue(WindowCaptionsClickBrushProperty); set => SetValue(WindowCaptionsClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty WindowCaptionsClickBrushProperty
            = DependencyProperty.Register(nameof(WindowCaptionsClickBrush), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Gray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the content area of a SSUI-themed window.
        /// </summary>
        public Brush WindowBackground { get => (Brush)GetValue(WindowBackgroundProperty); set => SetValue(WindowBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty WindowBackgroundProperty
            = DependencyProperty.Register(nameof(WindowBackground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// Get or set the theme to use for controls that are set to use accent brushes.
        /// </summary>
        /// <remarks>
        /// Using the accent theme on certain controls can help bring attention to those controls and make them stand out in the UI.
        /// By default, all controls will use the main theme; enable this for certain controls by setting <c>UseAccentTheme</c> to <c>true</c>
        /// on that control.
        /// </remarks>
        public SsuiTheme AccentTheme { get => (SsuiTheme)GetValue(AccentThemeProperty); set => SetValue(AccentThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="AccentTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty AccentThemeProperty
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
        public static readonly DependencyProperty SubitemThemeProperty
            = DependencyProperty.Register(nameof(SubitemTheme), typeof(SsuiTheme), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(new SsuiTheme()));

        /// <summary>
        /// Get or set if the <see cref="SubitemTheme"/> should be used for <see cref="Menu"/> and <see cref="ContextMenu"/> controls.
        /// </summary>
        public bool UseSubitemThemeWithMenus { get => (bool)GetValue(UseSubitemThemeWithMenusProperty); set => SetValue(UseSubitemThemeWithMenusProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseSubitemThemeWithMenus"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseSubitemThemeWithMenusProperty
            = DependencyProperty.Register(nameof(UseSubitemThemeWithMenus), typeof(bool), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set if the <see cref="SubitemTheme"/> should be used for <see cref="SelectPanel"/> and <see cref="PropertyList.PropertyList"/> controls.
        /// </summary>
        public bool UseSubitemThemeWithPanels { get => (bool)GetValue(UseSubitemThemeWithPanelsProperty); set => SetValue(UseSubitemThemeWithPanelsProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseSubitemThemeWithPanels"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseSubitemThemeWithPanelsProperty
            = DependencyProperty.Register(nameof(UseSubitemThemeWithPanels), typeof(bool), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set if the <see cref="SubitemTheme"/> should be used for the <c>Ribbon</c> control.
        /// </summary>
        public bool UseSubitemThemeWithRibbons { get => (bool)GetValue(UseSubitemThemeWithRibbonsProperty); set => SetValue(UseSubitemThemeWithRibbonsProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseSubitemThemeWithRibbons"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseSubitemThemeWithRibbonsProperty
            = DependencyProperty.Register(nameof(UseSubitemThemeWithRibbons), typeof(bool), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(true));

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new SsuiAppTheme();
        }
    }
}
