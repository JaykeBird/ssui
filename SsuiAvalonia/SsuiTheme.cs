using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Data;
using Avalonia.Media;

namespace SolidShineUi
{

    /// <summary>
    /// A class containing various brushes and other settings that can be used to set the appearance of various Solid Shine UI controls.
    /// </summary>
    /// <remarks>
    /// This can be used in all SSUI-themed controls. For using with SSUI-themed windows (see <see cref="ThemedWindow"/>)
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
            CheckBrush = Colors.Black.ToBrush();
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
            CommandBarBackground = cs.LightBackgroundColor.ToBrush();
            CommandBarBorderBrush = cs.BorderColor.ToBrush();
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
            HighlightForeground = StringToBrush(theme.HighlightForeground);
            ClickBrush = StringToBrush(theme.ClickBrush);
            TabBackground = StringToBrush(theme.TabBackground);
            TabSelectedBrush = StringToBrush(theme.TabSelectedBrush);
            TabHighlightBrush = StringToBrush(theme.TabHighlightBrush);
            TabHighlightBorderBrush = StringToBrush(theme.TabHighlightBorderBrush);
            SelectedBackgroundBrush = StringToBrush(theme.SelectedBackgroundBrush);
            SelectedBorderBrush = StringToBrush(theme.SelectedBorderBrush);
            SelectedForeground = StringToBrush(theme.SelectedForeground);
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

