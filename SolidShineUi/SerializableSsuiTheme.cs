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

        /// <summary>The value for the <see cref="SsuiTheme.ButtonBackground"/> property.</summary>
        public string ButtonBackground { get; set; } = sGainsboro;

        /// <summary>The value for the <see cref="SsuiTheme.ControlBackground"/> property.</summary>
        public string ControlBackground { get; set; } = sWhite;

        /// <summary>The value for the <see cref="SsuiTheme.PanelBackground"/> property.</summary>
        public string PanelBackground { get; set; } = sWhite;

        /// <summary>The value for the <see cref="SsuiTheme.BaseBackground"/> property.</summary>
        public string BaseBackground { get; set; } = sWhite;

        /// <summary>The value for the <see cref="SsuiTheme.ControlSatBrush"/> property.</summary>
        public string ControlSatBrush { get; set; } = sGray;

        /// <summary>The value for the <see cref="SsuiTheme.BorderBrush"/> property.</summary>
        public string BorderBrush { get; set; } = sBlack;

        /// <summary>The value for the <see cref="SsuiTheme.Foreground"/> property.</summary>
        public string Foreground { get; set; } = sBlack;

        /// <summary>The value for the <see cref="SsuiTheme.DisabledBackground"/> property.</summary>
        public string DisabledBackground { get; set; } = sDisabledBkgd;

        /// <summary>The value for the <see cref="SsuiTheme.DisabledBorderBrush"/> property.</summary>
        public string DisabledBorderBrush { get; set; } = sDisabledBorder;

        /// <summary>The value for the <see cref="SsuiTheme.DisabledForeground"/> property.</summary>
        public string DisabledForeground { get; set; } = sDisabledBorder;

        /// <summary>The value for the <see cref="SsuiTheme.HighlightBrush"/> property.</summary>
        public string HighlightBrush { get; set; } = sGray;

        /// <summary>The value for the <see cref="SsuiTheme.HighlightBorderBrush"/> property.</summary>
        public string HighlightBorderBrush { get; set; } = sBlack;

        /// <summary>The value for the <see cref="SsuiTheme.ClickBrush"/> property.</summary>
        public string ClickBrush { get; set; } = sLightGray;

        /// <summary>The value for the <see cref="SsuiTheme.HighlightForeground"/> property.</summary>
        public string HighlightForeground { get; set; } = sDarkerGray;

        /// <summary>The value for the <see cref="SsuiTheme.TabBackground"/> property.</summary>
        public string TabBackground { get; set; } = sGainsboro;

        /// <summary>The value for the <see cref="SsuiTheme.TabSelectedBrush"/> property.</summary>
        public string TabSelectedBrush { get; set; } = sWhite;

        /// <summary>The value for the <see cref="SsuiTheme.TabHighlightBrush"/> property.</summary>
        public string TabHighlightBrush { get; set; } = sGainsboro;

        /// <summary>The value for the <see cref="SsuiTheme.TabHighlightBorderBrush"/> property.</summary>
        public string TabHighlightBorderBrush { get; set; } = sBlack;

        /// <summary>The value for the <see cref="SsuiTheme.SelectedBackgroundBrush"/> property.</summary>
        public string SelectedBackgroundBrush { get; set; } = sGray;

        /// <summary>The value for the <see cref="SsuiTheme.SelectedBorderBrush"/> property.</summary>
        public string SelectedBorderBrush { get; set; } = sBlack;

        /// <summary>The value for the <see cref="SsuiTheme.LightBorderBrush"/> property.</summary>
        public string LightBorderBrush { get; set; } = sDarkGray;

        /// <summary>The value for the <see cref="SsuiTheme.CheckBrush"/> property.</summary>
        public string CheckBrush { get; set; } = sBlack;

        /// <summary>The value for the <see cref="SsuiTheme.CheckHighlightBrush"/> property.</summary>
        public string CheckHighlightBrush { get; set; } = sDarkerGray;

        /// <summary>The value for the <see cref="SsuiTheme.CheckBackgroundHighlightBrush"/> property.</summary>
        public string CheckBackgroundHighlightBrush { get; set; } = sWhiteLightHighlight;

        /// <summary>The value for the <see cref="SsuiTheme.ControlPopBrush"/> property.</summary>
        public string ControlPopBrush { get; set; } = sDarkGray;

        /// <summary>The value for the <see cref="SsuiTheme.CommandBarBackground"/> property.</summary>
        public string CommandBarBackground { get; set; } = sWhite;

        /// <summary>The value for the <see cref="SsuiTheme.CommandBarBorderBrush"/> property.</summary>
        public string CommandBarBorderBrush { get; set; } = sDarkGray;

        // non-Brush values

        /// <summary>The value for the <see cref="SsuiTheme.CornerRadius"/> property.</summary>
        public string CornerRadius { get; set; } = "0";

        // IconVariation stored as enum name

        /// <summary>The value for the <see cref="SsuiTheme.IconVariation"/> property.</summary>
        public string IconVariation { get; set; } = nameof(Utils.IconVariation.Color);

        // SsuiAppTheme-specific properties

        /// <summary>The value for the <see cref="SsuiAppTheme.WindowTitleBackground"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be an empty string.</summary>
        public string WindowTitleBackground { get; set; } = sLightGray;

        /// <summary>The value for the <see cref="SsuiAppTheme.WindowTitleForeground"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be an empty string.</summary>
        public string WindowTitleForeground { get; set; } = sBlack;

        /// <summary>The value for the <see cref="SsuiAppTheme.WindowInactiveBackground"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be an empty string.</summary>
        public string WindowInactiveBackground { get; set; } = sWhite;

        /// <summary>The value for the <see cref="SsuiAppTheme.WindowInactiveForeground"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be an empty string.</summary>
        public string WindowInactiveForeground { get; set; } = sWindowInactiveFore;

        /// <summary>The value for the <see cref="SsuiAppTheme.WindowCaptionsBackground"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be an empty string.</summary>
        public string WindowCaptionsBackground { get; set; } = sLightGray;

        /// <summary>The value for the <see cref="SsuiAppTheme.WindowCaptionsForeground"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be an empty string.</summary>
        public string WindowCaptionsForeground { get; set; } = sBlack;

        /// <summary>The value for the <see cref="SsuiAppTheme.WindowCaptionsHighlight"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be an empty string.</summary>
        public string WindowCaptionsHighlight { get; set; } = sGray;

        /// <summary>The value for the <see cref="SsuiAppTheme.WindowCaptionsHighlightForeground"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be an empty string.</summary>
        public string WindowCaptionsHighlightForeground { get; set; } = sBlack;

        /// <summary>The value for the <see cref="SsuiAppTheme.WindowCaptionsClickBrush"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be an empty string.</summary>
        public string WindowCaptionsClickBrush { get; set; } = sGray;

        /// <summary>The value for the <see cref="SsuiAppTheme.WindowBackground"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be an empty string.</summary>
        public string WindowBackground { get; set; } = sWhite;


