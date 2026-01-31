using SolidShineUi.Utils;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A basic class that can be converted to and from a <see cref="SsuiTheme"/> or <see cref="SsuiAppTheme"/>, 
    /// built specifically to use in serialization scenarios, such as storing in a settings file.
    /// Each brush property is stored as the compact string format produced/consumed by <see cref="BrushSerializer"/>.
    /// </summary>
#if NET8_0_OR_GREATER
    public class SerializableSsuiTheme
#else
    [Serializable]
    public class SerializableSsuiTheme
#endif
    {
        // by default, this uses the default values of SsuiTheme's actual properties
        // (this is just the BrushSerializer format for the solid color brushes)
        static readonly string sGainsboro = "s;" + Colors.Gainsboro.GetHexStringWithAlpha();
        static readonly string sWhite = "s;" + Colors.White.GetHexStringWithAlpha();
        static readonly string sBlack = "s;" + Colors.Black.GetHexStringWithAlpha();
        static readonly string sGray = "s;" + Colors.Gray.GetHexStringWithAlpha();
        static readonly string sDarkGray = "s;" + Colors.DarkGray.GetHexStringWithAlpha();
        static readonly string sLightGray = "s;" + Colors.LightGray.GetHexStringWithAlpha();
        static readonly string sDarkerGray = "s;" + ColorsHelper.DarkerGray.GetHexStringWithAlpha();
        static readonly string sDisabledBkgd = "s;" + "FFF5F5F5";
        static readonly string sDisabledBorder = "s;" + "FFAAAAAF";
        static readonly string sWhiteLightHighlight = "s;" + ColorsHelper.WhiteLightHighlight.GetHexStringWithAlpha();
        static readonly string sWindowInactiveFore = "s;" + "FF505050";

        #region Properties

        public string ButtonBackground { get; set; } = sGainsboro;

        public string ControlBackground { get; set; } = sWhite;

        public string PanelBackground { get; set; } = sWhite;

        public string BaseBackground { get; set; } = sWhite;

        public string ControlSatBrush { get; set; } = sGray;

        public string BorderBrush { get; set; } = sBlack;

        public string Foreground { get; set; } = sBlack;

        public string DisabledBackground { get; set; } = sDisabledBkgd;

        public string DisabledBorderBrush { get; set; } = sDisabledBorder;

        public string DisabledForeground { get; set; } = sDisabledBorder;

        public string HighlightBrush { get; set; } = sGray;

        public string HighlightBorderBrush { get; set; } = sBlack;

        public string ClickBrush { get; set; } = sLightGray;

        public string HighlightForeground { get; set; } = sDarkerGray;

        public string TabBackground { get; set; } = sGainsboro;

        public string TabSelectedBrush { get; set; } = sWhite;

        public string TabHighlightBrush { get; set; } = sGainsboro;

        public string TabHighlightBorderBrush { get; set; } = sBlack;

        public string SelectedBackgroundBrush { get; set; } = sGray;

        public string SelectedBorderBrush { get; set; } = sBlack;

        public string LightBorderBrush { get; set; } = sDarkGray;

        public string CheckBrush { get; set; } = sBlack;

        public string CheckHighlightBrush { get; set; } = sDarkerGray;

        public string CheckBackgroundHighlightBrush { get; set; } = sWhiteLightHighlight;

        public string ControlPopBrush { get; set; } = sDarkGray;

        public string CommandBarBackground { get; set; } = sWhite;

        public string CommandBarBorderBrush { get; set; } = sDarkGray;

        // non-Brush values
        public string CornerRadius { get; set; } = "0";

        // IconVariation stored as enum name
        public string IconVariation { get; set; } = nameof(Utils.IconVariation.Color);

        // SsuiAppTheme-specific properties
        public string WindowTitleBackground { get; set; } = sLightGray;

        public string WindowTitleForeground { get; set; } = sBlack;

        public string WindowInactiveBackground { get; set; } = sWhite;

        public string WindowInactiveForeground { get; set; } = sWindowInactiveFore;

        public string WindowCaptionsBackground { get; set; } = sLightGray;

        public string WindowCaptionsForeground { get; set; } = sBlack;

        public string WindowCaptionsHighlight { get; set; } = sGray;

        public string WindowCaptionsHighlightForeground { get; set; } = sBlack;

        public string WindowCaptionsClickBrush { get; set; } = sGray;

        public string WindowBackground { get; set; } = sWhite;


#if NETCOREAPP
        // subthemes
        public SerializableSsuiTheme? AccentTheme { get; set; } = null;

        public SerializableSsuiTheme? SubitemTheme { get; set; } = null;
#else
        // subthemes
        public SerializableSsuiTheme AccentTheme { get; set; } = null;

        public SerializableSsuiTheme SubitemTheme { get; set; } = null;
#endif

        public bool UseSubitemThemeWithMenus { get; set; } = false;
        public bool UseSubitemThemeWithPanels { get; set; } = false;
        public bool UseSubitemThemeWithRibbons { get; set; } = true;

        public bool AllowTitleBarBrushWithMenus { get; set; } = true;

        #endregion

        #region Conversion Methods

        /// <summary>
        /// Create a new SerializableSsuiTheme from an <see cref="SsuiTheme"/> (or <see cref="SsuiAppTheme"/>).
        /// <para/>
        /// This returned object is built to be used for serialization purposes, such as storing in settings files.
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="theme"/> is null</exception>
        public static SerializableSsuiTheme FromSsuiTheme(SsuiTheme theme)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(theme, nameof(theme));
#else
            if (theme == null)
            {
                throw new ArgumentNullException(nameof(theme));
            }
#endif

            var s = new SerializableSsuiTheme
            {
                ButtonBackground = BrushSerializer.Serialize(theme.ButtonBackground),
                ControlBackground = BrushSerializer.Serialize(theme.ControlBackground),
                PanelBackground = BrushSerializer.Serialize(theme.PanelBackground),
                BaseBackground = BrushSerializer.Serialize(theme.BaseBackground),
                ControlSatBrush = BrushSerializer.Serialize(theme.ControlSatBrush),
                BorderBrush = BrushSerializer.Serialize(theme.BorderBrush),
                Foreground = BrushSerializer.Serialize(theme.Foreground),
                DisabledBackground = BrushSerializer.Serialize(theme.DisabledBackground),
                DisabledBorderBrush = BrushSerializer.Serialize(theme.DisabledBorderBrush),
                DisabledForeground = BrushSerializer.Serialize(theme.DisabledForeground),
                HighlightBrush = BrushSerializer.Serialize(theme.HighlightBrush),
                HighlightBorderBrush = BrushSerializer.Serialize(theme.HighlightBorderBrush),
                ClickBrush = BrushSerializer.Serialize(theme.ClickBrush),
                HighlightForeground = BrushSerializer.Serialize(theme.HighlightForeground),
                TabBackground = BrushSerializer.Serialize(theme.TabBackground),
                TabSelectedBrush = BrushSerializer.Serialize(theme.TabSelectedBrush),
                TabHighlightBrush = BrushSerializer.Serialize(theme.TabHighlightBrush),
                TabHighlightBorderBrush = BrushSerializer.Serialize(theme.TabHighlightBorderBrush),
                SelectedBackgroundBrush = BrushSerializer.Serialize(theme.SelectedBackgroundBrush),
                SelectedBorderBrush = BrushSerializer.Serialize(theme.SelectedBorderBrush),
                LightBorderBrush = BrushSerializer.Serialize(theme.LightBorderBrush),
                CheckBrush = BrushSerializer.Serialize(theme.CheckBrush),
                CheckHighlightBrush = BrushSerializer.Serialize(theme.CheckHighlightBrush),
                CheckBackgroundHighlightBrush = BrushSerializer.Serialize(theme.CheckBackgroundHighlightBrush),
                ControlPopBrush = BrushSerializer.Serialize(theme.ControlPopBrush),
                CommandBarBackground = BrushSerializer.Serialize(theme.CommandBarBackground),
                CommandBarBorderBrush = BrushSerializer.Serialize(theme.CommandBarBorderBrush),
                CornerRadius = CornerRadiusToString(theme.CornerRadius),
                IconVariation = theme.IconVariation.ToString("g")
            };

            if (theme is SsuiAppTheme ssat)
            {
                s.WindowTitleBackground = BrushSerializer.Serialize(ssat.WindowTitleBackground);
                s.WindowTitleForeground = BrushSerializer.Serialize(ssat.WindowTitleForeground);
                s.WindowInactiveBackground = BrushSerializer.Serialize(ssat.WindowInactiveBackground);
                s.WindowInactiveForeground = BrushSerializer.Serialize(ssat.WindowInactiveForeground);
                s.WindowCaptionsBackground = BrushSerializer.Serialize(ssat.WindowCaptionsBackground);
                s.WindowCaptionsForeground = BrushSerializer.Serialize(ssat.WindowCaptionsForeground);
                s.WindowCaptionsHighlight = BrushSerializer.Serialize(ssat.WindowCaptionsHighlight);
                s.WindowCaptionsHighlightForeground = BrushSerializer.Serialize(ssat.WindowCaptionsHighlightForeground);
                s.WindowCaptionsClickBrush = BrushSerializer.Serialize(ssat.WindowCaptionsClickBrush);
                s.WindowBackground = BrushSerializer.Serialize(ssat.WindowBackground);

                if (ssat.AccentTheme != null)
                {
                    s.AccentTheme = FromSsuiTheme(ssat.AccentTheme);
                }
                if (ssat.SubitemTheme != null)
                {
                    s.SubitemTheme = FromSsuiTheme(ssat.SubitemTheme);
                }

                s.UseSubitemThemeWithMenus = ssat.UseSubitemThemeWithMenus;
                s.UseSubitemThemeWithPanels = ssat.UseSubitemThemeWithPanels;
                s.UseSubitemThemeWithRibbons = ssat.UseSubitemThemeWithRibbons;
                s.AllowTitleBarBrushWithMenus = ssat.AllowTitleBarBrushWithMenus;
            }

            return s;
        }

        /// <summary>
        /// Create a <see cref="SsuiTheme"/> object from this SerializableSsuiTheme, by parsing all of the string values.
        /// <para/>
        /// If the object you serialized was a <see cref="SsuiAppTheme"/>, you should use <see cref="ToSsuiAppTheme()"/> instead to get that type back.
        /// </summary>
        /// <remarks>
        /// The returned object is not frozen by default out of convenience; you can use <see cref="ToSsuiTheme(bool)"/> to create an automatically frozen object instead.
        /// <para/>
        /// The <see cref="BrushSerializer"/> is used to parse all of the brush strings; if a string cannot be parsed, then a transparent SolidColorBrush is used instead.
        /// </remarks>
        public SsuiTheme ToSsuiTheme()
        {
            return ToSsuiTheme(false);
        }

        /// <summary>
        /// Create a <see cref="SsuiTheme"/> object from this SerializableSsuiTheme, by parsing all of the string values.
        /// </summary>
        /// <param name="freeze">If the returned SsuiTheme object should be frozen</param>
        /// <remarks>
        /// The <see cref="BrushSerializer"/> is used to parse all of the brush strings; if a string cannot be parsed, then a transparent SolidColorBrush is used instead.
        /// </remarks>
        public SsuiTheme ToSsuiTheme(bool freeze)
        {
            var theme = new SsuiTheme
            {
                ButtonBackground = StringToBrush(ButtonBackground, freeze),
                ControlBackground = StringToBrush(ControlBackground, freeze),
                PanelBackground = StringToBrush(PanelBackground, freeze),
                BaseBackground = StringToBrush(BaseBackground, freeze),
                ControlSatBrush = StringToBrush(ControlSatBrush, freeze),
                BorderBrush = StringToBrush(BorderBrush, freeze),
                Foreground = StringToBrush(Foreground, freeze),
                DisabledBackground = StringToBrush(DisabledBackground, freeze),
                DisabledBorderBrush = StringToBrush(DisabledBorderBrush, freeze),
                DisabledForeground = StringToBrush(DisabledForeground, freeze),
                HighlightBrush = StringToBrush(HighlightBrush, freeze),
                HighlightBorderBrush = StringToBrush(HighlightBorderBrush, freeze),
                ClickBrush = StringToBrush(ClickBrush, freeze),
                HighlightForeground = StringToBrush(HighlightForeground, freeze),
                TabBackground = StringToBrush(TabBackground, freeze),
                TabSelectedBrush = StringToBrush(TabSelectedBrush, freeze),
                TabHighlightBrush = StringToBrush(TabHighlightBrush, freeze),
                TabHighlightBorderBrush = StringToBrush(TabHighlightBorderBrush, freeze),
                SelectedBackgroundBrush = StringToBrush(SelectedBackgroundBrush, freeze),
                SelectedBorderBrush = StringToBrush(SelectedBorderBrush, freeze),
                LightBorderBrush = StringToBrush(LightBorderBrush, freeze),
                CheckBrush = StringToBrush(CheckBrush, freeze),
                CheckHighlightBrush = StringToBrush(CheckHighlightBrush, freeze),
                CheckBackgroundHighlightBrush = StringToBrush(CheckBackgroundHighlightBrush, freeze),
                ControlPopBrush = StringToBrush(ControlPopBrush, freeze),
                CommandBarBackground = StringToBrush(CommandBarBackground, freeze),
                CommandBarBorderBrush = StringToBrush(CommandBarBorderBrush, freeze),
                CornerRadius = ParseCornerRadius(CornerRadius)
            };

            if (!string.IsNullOrEmpty(IconVariation) && Enum.TryParse<IconVariation>(IconVariation, out var iv))
            {
                theme.IconVariation = iv;
            }

            if (theme.CanFreeze && freeze) theme.Freeze();

            return theme;
        }

        /// <summary>
        /// Create a <see cref="SsuiAppTheme"/> object from this SerializableSsuiTheme, by parsing all of the string values.
        /// </summary>
        /// <remarks>
        /// The returned object is not frozen by default out of convenience; you can use <see cref="ToSsuiAppTheme(bool)"/> to create an automatically frozen object instead.
        /// <para/>
        /// The <see cref="BrushSerializer"/> is used to parse all of the brush strings; if a string cannot be parsed, then a transparent SolidColorBrush is used instead.
        /// </remarks>
        public SsuiAppTheme ToSsuiAppTheme()
        {
            return ToSsuiAppTheme(false);
        }

        /// <summary>
        /// Create a <see cref="SsuiAppTheme"/> object from this SerializableSsuiTheme, by parsing all of the string values.
        /// </summary>
        /// <param name="freeze">If the returned SsuiAppTheme object should be frozen</param>
        /// <remarks>
        /// The <see cref="BrushSerializer"/> is used to parse all of the brush strings; if a string cannot be parsed, then a transparent SolidColorBrush is used instead.
        /// </remarks>
        public SsuiAppTheme ToSsuiAppTheme(bool freeze)
        {
            var ssat = new SsuiAppTheme()
            {
                ButtonBackground = StringToBrush(ButtonBackground, freeze),
                ControlBackground = StringToBrush(ControlBackground, freeze),
                PanelBackground = StringToBrush(PanelBackground, freeze),
                BaseBackground = StringToBrush(BaseBackground, freeze),
                ControlSatBrush = StringToBrush(ControlSatBrush, freeze),
                BorderBrush = StringToBrush(BorderBrush, freeze),
                Foreground = StringToBrush(Foreground, freeze),
                DisabledBackground = StringToBrush(DisabledBackground, freeze),
                DisabledBorderBrush = StringToBrush(DisabledBorderBrush, freeze),
                DisabledForeground = StringToBrush(DisabledForeground, freeze),
                HighlightBrush = StringToBrush(HighlightBrush, freeze),
                HighlightBorderBrush = StringToBrush(HighlightBorderBrush, freeze),
                ClickBrush = StringToBrush(ClickBrush, freeze),
                HighlightForeground = StringToBrush(HighlightForeground, freeze),
                TabBackground = StringToBrush(TabBackground, freeze),
                TabSelectedBrush = StringToBrush(TabSelectedBrush, freeze),
                TabHighlightBrush = StringToBrush(TabHighlightBrush, freeze),
                TabHighlightBorderBrush = StringToBrush(TabHighlightBorderBrush, freeze),
                SelectedBackgroundBrush = StringToBrush(SelectedBackgroundBrush, freeze),
                SelectedBorderBrush = StringToBrush(SelectedBorderBrush, freeze),
                LightBorderBrush = StringToBrush(LightBorderBrush, freeze),
                CheckBrush = StringToBrush(CheckBrush, freeze),
                CheckHighlightBrush = StringToBrush(CheckHighlightBrush, freeze),
                CheckBackgroundHighlightBrush = StringToBrush(CheckBackgroundHighlightBrush, freeze),
                ControlPopBrush = StringToBrush(ControlPopBrush, freeze),
                CommandBarBackground = StringToBrush(CommandBarBackground, freeze),
                CommandBarBorderBrush = StringToBrush(CommandBarBorderBrush, freeze),
                CornerRadius = ParseCornerRadius(CornerRadius),

                WindowTitleBackground = StringToBrush(WindowTitleBackground, freeze),
                WindowTitleForeground = StringToBrush(WindowTitleForeground, freeze),
                WindowInactiveBackground = StringToBrush(WindowInactiveBackground, freeze),
                WindowInactiveForeground = StringToBrush(WindowInactiveForeground, freeze),
                WindowCaptionsBackground = StringToBrush(WindowCaptionsBackground, freeze),
                WindowCaptionsForeground = StringToBrush(WindowCaptionsForeground, freeze),
                WindowCaptionsHighlight = StringToBrush(WindowCaptionsHighlight, freeze),
                WindowCaptionsHighlightForeground = StringToBrush(WindowCaptionsHighlightForeground, freeze),
                WindowCaptionsClickBrush = StringToBrush(WindowCaptionsClickBrush, freeze),
                WindowBackground = StringToBrush(WindowBackground, freeze),

                UseSubitemThemeWithMenus = UseSubitemThemeWithMenus,
                UseSubitemThemeWithPanels = UseSubitemThemeWithPanels,
                UseSubitemThemeWithRibbons = UseSubitemThemeWithRibbons,
                AllowTitleBarBrushWithMenus = AllowTitleBarBrushWithMenus
            };

            if (!string.IsNullOrEmpty(IconVariation) && Enum.TryParse<IconVariation>(IconVariation, out var iv))
            {
                ssat.IconVariation = iv;
            }

            if (AccentTheme != null)
            {
                ssat.AccentTheme = AccentTheme.ToSsuiTheme();
            }
            if (SubitemTheme != null)
            {
                ssat.SubitemTheme = SubitemTheme.ToSsuiTheme();
            }

            if (ssat.CanFreeze && freeze) ssat.Freeze();

            return ssat;
        }

        #endregion

        #region Helper / Parser methods

        // Small helpers

        static Brush StringToBrush(string s, bool freeze)
        {
            if (string.IsNullOrEmpty(s))
            {
                Brush b = Colors.Transparent.ToBrush();
                if (freeze) b.Freeze();
                return b;
            }

            try
            {
                return BrushSerializer.DeserializeBrush(s, freeze);
            }
            catch (FormatException)
            {
                Brush b = Colors.Transparent.ToBrush();
                if (freeze) b.Freeze();
                return b;
            }
        }

        static string CornerRadiusToString(CornerRadius cr)
        {
            // I could probably figure out some way to make this work with the CornerRadiusConverter, but meh

            // check if the corner radius value is the same all the way around - if so, we can just use the double value all alone
            if (cr.TopLeft == cr.TopRight && cr.TopLeft == cr.BottomRight && cr.TopLeft == cr.BottomLeft)
            {
                return cr.TopLeft.ToString(CultureInfo.InvariantCulture);
            }
            // else, we'll make a comma-separated list for all of the corners - this should be easily parseable with the CornerRadiusConverter
            return string.Join(",", cr.TopLeft.ToString(CultureInfo.InvariantCulture),
                                     cr.TopRight.ToString(CultureInfo.InvariantCulture),
                                     cr.BottomRight.ToString(CultureInfo.InvariantCulture),
                                     cr.BottomLeft.ToString(CultureInfo.InvariantCulture));
        }

        static CornerRadius ParseCornerRadius(string s)
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

        #endregion
    }
}