            IBrush StringToBrush(string s)
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
                    return CornerRadius.Parse(s);
                }
                catch (FormatException)
                {
                    return new CornerRadius(0);
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
            TabHighlightBorderBrush = HighlightBorderBrush.Clone();
            SelectedBorderBrush = AddValue(h, s, v, vc2).ToBrush();
            ControlPopBrush = SelectedBorderBrush.Clone();
            BorderBrush = AddValue(h, s, v, vc3).ToBrush();
            LightBorderBrush = AddValue(h, s, v, vc1).ToBrush();

            // for the disabled brushes, we'll use the default values of the dependency properties

            // next, we'll create a couple brushes by making sure the saturation is within a few bounds
            // and then using either the actual saturation or the bounded value for these brushes

            if (s > ssc)
            {
                ButtonBackground = AddValue(h, ssc, v, t1).ToBrush(); // 4 - Secondary Color
                TabBackground = ButtonBackground.Clone();
            }
            else
            {
                ButtonBackground = AddValue(h, s, v, t1).ToBrush(); // 4
                TabBackground = ButtonBackground.Clone();
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
                ClickBrush = SelectedBackgroundBrush.Clone();
            }
            else
            {
                SelectedBackgroundBrush = AddValue(h, s, v, t3).ToBrush(); // 6
                ClickBrush = SelectedBackgroundBrush.Clone();
            }

            if (s > stc)
            {
                HighlightBrush = AddValue(h, stc, v, t3).ToBrush(); // 7 - Menu Highlight Color
                TabHighlightBrush = HighlightBrush.Clone();
            }
            else
            {
                HighlightBrush = AddValue(h, s, v, t3).ToBrush(); // 7
                TabHighlightBrush = HighlightBrush.Clone();
            }

            if (s > sbc)
            {
                PanelBackground = AddValue(h, sbc, v, t4).ToBrush(); // 8 - Menu Background Color
                ControlBackground = PanelBackground.Clone();
                TabSelectedBrush = PanelBackground.Clone();
            }
            else
            {
                PanelBackground = AddValue(h, s, v, t4).ToBrush(); // 8
                ControlBackground = PanelBackground.Clone();
                TabSelectedBrush = PanelBackground.Clone();
            }

            // finally, we'll want to check the foreground against the base background,
            float bBase = GetColorBrightness(((SolidColorBrush)BaseBackground).Color);
            Foreground = (bBase < 0.55 ? Colors.White : Colors.Black).ToBrush();

            // the highlight foreground against the highlight brush,
            float bHigh = GetColorBrightness(((SolidColorBrush)HighlightBrush).Color);
            HighlightForeground = (bHigh < 0.55 ? Colors.LightGray : ColorsHelper.DarkerGray).ToBrush();

            // the highlight foreground against the highlight brush,
            float bSel = GetColorBrightness(((SolidColorBrush)SelectedBackgroundBrush).Color);
            SelectedForeground = (bSel < 0.55 ? Colors.White : ColorsHelper.Black).ToBrush();

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
        public IBrush ButtonBackground { get => GetValue(ButtonBackgroundProperty); set => SetValue(ButtonBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="ButtonBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ButtonBackgroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(ButtonBackground), Colors.Gainsboro.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of most SSUI-themed controls that aren't buttons or multi-item panels.
        /// This brush should seem to "sink in" to the surrounding background, generally achieved by using a lighter or brighter color.
        /// </summary>
        public IBrush ControlBackground { get => GetValue(ControlBackgroundProperty); set => SetValue(ControlBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="ControlBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ControlBackgroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(ControlBackground), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of panel controls, such as tab controls and property lists.
        /// This brush should seem to "sink in" to the surrounding background, rather than "popping out".
        /// </summary>
        public IBrush PanelBackground { get => GetValue(PanelBackgroundProperty); set => SetValue(PanelBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="PanelBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> PanelBackgroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(PanelBackground), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of an entire underlying surface, pane, window, or area. This can also be used for
        /// the background of controls that should neither "pop out" or "sink in" to the underlying background.
        /// </summary>
        public IBrush BaseBackground { get => GetValue(BaseBackgroundProperty); set => SetValue(BaseBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="BaseBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> BaseBackgroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(BaseBackground), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the brush to use for saturated parts or elements of certain controls.
        /// </summary>
        public IBrush ControlSatBrush { get => GetValue(ControlSatBrushProperty); set => SetValue(ControlSatBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ControlSatBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ControlSatBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(ControlSatBrush), Colors.Gray.ToBrush());

        /// <summary>
        /// Get or set the brush to use for borders around the edges of most SSUI-themed controls.
        /// </summary>
        public IBrush BorderBrush { get => GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> BorderBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(BorderBrush), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush to use for foreground elements in SSUI-themed controls (such as text or certain symbols or icons).
        /// </summary>
        public IBrush Foreground { get => GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="Foreground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ForegroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(Foreground), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of disabled SSUI-themed controls.
        /// </summary>
        public IBrush DisabledBackground { get => GetValue(DisabledBackgroundProperty); set => SetValue(DisabledBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> DisabledBackgroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(DisabledBackground), Color.Parse("F5F5F5").ToBrush());

        /// <summary>
        /// Get or set the brush to use for the borders around the edges of disabled SSUI-themed controls.
        /// </summary>
        public IBrush DisabledBorderBrush { get => GetValue(DisabledBorderBrushProperty); set => SetValue(DisabledBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> DisabledBorderBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(DisabledBorderBrush), Color.Parse("AAAAAF").ToBrush());

        /// <summary>
        /// Get or set the brush to use for the foreground elements of disabled SSUI-themed controls.
        /// </summary>
        public IBrush DisabledForeground { get => GetValue(DisabledForegroundProperty); set => SetValue(DisabledForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> DisabledForegroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(DisabledForeground), Color.Parse("AAAAAF").ToBrush());

        /// <summary>
        /// Get or set the brush to use for when a SSUI-themed control is highlighted (e.g. mouse over, keyboard focus).
        /// </summary>
        public IBrush HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> HighlightBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(HighlightBrush), Colors.Gray.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the border around the edges of a SSUI-themed control while it is highlighted
        /// (e.g. mouse over, keyboard focus).
        /// </summary>
        public IBrush HighlightBorderBrush { get => GetValue(HighlightBorderBrushProperty); set => SetValue(HighlightBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> HighlightBorderBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(HighlightBorderBrush), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush to use for foreground elements of a SSUI-themed control while it is being highlighted.
        /// </summary>
        public IBrush HighlightForeground { get => GetValue(HighlightForegroundProperty); set => SetValue(HighlightForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> HighlightForegroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(HighlightForeground), Color.Parse("414141").ToBrush());

        /// <summary>
        /// Get or set the brush to use for when a SSUI-themed control is being clicked/pressed.
        /// </summary>
        public IBrush ClickBrush { get => GetValue(ClickBrushProperty); set => SetValue(ClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ClickBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(ClickBrush), Colors.LightGray.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of SSUI-themed controls while they are selected.
        /// </summary>
        public IBrush SelectedBackgroundBrush { get => GetValue(SelectedBackgroundBrushProperty); set => SetValue(SelectedBackgroundBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedBackgroundBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> SelectedBackgroundBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(SelectedBackgroundBrush), Colors.Gray.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the border around the edges of SSUI-themed controls while they are selected.
        /// </summary>
        public IBrush SelectedBorderBrush { get => GetValue(SelectedBorderBrushProperty); set => SetValue(SelectedBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> SelectedBorderBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(SelectedBorderBrush), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the foreground of SSUI-themed controls while they are selected.
        /// </summary>
        public IBrush SelectedForeground { get => GetValue(SelectedForegroundProperty); set => SetValue(SelectedForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> SelectedForegroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(SelectedForeground), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the borders around the edges of SSUI-themed controls where a lighter border color is used.
        /// </summary>
        public IBrush LightBorderBrush { get => GetValue(LightBorderBrushProperty); set => SetValue(LightBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="LightBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> LightBorderBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(LightBorderBrush), Colors.DarkGray.ToBrush());

        /// <summary>
        /// Get or set the brush to use for key elements in certain SSUI-themed controls that should be distinguished or stand out.
        /// </summary>
        public IBrush ControlPopBrush { get => GetValue(ControlPopBrushProperty); set => SetValue(ControlPopBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ControlPopBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ControlPopBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(ControlPopBrush), Colors.DarkGray.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of the tabs in a tabbed control when they are not selected.
        /// </summary>
        public IBrush TabBackground { get => GetValue(TabBackgroundProperty); set => SetValue(TabBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="TabBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabBackgroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(TabBackground), Colors.Gainsboro.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of a tab while it is selected.
        /// </summary>
        public IBrush TabSelectedBrush { get => GetValue(TabSelectedBrushProperty); set => SetValue(TabSelectedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="TabSelectedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabSelectedBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(TabSelectedBrush), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of a tab while it is highlighted (e.g., mouse over).
        /// </summary>
        public IBrush TabHighlightBrush { get => GetValue(TabHighlightBrushProperty); set => SetValue(TabHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="TabHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabHighlightBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(TabHighlightBrush), Colors.Gainsboro.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the border around a tab while it is highlighted (e.g., mouse over).
        /// </summary>
        public IBrush TabHighlightBorderBrush { get => GetValue(TabHighlightBorderBrushProperty); set => SetValue(TabHighlightBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="TabHighlightBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabHighlightBorderBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(TabHighlightBorderBrush), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush to use for checkmark symbols in certain SSUI-themed controls.
        /// </summary>
        public IBrush CheckBrush { get => GetValue(CheckBrushProperty); set => SetValue(CheckBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> CheckBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(CheckBrush), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush to use for checkmark symbols in certain SSUI-themed controls while highlighted (e.g., mouse over).
        /// </summary>
        public IBrush CheckHighlightBrush { get => GetValue(CheckHighlightBrushProperty); set => SetValue(CheckHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> CheckHighlightBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(CheckHighlightBrush), Color.Parse("414141").ToBrush());

        /// <summary>
        /// Get or set the brush to use in the background behind checkmark symbols in certain SSUI-themed controls while highlighted (e.g., mouse over).
        /// </summary>
        public IBrush CheckBackgroundHighlightBrush { get => GetValue(CheckBackgroundHighlightBrushProperty); set => SetValue(CheckBackgroundHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBackgroundHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> CheckBackgroundHighlightBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(CheckBackgroundHighlightBrush), new SolidColorBrush(Color.FromArgb(16, 255, 255, 255)));

        /// <summary>
        /// Get or set the brush to use for the background of command bars, such as toolbars.
        /// </summary>
        public IBrush CommandBarBackground { get => GetValue(CommandBarBackgroundProperty); set => SetValue(CommandBarBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="CommandBarBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> CommandBarBackgroundProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(CommandBarBackground), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the border around command bars, such as toolbars.
        /// </summary>
        public IBrush CommandBarBorderBrush { get => GetValue(CommandBarBorderBrushProperty); set => SetValue(CommandBarBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CommandBarBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> CommandBarBorderBrushProperty
            = AvaloniaProperty.Register<SsuiTheme, IBrush>(nameof(CommandBarBorderBrush), Colors.DarkGray.ToBrush());

        /// <summary>
        /// Get or set the corner radius to use around the edges of many SSUI-themed controls.
        /// </summary>
        /// <remarks>
        /// A corner radius value of 0 will result in completely square 90-degree angle corners; values above 0
        /// will result in increasingly more rounded corners.
        /// </remarks>
        public CornerRadius CornerRadius { get => GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        /// <summary>The backing styled property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty
            = AvaloniaProperty.Register<SsuiTheme, CornerRadius>(nameof(CornerRadius), new CornerRadius(0));

        /// <summary>
        /// Get or set the variation to use for image-based icons in various SSUI-themed controls.
        /// </summary>
        public IconVariation IconVariation { get => GetValue(IconVariationProperty); set => SetValue(IconVariationProperty, value); }

        /// <summary>The backing styled property for <see cref="IconVariation"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IconVariation> IconVariationProperty
            = AvaloniaProperty.Register<SsuiTheme, IconVariation>(nameof(IconVariation), IconVariation.Color);


        #endregion

        #region Helper Methods

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
        /// Create a <see cref="ReflectionBinding"/> for a property in the SsuiTheme.
        /// </summary>
        /// <param name="ssuiThemeProperty">the SsuiTheme or SsuiAppTheme property to bind to</param>
        /// <param name="source">the SsuiTheme object holding the value to bind</param>
        /// <param name="priority">the binding priority to use; if unsure, keep this as "Style"</param>
        /// <returns>A <see cref="ReflectionBinding"/> object that can be used to bind other controls' properties to this property.</returns>
        /// <exception cref="ArgumentException">thrown if <paramref name="ssuiThemeProperty"/> is not a SsuiTheme property, or a property from a class that inherits from SsuiTheme</exception>
        public static ReflectionBinding CreateBinding(AvaloniaProperty ssuiThemeProperty, SsuiTheme source,
            BindingPriority priority = BindingPriority.Style)
        {
            if (ssuiThemeProperty.OwnerType != typeof(SsuiTheme) && !ssuiThemeProperty.OwnerType.IsSubclassOf(typeof(SsuiTheme)))
            {
                throw new ArgumentException("This property is not an SsuiTheme property", nameof(ssuiThemeProperty));
            }
            return new ReflectionBinding(ssuiThemeProperty.Name) { Source = source, Priority = priority };

            // in the future, I could create a small cache of Binding objects that the SsuiTheme itself stores and returns, rather than creating a new binding each time
            // what I don't know, though, is whether each binding expression requires a unique Binding object, or if I can reuse Binding objects
        }

        /// <summary>
        /// Create a <see cref="CompiledBinding"/> for a property in the SsuiTheme.
        /// </summary>
        /// <param name="ssuiThemeProperty">the SsuiTheme or SsuiAppTheme property to bind to</param>
        /// <param name="source">the SsuiTheme object holding the value to bind</param>
        /// <param name="priority">the binding priority to use; if unsure, keep this as "Style"</param>
        /// <returns>A <see cref="CompiledBinding"/> object that can be used to bind other controls' properties to this property.</returns>
        /// <exception cref="ArgumentException">thrown if <paramref name="ssuiThemeProperty"/> is not a SsuiTheme property, or a property from a class that inherits from SsuiTheme</exception>
        public static CompiledBinding CreateCompiledBinding<T>(AvaloniaProperty<T> ssuiThemeProperty, SsuiTheme source,
            BindingPriority priority = BindingPriority.Style)
        {
            if (ssuiThemeProperty.OwnerType != typeof(SsuiTheme) && !ssuiThemeProperty.OwnerType.IsSubclassOf(typeof(SsuiTheme)))
            {
                throw new ArgumentException("This property is not an SsuiTheme property", nameof(ssuiThemeProperty));
            }

            return CompiledBinding.Create<SsuiTheme, T>(expression: theme => theme.GetValue<T>(ssuiThemeProperty), source: source, priority: priority);
        }

        /// <summary>
        /// Creates a copy of this SsuiTheme with all property values copied to the new instance.
        /// </summary>
        /// <returns>A new SsuiTheme instance with the same property values as this instance.</returns>
        public virtual SsuiTheme Copy()
        {
            var copy = new SsuiTheme
            {
                ButtonBackground = ButtonBackground,
                ControlBackground = ControlBackground,
                PanelBackground = PanelBackground,
                BaseBackground = BaseBackground,
                ControlSatBrush = ControlSatBrush,
                BorderBrush = BorderBrush,
                Foreground = Foreground,
                DisabledBackground = DisabledBackground,
                DisabledBorderBrush = DisabledBorderBrush,
                DisabledForeground = DisabledForeground,
                HighlightBrush = HighlightBrush,
                HighlightBorderBrush = HighlightBorderBrush,
                HighlightForeground = HighlightForeground,
                ClickBrush = ClickBrush,
                SelectedBackgroundBrush = SelectedBackgroundBrush,
                SelectedBorderBrush = SelectedBorderBrush,
                SelectedForeground = SelectedForeground,
                LightBorderBrush = LightBorderBrush,
                ControlPopBrush = ControlPopBrush,
                TabBackground = TabBackground,
                TabSelectedBrush = TabSelectedBrush,
                TabHighlightBrush = TabHighlightBrush,
                TabHighlightBorderBrush = TabHighlightBorderBrush,
                CheckBrush = CheckBrush,
                CheckHighlightBrush = CheckHighlightBrush,
                CheckBackgroundHighlightBrush = CheckBackgroundHighlightBrush,
                CommandBarBackground = CommandBarBackground,
                CommandBarBorderBrush = CommandBarBorderBrush,
                CornerRadius = CornerRadius,
                IconVariation = IconVariation,
            };
            return copy;
        }

        #endregion
    }

    /// <summary>
    /// A class containing various brushes and other settings for SSUI-themed controls, windows, and entire applications.
    /// This class extends <see cref="SsuiTheme"/> with additional properties and options for windows and application-wide theming.
    /// </summary>
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
            WindowTitleForeground = cs.WindowTitleBarTextColor.ToBrush();
            WindowInactiveBackground = cs.WindowInactiveColor.ToBrush();
            WindowInactiveForeground = cs.WindowTitleBarTextColor.ToBrush();
            WindowCaptionsBackground = cs.WindowTitleBarColor.ToBrush();
            WindowCaptionsForeground = cs.WindowTitleBarTextColor.ToBrush();
            WindowCaptionsHighlight = cs.HighlightColor.ToBrush();
            WindowCaptionsHighlightForeground = cs.WindowTitleBarTextColor.ToBrush();
            WindowCaptionsClickBrush = cs.SelectionColor.ToBrush();
            WindowBackground = cs.BackgroundColor.ToBrush();

            AccentTheme = new SsuiTheme(cs.AccentMainColor);

            if (cs.MenusUseAccent)
            {
                SubitemTheme = new SsuiTheme(cs.AccentMainColor);
                UseSubitemThemeWithMenus = true;
            }
            else
            {
                SubitemTheme = new SsuiTheme(cs.MainColor);
                UseSubitemThemeWithMenus = false;
            }
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
            AllowTitleBarBrushWithMenus = theme.AllowTitleBarBrushWithMenus;

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

            IBrush StringToBrush(string s)
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
                    return CornerRadius.Parse(s);
                }
                catch (FormatException)
                {
                    return new CornerRadius(0);
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
        public IBrush WindowTitleBackground { get => GetValue(WindowTitleBackgroundProperty); set => SetValue(WindowTitleBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="WindowTitleBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> WindowTitleBackgroundProperty
            = AvaloniaProperty.Register<SsuiAppTheme, IBrush>(nameof(WindowTitleBackground), Colors.LightGray.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the foreground elements of the title bar area of a SSUI-themed window (e.g. the window title text).
        /// </summary>
        public IBrush WindowTitleForeground { get => GetValue(WindowTitleForegroundProperty); set => SetValue(WindowTitleForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="WindowTitleForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> WindowTitleForegroundProperty
            = AvaloniaProperty.Register<SsuiAppTheme, IBrush>(nameof(WindowTitleForeground), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of the title bar area of a SSUI-themed window while it is inactive (not focused).
        /// </summary>
        public IBrush WindowInactiveBackground { get => GetValue(WindowInactiveBackgroundProperty); set => SetValue(WindowInactiveBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="WindowInactiveBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> WindowInactiveBackgroundProperty
            = AvaloniaProperty.Register<SsuiAppTheme, IBrush>(nameof(WindowInactiveBackground), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the foreground elements of the title bar area of a SSUI-themed window while it is inactive (not focused).
        /// </summary>
        public IBrush WindowInactiveForeground { get => GetValue(WindowInactiveForegroundProperty); set => SetValue(WindowInactiveForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="WindowInactiveForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> WindowInactiveForegroundProperty
            = AvaloniaProperty.Register<SsuiAppTheme, IBrush>(nameof(WindowInactiveForeground), Color.Parse("505050").ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of the window caption controls in the top corner of a SSUI-themed window.
        /// </summary>
        public IBrush WindowCaptionsBackground { get => GetValue(WindowCaptionsBackgroundProperty); set => SetValue(WindowCaptionsBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="WindowCaptionsBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> WindowCaptionsBackgroundProperty
            = AvaloniaProperty.Register<SsuiAppTheme, IBrush>(nameof(WindowCaptionsBackground), Colors.LightGray.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the symbols of the window caption controls in the top corner of a SSUI-themed window.
        /// </summary>
        public IBrush WindowCaptionsForeground { get => GetValue(WindowCaptionsForegroundProperty); set => SetValue(WindowCaptionsForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="WindowCaptionsForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> WindowCaptionsForegroundProperty
            = AvaloniaProperty.Register<SsuiAppTheme, IBrush>(nameof(WindowCaptionsForeground), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of a window caption control while it is highlighted (e.g. mouse over, keyboard focus).
        /// </summary>
        public IBrush WindowCaptionsHighlight { get => GetValue(WindowCaptionsHighlightProperty); set => SetValue(WindowCaptionsHighlightProperty, value); }

        /// <summary>The backing styled property for <see cref="WindowCaptionsHighlight"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> WindowCaptionsHighlightProperty
            = AvaloniaProperty.Register<SsuiAppTheme, IBrush>(nameof(WindowCaptionsHighlight), Colors.Gray.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the symbols of a window caption control while it is highlighted (e.g. mouse over, keyboard focus).
        /// </summary>
        public IBrush WindowCaptionsHighlightForeground { get => GetValue(WindowCaptionsHighlightForegroundProperty); set => SetValue(WindowCaptionsHighlightForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="WindowCaptionsHighlightForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> WindowCaptionsHighlightForegroundProperty
            = AvaloniaProperty.Register<SsuiAppTheme, IBrush>(nameof(WindowCaptionsHighlightForeground), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of a window caption control while it is being clicked/pressed.
        /// </summary>
        public IBrush WindowCaptionsClickBrush { get => GetValue(WindowCaptionsClickBrushProperty); set => SetValue(WindowCaptionsClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="WindowCaptionsClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> WindowCaptionsClickBrushProperty
            = AvaloniaProperty.Register<SsuiAppTheme, IBrush>(nameof(WindowCaptionsClickBrush), Colors.Gray.ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of the content area of a SSUI-themed window.
        /// </summary>
        public IBrush WindowBackground { get => GetValue(WindowBackgroundProperty); set => SetValue(WindowBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="WindowBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> WindowBackgroundProperty
            = AvaloniaProperty.Register<SsuiAppTheme, IBrush>(nameof(WindowBackground), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the theme to use for controls that are set to use accent brushes.
        /// </summary>
        public SsuiTheme? AccentTheme { get => GetValue(AccentThemeProperty); set => SetValue(AccentThemeProperty, value); }

        /// <summary>The backing styled property for <see cref="AccentTheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<SsuiTheme?> AccentThemeProperty
            = AvaloniaProperty.Register<SsuiAppTheme, SsuiTheme?>(nameof(AccentTheme), new SsuiTheme());

        /// <summary>
        /// Get or set the theme to use for child items within certain SSUI container controls.
        /// </summary>
        public SsuiTheme? SubitemTheme { get => GetValue(SubitemThemeProperty); set => SetValue(SubitemThemeProperty, value); }

        /// <summary>The backing styled property for <see cref="SubitemTheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<SsuiTheme?> SubitemThemeProperty
            = AvaloniaProperty.Register<SsuiAppTheme, SsuiTheme?>(nameof(SubitemTheme), new SsuiTheme());

        /// <summary>
        /// Get or set if the <see cref="SubitemTheme"/> should be used for menu controls.
        /// </summary>
        public bool UseSubitemThemeWithMenus { get => GetValue(UseSubitemThemeWithMenusProperty); set => SetValue(UseSubitemThemeWithMenusProperty, value); }

        /// <summary>The backing styled property for <see cref="UseSubitemThemeWithMenus"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseSubitemThemeWithMenusProperty
            = AvaloniaProperty.Register<SsuiAppTheme, bool>(nameof(UseSubitemThemeWithMenus), false);

        /// <summary>
        /// Get or set if the <see cref="SubitemTheme"/> should be used for panel controls.
        /// </summary>
        public bool UseSubitemThemeWithPanels { get => GetValue(UseSubitemThemeWithPanelsProperty); set => SetValue(UseSubitemThemeWithPanelsProperty, value); }

        /// <summary>The backing styled property for <see cref="UseSubitemThemeWithPanels"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseSubitemThemeWithPanelsProperty
            = AvaloniaProperty.Register<SsuiAppTheme, bool>(nameof(UseSubitemThemeWithPanels), false);

        /// <summary>
        /// Get or set if the <see cref="SubitemTheme"/> should be used for ribbon controls.
        /// </summary>
        public bool UseSubitemThemeWithRibbons { get => GetValue(UseSubitemThemeWithRibbonsProperty); set => SetValue(UseSubitemThemeWithRibbonsProperty, value); }

        /// <summary>The backing styled property for <see cref="UseSubitemThemeWithRibbons"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseSubitemThemeWithRibbonsProperty
            = AvaloniaProperty.Register<SsuiAppTheme, bool>(nameof(UseSubitemThemeWithRibbons), true);

        /// <summary>
        /// Get or set if menu controls should use the <see cref="WindowTitleBackground"/> brush for their background.
        /// </summary>
        public bool AllowTitleBarBrushWithMenus { get => GetValue(AllowTitleBarBrushWithMenusProperty); set => SetValue(AllowTitleBarBrushWithMenusProperty, value); }

        /// <summary>The backing styled property for <see cref="AllowTitleBarBrushWithMenus"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> AllowTitleBarBrushWithMenusProperty
            = AvaloniaProperty.Register<SsuiAppTheme, bool>(nameof(AllowTitleBarBrushWithMenus), true);

        #endregion

        /// <summary>
        /// Creates a copy of this SsuiAppTheme with all property values copied to the new instance.
        /// </summary>
        /// <returns>A new SsuiAppTheme instance with the same property values as this instance.</returns>
        public override SsuiTheme Copy()
        {
            var copy = new SsuiAppTheme
            {
                // Base SsuiTheme properties
                ButtonBackground = ButtonBackground,
                ControlBackground = ControlBackground,
                PanelBackground = PanelBackground,
                BaseBackground = BaseBackground,
                ControlSatBrush = ControlSatBrush,
                BorderBrush = BorderBrush,
                Foreground = Foreground,
                DisabledBackground = DisabledBackground,
                DisabledBorderBrush = DisabledBorderBrush,
                DisabledForeground = DisabledForeground,
                HighlightBrush = HighlightBrush,
                HighlightBorderBrush = HighlightBorderBrush,
                HighlightForeground = HighlightForeground,
                ClickBrush = ClickBrush,
                SelectedBackgroundBrush = SelectedBackgroundBrush,
                SelectedBorderBrush = SelectedBorderBrush,
                SelectedForeground = SelectedForeground,
                LightBorderBrush = LightBorderBrush,
                ControlPopBrush = ControlPopBrush,
                TabBackground = TabBackground,
                TabSelectedBrush = TabSelectedBrush,
                TabHighlightBrush = TabHighlightBrush,
                TabHighlightBorderBrush = TabHighlightBorderBrush,
                CheckBrush = CheckBrush,
                CheckHighlightBrush = CheckHighlightBrush,
                CheckBackgroundHighlightBrush = CheckBackgroundHighlightBrush,
                CommandBarBackground = CommandBarBackground,
                CommandBarBorderBrush = CommandBarBorderBrush,
                CornerRadius = CornerRadius,
                IconVariation = IconVariation,
                // SsuiAppTheme-specific properties
                WindowTitleBackground = WindowTitleBackground,
                WindowTitleForeground = WindowTitleForeground,
                WindowInactiveBackground = WindowInactiveBackground,
                WindowInactiveForeground = WindowInactiveForeground,
                WindowCaptionsBackground = WindowCaptionsBackground,
                WindowCaptionsForeground = WindowCaptionsForeground,
                WindowCaptionsHighlight = WindowCaptionsHighlight,
                WindowCaptionsHighlightForeground = WindowCaptionsHighlightForeground,
                WindowCaptionsClickBrush = WindowCaptionsClickBrush,
                WindowBackground = WindowBackground,
                AccentTheme = AccentTheme?.Copy(),
                SubitemTheme = SubitemTheme?.Copy(),
                UseSubitemThemeWithMenus = UseSubitemThemeWithMenus,
                UseSubitemThemeWithPanels = UseSubitemThemeWithPanels,
                UseSubitemThemeWithRibbons = UseSubitemThemeWithRibbons,
                AllowTitleBarBrushWithMenus = AllowTitleBarBrushWithMenus
            };
            return copy;
        }
    }
}