#if NETCOREAPP
        // subthemes
        /// <summary>The value for the <see cref="SsuiAppTheme.AccentTheme"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be null.</summary>
        public SerializableSsuiTheme? AccentTheme { get; set; } = null;
        
        /// <summary>The value for the <see cref="SsuiAppTheme.SubitemTheme"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be null.</summary>
        public SerializableSsuiTheme? SubitemTheme { get; set; } = null;
#else
        // subthemes
        /// <summary>The value for the <see cref="SsuiAppTheme.AccentTheme"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be null.</summary>
        public SerializableSsuiTheme AccentTheme { get; set; } = null;

        /// <summary>The value for the <see cref="SsuiAppTheme.SubitemTheme"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be null.</summary>
        public SerializableSsuiTheme SubitemTheme { get; set; } = null;
#endif

        /// <summary>The value for the <see cref="SsuiAppTheme.UseSubitemThemeWithMenus"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be false.</summary>
        public bool UseSubitemThemeWithMenus { get; set; } = false;

        /// <summary>The value for the <see cref="SsuiAppTheme.UseSubitemThemeWithPanels"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be false.</summary>
        public bool UseSubitemThemeWithPanels { get; set; } = false;

        /// <summary>The value for the <see cref="SsuiAppTheme.UseSubitemThemeWithRibbons"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be false.</summary>
        public bool UseSubitemThemeWithRibbons { get; set; } = true;

        /// <summary>The value for the <see cref="SsuiAppTheme.AllowTitleBarBrushWithMenus"/> property.
        /// <para/>If this was serialized from a <see cref="SsuiTheme"/>, this will be false.</summary>
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
            else
            {
                s.WindowTitleBackground = "";
                s.WindowTitleForeground = "";
                s.WindowInactiveBackground = "";
                s.WindowInactiveForeground = "";
                s.WindowCaptionsBackground = "";
                s.WindowCaptionsForeground = "";
                s.WindowCaptionsHighlight = "";
                s.WindowCaptionsHighlightForeground = "";
                s.WindowCaptionsClickBrush = "";
                s.WindowBackground = "";

                s.AccentTheme = null;
                s.SubitemTheme = null;

                s.UseSubitemThemeWithMenus = false;
                s.UseSubitemThemeWithPanels = false;
                s.UseSubitemThemeWithRibbons = false;
                s.AllowTitleBarBrushWithMenus = false;
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
                ButtonBackground = ParseBrush(ButtonBackground, freeze),
                ControlBackground = ParseBrush(ControlBackground, freeze),
                PanelBackground = ParseBrush(PanelBackground, freeze),
                BaseBackground = ParseBrush(BaseBackground, freeze),
                ControlSatBrush = ParseBrush(ControlSatBrush, freeze),
                BorderBrush = ParseBrush(BorderBrush, freeze),
                Foreground = ParseBrush(Foreground, freeze),
                DisabledBackground = ParseBrush(DisabledBackground, freeze),
                DisabledBorderBrush = ParseBrush(DisabledBorderBrush, freeze),
                DisabledForeground = ParseBrush(DisabledForeground, freeze),
                HighlightBrush = ParseBrush(HighlightBrush, freeze),
                HighlightBorderBrush = ParseBrush(HighlightBorderBrush, freeze),
                ClickBrush = ParseBrush(ClickBrush, freeze),
                HighlightForeground = ParseBrush(HighlightForeground, freeze),
                TabBackground = ParseBrush(TabBackground, freeze),
                TabSelectedBrush = ParseBrush(TabSelectedBrush, freeze),
                TabHighlightBrush = ParseBrush(TabHighlightBrush, freeze),
                TabHighlightBorderBrush = ParseBrush(TabHighlightBorderBrush, freeze),
                SelectedBackgroundBrush = ParseBrush(SelectedBackgroundBrush, freeze),
                SelectedBorderBrush = ParseBrush(SelectedBorderBrush, freeze),
                LightBorderBrush = ParseBrush(LightBorderBrush, freeze),
                CheckBrush = ParseBrush(CheckBrush, freeze),
                CheckHighlightBrush = ParseBrush(CheckHighlightBrush, freeze),
                CheckBackgroundHighlightBrush = ParseBrush(CheckBackgroundHighlightBrush, freeze),
                ControlPopBrush = ParseBrush(ControlPopBrush, freeze),
                CommandBarBackground = ParseBrush(CommandBarBackground, freeze),
                CommandBarBorderBrush = ParseBrush(CommandBarBorderBrush, freeze),
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
                ButtonBackground = ParseBrush(ButtonBackground, freeze),
                ControlBackground = ParseBrush(ControlBackground, freeze),
                PanelBackground = ParseBrush(PanelBackground, freeze),
                BaseBackground = ParseBrush(BaseBackground, freeze),
                ControlSatBrush = ParseBrush(ControlSatBrush, freeze),
                BorderBrush = ParseBrush(BorderBrush, freeze),
                Foreground = ParseBrush(Foreground, freeze),
                DisabledBackground = ParseBrush(DisabledBackground, freeze),
                DisabledBorderBrush = ParseBrush(DisabledBorderBrush, freeze),
                DisabledForeground = ParseBrush(DisabledForeground, freeze),
                HighlightBrush = ParseBrush(HighlightBrush, freeze),
                HighlightBorderBrush = ParseBrush(HighlightBorderBrush, freeze),
                ClickBrush = ParseBrush(ClickBrush, freeze),
                HighlightForeground = ParseBrush(HighlightForeground, freeze),
                TabBackground = ParseBrush(TabBackground, freeze),
                TabSelectedBrush = ParseBrush(TabSelectedBrush, freeze),
                TabHighlightBrush = ParseBrush(TabHighlightBrush, freeze),
                TabHighlightBorderBrush = ParseBrush(TabHighlightBorderBrush, freeze),
                SelectedBackgroundBrush = ParseBrush(SelectedBackgroundBrush, freeze),
                SelectedBorderBrush = ParseBrush(SelectedBorderBrush, freeze),
                LightBorderBrush = ParseBrush(LightBorderBrush, freeze),
                CheckBrush = ParseBrush(CheckBrush, freeze),
                CheckHighlightBrush = ParseBrush(CheckHighlightBrush, freeze),
                CheckBackgroundHighlightBrush = ParseBrush(CheckBackgroundHighlightBrush, freeze),
                ControlPopBrush = ParseBrush(ControlPopBrush, freeze),
                CommandBarBackground = ParseBrush(CommandBarBackground, freeze),
                CommandBarBorderBrush = ParseBrush(CommandBarBorderBrush, freeze),
                CornerRadius = ParseCornerRadius(CornerRadius),

                WindowTitleBackground = ParseBrush(WindowTitleBackground, freeze),
                WindowTitleForeground = ParseBrush(WindowTitleForeground, freeze),
                WindowInactiveBackground = ParseBrush(WindowInactiveBackground, freeze),
                WindowInactiveForeground = ParseBrush(WindowInactiveForeground, freeze),
                WindowCaptionsBackground = ParseBrush(WindowCaptionsBackground, freeze),
                WindowCaptionsForeground = ParseBrush(WindowCaptionsForeground, freeze),
                WindowCaptionsHighlight = ParseBrush(WindowCaptionsHighlight, freeze),
                WindowCaptionsHighlightForeground = ParseBrush(WindowCaptionsHighlightForeground, freeze),
                WindowCaptionsClickBrush = ParseBrush(WindowCaptionsClickBrush, freeze),
                WindowBackground = ParseBrush(WindowBackground, freeze),

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

        static Brush ParseBrush(string s, bool freeze)
        {
            // try to use the BrushSerializer if I can, but if the string is null, empty, or can't be parsed, we'll return a Transparent solid color brush
            // a Transparent brush is probably not what's really desired, but in this situation where data may have been corrupted, not sure what else to do

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
                    return new CornerRadius(0); // fallback value
                }
            }
        }

        #endregion
    }
}
