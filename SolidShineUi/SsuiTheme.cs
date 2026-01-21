using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SolidShineUi
{

    /// <summary>
    /// A class containing various brushes and other settings that can be used to set the appearance of various Solid Shine UI controls.
    /// </summary>
    /// <remarks>
    /// This can be used in all SSUI-themed controls. For SSUI-themed windows (see <see cref="ThemedWindow"/>)
    /// or to have more options for applying across an entire application, use a <see cref="SsuiAppTheme"/> instead.
    /// <para/>
    /// Note that this class is not meant to be used for serialization. Use the <see cref="ToSerializableObject"/> method to create a copy of this
    /// that can be used for serialization scenarios, like storing in a settings file.
    /// </remarks>
    public class SsuiTheme : Animatable
    {
        #region Constructors / Create Palette

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
        /// <remarks>
        /// It is recommended for new programs to create and use a SsuiTheme using one of the other constructors, rather than
        /// using a ColorScheme. This constructor is primarily provided as an upgrade path for those upgrading their app from SSUI 1.9.
        /// <para/>
        /// This creates a SsuiTheme by assigning brush values from the colors of the ColorScheme, with the goal of closely matching
        /// the visual appearance of how the ColorScheme's colors were used within SSUI's controls.
        /// If the ColorScheme <paramref name="cs"/> is a high contrast theme, this may create undesirable results. Instead, you should 
        /// create a high contrast SsuiTheme using <see cref="SsuiThemes.GetHighContrastTheme(HighContrastOption)"/>.
        /// </remarks>
        public SsuiTheme(ColorScheme cs)
        {
            // maybe a future thing to explore is checking if cs is a high contrast theme, and then copying in all the values from
            // the corresponding high contrast SsuiTheme in SsuiThemes, rather than just blindly copying the values here
            //
            // SSUI's controls have special behavior/handling for high contrast ColorSchemes that just isn't needed for a SsuiTheme
            // however, due to this, directly copying over the ColorScheme's values without that marker will cause certain controls
            // to look or act a bit differently.

            BaseBackground = cs.BackgroundColor.ToBrush();
            BorderBrush = cs.BorderColor.ToBrush();
            CheckBrush = cs.ForegroundColor.ToBrush();
            ClickBrush = cs.SecondHighlightColor.ToBrush();
            ButtonBackground = cs.SecondaryColor.ToBrush();
            TabBackground = cs.SecondaryColor.ToBrush();
            ControlPopBrush = cs.SelectionColor.ToBrush();
            ControlSatBrush = cs.MainColor.ToBrush();
            DisabledBackground = cs.LightDisabledColor.ToBrush();
            DisabledBorderBrush = cs.DarkDisabledColor.ToBrush();
            DisabledForeground = cs.DarkDisabledColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
            HighlightBrush = cs.ThirdHighlightColor.ToBrush();
            HighlightBorderBrush = cs.HighlightColor.ToBrush();
            HighlightForeground = cs.ForegroundColor.ToBrush();
            TabHighlightBrush = cs.ThirdHighlightColor.ToBrush();
            TabHighlightBorderBrush = cs.HighlightColor.ToBrush();
            LightBorderBrush = cs.HighlightColor.ToBrush();
            PanelBackground = cs.LightBackgroundColor.ToBrush();
            ControlBackground = cs.LightBackgroundColor.ToBrush();
            SelectedBackgroundBrush = cs.ThirdHighlightColor.ToBrush();
            SelectedBorderBrush = cs.SelectionColor.ToBrush();
            TabSelectedBrush = cs.LightBackgroundColor.ToBrush();
        }

        /// <summary>
        /// Create a new SsuiTheme by parsing a <see cref="SerializableSsuiTheme"/> object.
        /// </summary>
        /// <param name="theme">the object to parse</param>
        public SsuiTheme(SerializableSsuiTheme theme)
        {
            ButtonBackground = StringToBrush(theme.ButtonBackground);
            ControlBackground = StringToBrush(theme.ControlBackground);
            PanelBackground = StringToBrush(theme.PanelBackground);
            BaseBackground = StringToBrush(theme.BaseBackground);
            ControlSatBrush = StringToBrush(theme.ControlSatBrush);
            BorderBrush = StringToBrush(theme.BorderBrush);
            Foreground = StringToBrush(theme.Foreground);
            DisabledBackground = StringToBrush(theme.DisabledBackground);
            DisabledBorderBrush = StringToBrush(theme.DisabledBorderBrush);
            DisabledForeground = StringToBrush(theme.DisabledForeground);
            HighlightBrush = StringToBrush(theme.HighlightBrush);
            HighlightBorderBrush = StringToBrush(theme.HighlightBorderBrush);
            ClickBrush = StringToBrush(theme.ClickBrush);
            HighlightForeground = StringToBrush(theme.HighlightForeground);
            TabBackground = StringToBrush(theme.TabBackground);
            TabSelectedBrush = StringToBrush(theme.TabSelectedBrush);
            TabHighlightBrush = StringToBrush(theme.TabHighlightBrush);
            TabHighlightBorderBrush = StringToBrush(theme.TabHighlightBorderBrush);
            SelectedBackgroundBrush = StringToBrush(theme.SelectedBackgroundBrush);
            SelectedBorderBrush = StringToBrush(theme.SelectedBorderBrush);
            LightBorderBrush = StringToBrush(theme.LightBorderBrush);
            CheckBrush = StringToBrush(theme.CheckBrush);
            CheckHighlightBrush = StringToBrush(theme.CheckHighlightBrush);
            CheckBackgroundHighlightBrush = StringToBrush(theme.CheckBackgroundHighlightBrush);
            ControlPopBrush = StringToBrush(theme.ControlPopBrush);
            CommandBarBackground = StringToBrush(theme.CommandBarBackground);
            CommandBarBorderBrush = StringToBrush(theme.CommandBarBorderBrush);
            CornerRadius = ParseCornerRadius(theme.CornerRadius);

            if (Enum.TryParse<IconVariation>(theme.IconVariation, out var res))
            {
                IconVariation = res;
            }

            Brush StringToBrush(string s)
            {
                if (string.IsNullOrEmpty(s)) return Colors.Transparent.ToBrush();
                try
                {
                    return BrushSerializer.DeserializeBrush(s);
                }
                catch (FormatException)
                {
                    return Colors.Transparent.ToBrush();
                }
            }

            CornerRadius ParseCornerRadius(string s)
            {
                try
                {
                    CornerRadiusConverter tc = new CornerRadiusConverter();
#if NETCOREAPP
                    object? Maybeval = tc.ConvertFromString(null, CultureInfo.InvariantCulture, s);
#else
                    object Maybeval = tc.ConvertFromString(null, CultureInfo.InvariantCulture, s);
#endif
                    if (Maybeval is CornerRadius tt)
                    {
                        return tt;
                    }
                    else { throw new NotSupportedException(); } // this can also be thrown by CornerRadiusConverter if it can't do the conversion
                }
                catch (NotSupportedException)
                {
                    if (double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out double dval))
                    {
                        return new CornerRadius(dval);
                    }
                    else
                    {
                        return new CornerRadius(0);
                    }
                }
            }
        }

        void CreatePalette(Color baseColor)
        {
            // this palette building system works by pretty much taking the HSV values of the base color
            // and making alternate colors by changing the value by various amounts
            // and then creating clamps/bounds on saturation to prevent colors going too light
            //
            // amusingly, the base color itself isn't actually used for that much (for a SsuiAppTheme, it does become the title bar color)
            // so while a saturated, high value color is recommended for creating a palette, that saturated color itself won't appear that much

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

            ControlSatBrush = baseColor.ToBrush();
            //MainColor = baseColor;
            //WindowTitleBarColor = baseColor;
            //WindowInactiveColor = baseColor;
            HighlightBorderBrush = AddValue(h, s, v, vc1).ToBrush();
            TabHighlightBorderBrush = HighlightBorderBrush.CloneCurrentValue();
            SelectedBorderBrush = AddValue(h, s, v, vc2).ToBrush();
            ControlPopBrush = SelectedBorderBrush.CloneCurrentValue();
            BorderBrush = AddValue(h, s, v, vc3).ToBrush();
            LightBorderBrush = AddValue(h, s, v, vc1).ToBrush();

            // for the disabled brushes, we'll use the default values of the dependency properties

            // next, we'll create a couple brushes by making sure the saturation is within a few bounds
            // and then using either the actual saturation or the bounded value for these brushes

            if (s > ssc)
            {
                ButtonBackground = AddValue(h, ssc, v, t1).ToBrush(); // 4 - Secondary Color
                TabBackground = ButtonBackground.CloneCurrentValue();
            }
            else
            {
                ButtonBackground = AddValue(h, s, v, t1).ToBrush(); // 4
                TabBackground = ButtonBackground.CloneCurrentValue();
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
                ClickBrush = SelectedBackgroundBrush.CloneCurrentValue();
            }
            else
            {
                SelectedBackgroundBrush = AddValue(h, s, v, t3).ToBrush(); // 6
                ClickBrush = SelectedBackgroundBrush.CloneCurrentValue();
            }

            if (s > stc)
            {
                HighlightBrush = AddValue(h, stc, v, t3).ToBrush(); // 7 - Menu Highlight Color
                TabHighlightBrush = HighlightBrush.CloneCurrentValue();
            }
            else
            {
                HighlightBrush = AddValue(h, s, v, t3).ToBrush(); // 7
                TabHighlightBrush = HighlightBrush.CloneCurrentValue();
            }

            if (s > sbc)
            {
                PanelBackground = AddValue(h, sbc, v, t4).ToBrush(); // 8 - Menu Background Color
                ControlBackground = PanelBackground.CloneCurrentValue();
                TabSelectedBrush = PanelBackground.CloneCurrentValue();
            }
            else
            {
                PanelBackground = AddValue(h, s, v, t4).ToBrush(); // 8
                ControlBackground = PanelBackground.CloneCurrentValue();
                TabSelectedBrush = PanelBackground.CloneCurrentValue();
            }

            // finally, we'll want to check the foreground against the base background,
            float bBase = GetColorBrightness(((SolidColorBrush)BaseBackground).Color);
            Foreground = (bBase < 0.55 ? Colors.White : Colors.Black).ToBrush();

            // the highlight foreground against the highlight brush,
            float bHigh = GetColorBrightness(((SolidColorBrush)HighlightBrush).Color);
            HighlightForeground = (bHigh < 0.55 ? Colors.LightGray : ColorsHelper.DarkerGray).ToBrush();

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

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the brush to use for the background of SSUI's button controls. This brush is meant to "pop out" from the background,
        /// versus <see cref="ControlBackground"/> or <see cref="PanelBackground"/> that should appear to "sink in".
        /// </summary>
        public Brush ButtonBackground { get => (Brush)GetValue(ButtonBackgroundProperty); set => SetValue(ButtonBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonBackgroundProperty
            = DependencyProperty.Register(nameof(ButtonBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of most SSUI-themed controls that aren't buttons or multi-item panels.
        /// This brush should seem to "sink in" to the surrounding background, generally achieved by using a lighter or brighter color.
        /// </summary>
        public Brush ControlBackground { get => (Brush)GetValue(ControlBackgroundProperty); set => SetValue(ControlBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ControlBackgroundProperty
            = DependencyProperty.Register(nameof(ControlBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of panel controls, such as <see cref="SelectPanel"/>, <see cref="TabControl"/>,
        /// and <see cref="PropertyList.PropertyList"/>. This brush should seem to "sink in" to the surrounding background, rather than "popping out".
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
        /// Get or set the brush to use for saturated parts or elements of certain controls.
        /// <para/>
        /// If this SsuiTheme was created by using a base color (via using <c>new </c><see cref="SsuiTheme(Color)"/>), this brush will also be set to that base color.
        /// </summary>
        public Brush ControlSatBrush { get => (Brush)GetValue(ControlSatBrushProperty); set => SetValue(ControlSatBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlSatBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ControlSatBrushProperty
            = DependencyProperty.Register(nameof(ControlSatBrush), typeof(Brush), typeof(SsuiTheme),
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
            new FrameworkPropertyMetadata(ColorsHelper.DarkerGray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the tabs in a tabbed control like <see cref="TabControl"/> when they are not selected.
        /// </summary>
        /// <remarks>
        /// Note that individual tabs' backgrounds in a TabControl can be recolored by using <see cref="TabItem.TabBackground"/>.
        /// </remarks>
        public Brush TabBackground { get => (Brush)GetValue(TabBackgroundProperty); set => SetValue(TabBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TabBackgroundProperty
            = DependencyProperty.Register(nameof(TabBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>
        /// The brush to use for the background of a tab while it is selected, such as tabs in a <see cref="TabControl"/> or a <c>Ribbon</c>.
        /// </summary>
        public Brush TabSelectedBrush { get => (Brush)GetValue(TabSelectedBrushProperty); set => SetValue(TabSelectedBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabSelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TabSelectedBrushProperty
            = DependencyProperty.Register(nameof(TabSelectedBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// The brush to use for the background of a tab while it is higlighted (e.g., mouse over), such as tabs in a <see cref="TabControl"/> or a <c>Ribbon</c>.
        /// </summary>
        public Brush TabHighlightBrush { get => (Brush)GetValue(TabHighlightBrushProperty); set => SetValue(TabHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TabHighlightBrushProperty
            = DependencyProperty.Register(nameof(TabHighlightBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>
        /// The brush to use for the border around a tab while it is higlighted (e.g., mouse over), such as tabs in a <see cref="TabControl"/> or a <c>Ribbon</c>.
        /// </summary>
        public Brush TabHighlightBorderBrush { get => (Brush)GetValue(TabHighlightBorderBrushProperty); set => SetValue(TabHighlightBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabHighlightBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TabHighlightBorderBrushProperty
            = DependencyProperty.Register(nameof(TabHighlightBorderBrush), typeof(Brush), typeof(SsuiTheme),
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
        /// Get or set the brush to use for checkmark symbols in certain SSUI-themed controls such as <see cref="CheckBox"/>, while higlighted (e.g., mouse over).
        /// </summary>
        public Brush CheckHighlightBrush { get => (Brush)GetValue(CheckHighlightBrushProperty); set => SetValue(CheckHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CheckHighlightBrushProperty
            = DependencyProperty.Register(nameof(CheckHighlightBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(ColorsHelper.DarkerGray.ToBrush()));

        /// <summary>
        /// Get or set the brush to use in the background behind checkmark symbols in certain SSUI-themed controls such as <see cref="CheckBox"/>, while highlighted (e.g., mouse over).
        /// </summary>
        public Brush CheckBackgroundHighlightBrush { get => (Brush)GetValue(CheckBackgroundHighlightBrushProperty); set => SetValue(CheckBackgroundHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CheckBackgroundHighlightBrushProperty
            = DependencyProperty.Register(nameof(CheckBackgroundHighlightBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(ColorsHelper.WhiteLightHighlight.ToBrush()));

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

        /// <summary>
        /// The brush to use for the background of command bars, such as toolbars or the main bar of Ribbons.
        /// <para/>
        /// No controls in Solid Shine UI 2.0 use this brush automatically, but it can be used for other SSUI-themed controls.
        /// This is included for forward compatibility with future versions.
        /// </summary>
        public Brush CommandBarBackground { get => (Brush)GetValue(CommandBarBackgroundProperty); set => SetValue(CommandBarBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="CommandBarBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CommandBarBackgroundProperty
            = DependencyProperty.Register(nameof(CommandBarBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));


        /// <summary>
        /// The brush to use for the border around command bars, such as toolbars or the main bar of Ribbons.
        /// <para/>
        /// No controls in Solid Shine UI 2.0 use this brush automatically, but it can be used for other SSUI-themed controls.
        /// This is included for forward compatibility with future versions.
        /// </summary>
        public Brush CommandBarBorderBrush { get => (Brush)GetValue(CommandBarBorderBrushProperty); set => SetValue(CommandBarBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="CommandBarBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CommandBarBorderBrushProperty
            = DependencyProperty.Register(nameof(CommandBarBorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.DarkGray.ToBrush()));

        #endregion

        #region Freezable Methods

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new SsuiTheme();
        }

        /// <summary>
        /// Creates a modifiable clone of the SsuiTheme, making deep copies of the object's values. When copying the object's dependency properties,
        /// this method copies expressions (which might no longer resolve) but not animations or their current values.
        /// <para/>
        /// See <see cref="Freezable.Clone"/>.
        /// </summary>
        public new SsuiTheme Clone()
        {
            return (SsuiTheme)base.Clone();
        }

        /// <summary>
        /// Creates a modifiable clone (deep copy) of the SsuiTheme using its current values.
        /// <para/>
        /// See <see cref="Freezable.CloneCurrentValue"/>.
        /// </summary>
        public new SsuiTheme CloneCurrentValue()
        {
            return (SsuiTheme)base.CloneCurrentValue();
        }

        #endregion

        #region Helper Methods

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

        /// <summary>
        /// Create a <see cref="SerializableSsuiTheme"/> object that is built to be used in serialization scenarios, such as storing in a settings file.
        /// </summary>
        /// <remarks>
        /// As SsuiTheme is a WPF Freezable object, it includes properties and 
        /// </remarks>
        public SerializableSsuiTheme ToSerializableObject()
        {
            return SerializableSsuiTheme.FromSsuiTheme(this);
        }

        /// <summary>
        /// Create a copy of this SsuiTheme.
        /// </summary>
        /// <remarks>
        /// Only created this for niche situations. Most people should use <see cref="Clone"/> or <see cref="CloneCurrentValue"/>, to utilize the functionality provided by Freezable.
        /// </remarks>
        internal protected SsuiTheme Copy()
        {
            return new SsuiTheme
            {
                ButtonBackground = this.ButtonBackground,
                ControlBackground = this.ControlBackground,
                PanelBackground = this.PanelBackground,
                BaseBackground = this.BaseBackground,
                ControlSatBrush = this.ControlSatBrush,
                BorderBrush = this.BorderBrush,
                Foreground = this.Foreground,
                DisabledBackground = this.DisabledBackground,
                DisabledBorderBrush = this.DisabledBorderBrush,
                DisabledForeground = this.DisabledForeground,
                HighlightBrush = this.HighlightBrush,
                HighlightBorderBrush = this.HighlightBorderBrush,
                ClickBrush = this.ClickBrush,
                HighlightForeground = this.HighlightForeground,
                TabBackground = this.TabBackground,
                TabSelectedBrush = this.TabSelectedBrush,
                TabHighlightBrush = this.TabHighlightBrush,
                TabHighlightBorderBrush = this.TabHighlightBorderBrush,
                SelectedBackgroundBrush = this.SelectedBackgroundBrush,
                SelectedBorderBrush = this.SelectedBorderBrush,
                LightBorderBrush = this.LightBorderBrush,
                CheckBrush = this.CheckBrush,
                CheckHighlightBrush = this.CheckHighlightBrush,
                CheckBackgroundHighlightBrush = this.CheckBackgroundHighlightBrush,
                ControlPopBrush = this.ControlPopBrush,
                CommandBarBackground = this.CommandBarBackground,
                CommandBarBorderBrush = this.CommandBarBorderBrush,
                CornerRadius = this.CornerRadius,
                IconVariation = this.IconVariation,
            };

        }

        #endregion

    }


    /// <summary>
    /// A class containing various brushes and other settings that can be used to set the appearance of SSUI-themed controls, windows, and entire applications.
    /// </summary>
    /// <remarks>
    /// This has extra properties and options that can be used to apply to windows, and to allow features like an accent theme or subitem theme.
    /// <para/>
    /// Note that this class is not meant to be used for serialization. Use the <c>ToSerializableObject()</c> method to create a copy of this
    /// that can be used for serialization scenarios, like storing in a settings file.
    /// </remarks>
    public class SsuiAppTheme : SsuiTheme
    {

        #region Constructors / Create Palette

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
        /// Create a new SsuiAppTheme by adapting a Solid Shine UI 1.x ColorScheme object.
        /// </summary>
        /// <param name="cs">the ColorScheme object to adapt from</param>
        /// <remarks>
        /// It is recommended for new programs to create and use a SsuiAppTheme using one of the other constructors, rather than
        /// using a ColorScheme. This constructor is primarily provided as an upgrade path for those upgrading their app from SSUI 1.9.
        /// <para/>
        /// This creates a SsuiAppTheme by assigning brush values from the colors of the ColorScheme, with the goal of closely matching
        /// the visual appearance of how the ColorScheme's colors were used within SSUI's controls.
        /// If the ColorScheme <paramref name="cs"/> is a high contrast theme, this may create undesirable results. Instead, you should 
        /// create a high contrast SsuiTheme using <see cref="SsuiThemes.GetHighContrastTheme(HighContrastOption)"/>.
        /// </remarks>
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

        /// <summary>
        /// Create a new SsuiAppTheme by parsing a <see cref="SerializableSsuiTheme"/> object.
        /// </summary>
        /// <param name="theme">the object to parse</param>
        public SsuiAppTheme(SerializableSsuiTheme theme)
        {
            ButtonBackground = StringToBrush(theme.ButtonBackground);
            ControlBackground = StringToBrush(theme.ControlBackground);
            PanelBackground = StringToBrush(theme.PanelBackground);
            BaseBackground = StringToBrush(theme.BaseBackground);
            ControlSatBrush = StringToBrush(theme.ControlSatBrush);
            BorderBrush = StringToBrush(theme.BorderBrush);
            Foreground = StringToBrush(theme.Foreground);
            DisabledBackground = StringToBrush(theme.DisabledBackground);
            DisabledBorderBrush = StringToBrush(theme.DisabledBorderBrush);
            DisabledForeground = StringToBrush(theme.DisabledForeground);
            HighlightBrush = StringToBrush(theme.HighlightBrush);
            HighlightBorderBrush = StringToBrush(theme.HighlightBorderBrush);
            ClickBrush = StringToBrush(theme.ClickBrush);
            HighlightForeground = StringToBrush(theme.HighlightForeground);
            TabBackground = StringToBrush(theme.TabBackground);
            TabSelectedBrush = StringToBrush(theme.TabSelectedBrush);
            TabHighlightBrush = StringToBrush(theme.TabHighlightBrush);
            TabHighlightBorderBrush = StringToBrush(theme.TabHighlightBorderBrush);
            SelectedBackgroundBrush = StringToBrush(theme.SelectedBackgroundBrush);
            SelectedBorderBrush = StringToBrush(theme.SelectedBorderBrush);
            LightBorderBrush = StringToBrush(theme.LightBorderBrush);
            CheckBrush = StringToBrush(theme.CheckBrush);
            CheckHighlightBrush = StringToBrush(theme.CheckHighlightBrush);
            CheckBackgroundHighlightBrush = StringToBrush(theme.CheckBackgroundHighlightBrush);
            ControlPopBrush = StringToBrush(theme.ControlPopBrush);
            CommandBarBackground = StringToBrush(theme.CommandBarBackground);
            CommandBarBorderBrush = StringToBrush(theme.CommandBarBorderBrush);
            CornerRadius = ParseCornerRadius(theme.CornerRadius);

            WindowTitleBackground = StringToBrush(theme.WindowTitleBackground);
            WindowTitleForeground = StringToBrush(theme.WindowTitleForeground);
            WindowInactiveBackground = StringToBrush(theme.WindowInactiveBackground);
            WindowInactiveForeground = StringToBrush(theme.WindowInactiveForeground);
            WindowCaptionsBackground = StringToBrush(theme.WindowCaptionsBackground);
            WindowCaptionsForeground = StringToBrush(theme.WindowCaptionsForeground);
            WindowCaptionsHighlight = StringToBrush(theme.WindowCaptionsHighlight);
            WindowCaptionsHighlightForeground = StringToBrush(theme.WindowCaptionsHighlightForeground);
            WindowCaptionsClickBrush = StringToBrush(theme.WindowCaptionsClickBrush);
            WindowBackground = StringToBrush(theme.WindowBackground);

            UseSubitemThemeWithMenus = theme.UseSubitemThemeWithMenus;
            UseSubitemThemeWithPanels = theme.UseSubitemThemeWithPanels;
            UseSubitemThemeWithRibbons = theme.UseSubitemThemeWithRibbons;

            if (!string.IsNullOrEmpty(theme.IconVariation) && Enum.TryParse<IconVariation>(theme.IconVariation, out var iv))
            {
                IconVariation = iv;
            }

            if (theme.AccentTheme != null)
            {
                AccentTheme = theme.AccentTheme.ToSsuiTheme();
            }
            if (theme.SubitemTheme != null)
            {
                SubitemTheme = theme.SubitemTheme.ToSsuiTheme();
            }

            Brush StringToBrush(string s)
            {
                if (string.IsNullOrEmpty(s)) return Colors.Transparent.ToBrush();
                try
                {
                    return BrushSerializer.DeserializeBrush(s);
                }
                catch (FormatException)
                {
                    return Colors.Transparent.ToBrush();
                }
            }

            CornerRadius ParseCornerRadius(string s)
            {
                try
                {
                    CornerRadiusConverter tc = new CornerRadiusConverter();
#if NETCOREAPP
                    object? Maybeval = tc.ConvertFromString(null, CultureInfo.InvariantCulture, s);
#else
                    object Maybeval = tc.ConvertFromString(null, CultureInfo.InvariantCulture, s);
#endif
                    if (Maybeval is CornerRadius tt)
                    {
                        return tt;
                    }
                    else { throw new NotSupportedException(); } // this can also be thrown by CornerRadiusConverter if it can't do the conversion
                }
                catch (NotSupportedException)
                {
                    if (double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out double dval))
                    {
                        return new CornerRadius(dval);
                    }
                    else
                    {
                        return new CornerRadius(0);
                    }
                }
            }
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

        #endregion

        #region Properties

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

        #endregion

        #region Accent / Subitem Themes

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
        /// Get or set if the <see cref="SubitemTheme"/> should be used for the <c>Ribbon</c> control (not included in Solid Shine UI 2.0).
        /// </summary>
        public bool UseSubitemThemeWithRibbons { get => (bool)GetValue(UseSubitemThemeWithRibbonsProperty); set => SetValue(UseSubitemThemeWithRibbonsProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseSubitemThemeWithRibbons"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseSubitemThemeWithRibbonsProperty
            = DependencyProperty.Register(nameof(UseSubitemThemeWithRibbons), typeof(bool), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(true));

        #endregion

        #region Freezable Methods

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new SsuiAppTheme();
        }

        /// <summary>
        /// Creates a modifiable clone of the SsuiAppTheme, making deep copies of the object's values. When copying the object's dependency properties,
        /// this method copies expressions (which might no longer resolve) but not animations or their current values.
        /// <para/>
        /// See <see cref="Freezable.Clone"/>.
        /// </summary>
        public new SsuiAppTheme Clone()
        {
            return (SsuiAppTheme)base.Clone();
        }

        /// <summary>
        /// Creates a modifiable clone (deep copy) of the SsuiAppTheme using its current values.
        /// <para/>
        /// See <see cref="Freezable.CloneCurrentValue"/>.
        /// </summary>
        public new SsuiAppTheme CloneCurrentValue()
        {
            return (SsuiAppTheme)base.CloneCurrentValue();
        }

        #endregion

    }
}
