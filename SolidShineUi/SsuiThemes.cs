using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A collection of premade <see cref="SsuiAppTheme"/> objects.
    /// </summary>
    public static class SsuiThemes
    {
        /// <summary>
        /// A SsuiAppTheme that uses the system's colors for the controls.
        /// </summary>
        /// <remarks>
        /// Note that this will have a somewhat old-school (think Windows XP) appearance on modern computers, 
        /// given that <see cref="SystemColors"/> only provides basic color brushes.
        /// </remarks>
        public static SsuiAppTheme SystemTheme
        {
            get => CreateSystemTheme(new CornerRadius(0));
        }

        /// <summary>
        /// A SsuiAppTheme that uses the system's colors for the controls, and adds a slight rounded corners to the edges of many controls.
        /// </summary>
        /// <remarks>
        /// Note that this will have a somewhat old-school (think Windows XP) appearance on modern computers, 
        /// given that <see cref="SystemColors"/> only provides basic color brushes.
        /// </remarks>
        public static SsuiAppTheme SystemThemeRoundedCorners
        {
            get => CreateSystemTheme(new CornerRadius(3));
        }

        static SsuiAppTheme CreateSystemTheme(CornerRadius cornerRadius)
        {
            SsuiAppTheme ssat = new SsuiAppTheme()
            {
                BaseBackground = SystemColors.WindowBrush,
                PanelBackground = SystemColors.ControlLightLightBrush,
                ControlBackground = SystemColors.ControlLightLightBrush,
                ButtonBackground = SystemColors.ControlBrush,
                BorderBrush = SystemColors.ControlDarkDarkBrush,
                LightBorderBrush = SystemColors.ControlDarkBrush,
                ControlPopBrush = SystemColors.HighlightBrush,
                ControlSatBrush = SystemColors.ControlLightBrush,
                CheckBrush = SystemColors.ControlTextBrush,
                ClickBrush = SystemColors.MenuHighlightBrush,
                SelectedBackgroundBrush = SystemColors.HighlightBrush,
                SelectedBorderBrush = SystemColors.ControlDarkBrush,
                HighlightBrush = SystemColors.MenuHighlightBrush,
                HighlightBorderBrush = SystemColors.ControlDarkBrush,
                HighlightForeground = SystemColors.HighlightTextBrush,
                DisabledBackground = SystemColors.InactiveCaptionBrush,
                DisabledForeground = SystemColors.GrayTextBrush,
                DisabledBorderBrush = SystemColors.GrayTextBrush,
                Foreground = SystemColors.ControlTextBrush,
                WindowCaptionsForeground = SystemColors.ActiveCaptionTextBrush,
                WindowBackground = SystemColors.WindowBrush,
                WindowCaptionsBackground = Color.FromArgb(1, 255, 255, 255).ToBrush(), // almost entirely transparent
                WindowTitleBackground = new LinearGradientBrush(SystemColors.ActiveCaptionColor, SystemColors.GradientActiveCaptionColor, 0.0d),
                WindowTitleForeground = SystemColors.ActiveCaptionTextBrush,
                WindowInactiveBackground = new LinearGradientBrush(SystemColors.InactiveCaptionColor, SystemColors.GradientInactiveCaptionColor, 0.0d),
                WindowInactiveForeground = SystemColors.InactiveCaptionTextBrush,
                WindowCaptionsHighlight = SystemColors.MenuHighlightBrush,
                WindowCaptionsHighlightForeground = SystemColors.HighlightTextBrush,
                WindowCaptionsClickBrush = SystemColors.MenuHighlightBrush,
                TabBackground = SystemColors.ControlBrush,
                TabHighlightBorderBrush = SystemColors.ControlDarkDarkBrush,
                TabHighlightBrush = SystemColors.ControlDarkBrush,
                TabSelectedBrush = SystemColors.ControlLightLightBrush,
                CommandBarBackground = SystemColors.ControlLightBrush,
                CommandBarBorderBrush = SystemColors.ControlDarkDarkBrush,
                UseSubitemThemeWithMenus = false,
                UseSubitemThemeWithPanels = false,
                UseSubitemThemeWithRibbons = false,
                CornerRadius = cornerRadius,
                IconVariation = Utils.IconVariation.Color
            };

            ssat.AccentTheme = ssat.Copy();
            ssat.SubitemTheme = ssat.Copy();

            return ssat;
        }

        /// <summary>
        /// A standard premade theme, with a lot of light gray/white colors. 
        /// <para/>
        /// Use <see cref="CreateLightTheme(Color)"/> to create a light theme with an accent color.
        /// </summary>
        public static SsuiAppTheme LightTheme
        {
            get => CreateLightTheme(ColorsHelper.CreateFromHex("A8A8A8"));
        }

        /// <summary>
        /// A standard premade theme, with a lot of dark gray/black colors. 
        /// <para/>
        /// Use <see cref="CreateDarkTheme(Color)"/> to create a dark theme with an accent color.
        /// </summary>
        public static SsuiAppTheme DarkTheme
        {
            get => CreateDarkTheme(ColorsHelper.CreateFromHex("C8C8C8"));
        }


        /// <summary>
        /// Create a premade light theme color scheme. Lighter gray colors are used, and a custom accent color can be provided to add some more color.
        /// </summary>
        /// <param name="accentColor">The accent color to use with the light theme, to add more color to certain elements.</param>
        public static SsuiAppTheme CreateLightTheme(Color accentColor)
        {
            return new SsuiAppTheme(accentColor)
            {
                WindowBackground = Color.FromRgb(246, 246, 246).ToBrush(),
                BaseBackground = Color.FromRgb(246, 246, 246).ToBrush(),
                PanelBackground = Color.FromRgb(255, 255, 255).ToBrush(),
                ControlBackground = Color.FromRgb(255, 255, 255).ToBrush(),
                ControlSatBrush = Color.FromRgb(200, 200, 200).ToBrush(),
                WindowCaptionsBackground = Color.FromRgb(200, 200, 200).ToBrush(),
                WindowTitleBackground = Color.FromRgb(200, 200, 200).ToBrush(),
                WindowInactiveBackground = Color.FromRgb(200, 200, 200).ToBrush(),
                WindowTitleForeground = Color.FromRgb(0, 0, 0).ToBrush(),
                ButtonBackground = Color.FromRgb(220, 220, 220).ToBrush(),
                TabBackground = Color.FromRgb(220, 220, 220).ToBrush(),
                CommandBarBackground = Color.FromRgb(220, 220, 220).ToBrush(),
                TabSelectedBrush = Color.FromRgb(255, 255, 255).ToBrush(),
                BorderBrush = Color.FromRgb(128, 128, 128).ToBrush(),
                CommandBarBorderBrush = Color.FromRgb(128, 128, 128).ToBrush(),
                Foreground = Color.FromRgb(0, 0, 0).ToBrush(),
                HighlightForeground = Color.FromRgb(0, 0, 0).ToBrush(),
                WindowInactiveForeground = Color.FromRgb(0, 0, 0).ToBrush(),
                WindowCaptionsForeground = Color.FromRgb(0, 0, 0).ToBrush(),
                WindowCaptionsHighlightForeground = Color.FromRgb(0, 0, 0).ToBrush()
            };
        }

        /// <summary>
        /// Create a premade dark theme color scheme. Darker gray colors are used, and a custom accent color can be provided to add some more color.
        /// </summary>
        /// <param name="accentColor">The accent color to use with the dark theme, to add more color to certain elements.</param>
        public static SsuiAppTheme CreateDarkTheme(Color accentColor)
        {
            ColorsHelper.ToHSV(accentColor, out double h, out double s, out double v);
            double vc3 = -0.45;
            double vc4 = -0.2;
            double vc5 = -0.32;
            Color dark = AddValue(h, s, v, vc3);
            Color dark2 = AddValue(h, s, v, vc4);
            Color darkHigh = AddValue(h, s, v, vc5);

            SsuiAppTheme ssat = new SsuiAppTheme(accentColor);
            ColorsHelper.ToHSV(dark, out h, out s, out v);

            ssat.WindowBackground = Color.FromRgb(15, 15, 15).ToBrush();
            ssat.BaseBackground = Color.FromRgb(15, 15, 15).ToBrush();
            ssat.PanelBackground = Color.FromRgb(0, 0, 0).ToBrush();
            ssat.ControlBackground = Color.FromRgb(0, 0, 0).ToBrush();
            ssat.ControlSatBrush = Color.FromRgb(55, 55, 55).ToBrush();
            ssat.WindowTitleBackground = Color.FromRgb(55, 55, 55).ToBrush();
            ssat.WindowCaptionsBackground = Color.FromRgb(55, 55, 55).ToBrush();
            ssat.WindowInactiveBackground = Color.FromRgb(55, 55, 55).ToBrush();
            ssat.WindowTitleForeground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.WindowInactiveForeground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.ButtonBackground = Color.FromRgb(40, 40, 40).ToBrush();
            ssat.HighlightBrush = darkHigh.ToBrush();
            ssat.TabHighlightBrush = darkHigh.ToBrush();
            ssat.ClickBrush = dark2.ToBrush();
            ssat.Foreground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.TabBackground = Color.FromRgb(40, 40, 40).ToBrush();
            ssat.CommandBarBackground = Color.FromRgb(40, 40, 40).ToBrush();
            ssat.TabSelectedBrush = Color.FromRgb(0, 0, 0).ToBrush();
            ssat.BorderBrush = Color.FromRgb(128, 128, 128).ToBrush();
            ssat.CommandBarBorderBrush = Color.FromRgb(128, 128, 128).ToBrush();
            ssat.SelectedBackgroundBrush = dark.ToBrush();
            ssat.SelectedBorderBrush = dark2.ToBrush();
            ssat.HighlightForeground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.WindowCaptionsForeground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.WindowCaptionsHighlightForeground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.WindowCaptionsHighlight = darkHigh.ToBrush();
            ssat.DisabledForeground = Color.FromRgb(255, 255, 255).ToBrush();
            //ssat.DisabledBorderBrush = Color.FromRgb(128, 128, 128).ToBrush();
            ssat.DisabledBackground = ColorsHelper.CreateFromHex("9D9D9D").ToBrush();

            return ssat;

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
        }

        /// <summary>
        /// A <see cref="SsuiAppTheme"/> with high contrast colors, with white text on a black background.
        /// </summary>
        public static SsuiAppTheme HighContrastWhiteOnBlack
        {
            get
            {
                SsuiAppTheme ssat = new SsuiAppTheme()
                {
                    WindowBackground = Colors.Black.ToBrush(),
                    BaseBackground = Colors.Black.ToBrush(),
                    PanelBackground = Colors.Black.ToBrush(),
                    ControlBackground = Colors.Black.ToBrush(),
                    ControlSatBrush = Colors.White.ToBrush(),
                    WindowCaptionsBackground = Colors.Black.ToBrush(),
                    WindowTitleBackground = ColorsHelper.HighContrastPurple.ToBrush(),
                    WindowInactiveBackground = ColorsHelper.HighContrastPurple.ToBrush(),
                    WindowTitleForeground = Colors.White.ToBrush(),
                    ButtonBackground = Colors.Black.ToBrush(),
                    BorderBrush = Colors.White.ToBrush(),
                    Foreground = Colors.White.ToBrush(),
                    HighlightForeground = Colors.Black.ToBrush(),
                    HighlightBrush = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    WindowCaptionsHighlight = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    ClickBrush = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    WindowCaptionsClickBrush = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    HighlightBorderBrush = Colors.White.ToBrush(),
                    WindowInactiveForeground = Colors.White.ToBrush(),
                    WindowCaptionsForeground = Colors.White.ToBrush(),
                    WindowCaptionsHighlightForeground = Colors.Black.ToBrush(),
                    ControlPopBrush = Colors.White.ToBrush(),
                    SelectedBorderBrush = Colors.White.ToBrush(),
                    SelectedBackgroundBrush = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    DisabledBackground = Colors.Black.ToBrush(),
                    DisabledBorderBrush = ColorsHelper.HighContrastLightGreen.ToBrush(),
                    DisabledForeground = ColorsHelper.HighContrastLightGreen.ToBrush(),
                    LightBorderBrush = Colors.White.ToBrush(),
                    TabBackground = Colors.Black.ToBrush(),
                    TabHighlightBorderBrush = Colors.White.ToBrush(),
                    TabSelectedBrush = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    TabHighlightBrush = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    CommandBarBackground = Colors.Black.ToBrush(),
                    CommandBarBorderBrush = Colors.White.ToBrush(),
                    CheckBrush = Colors.Black.ToBrush(),
                    CheckHighlightBrush = Colors.White.ToBrush(),
                    CheckBackgroundHighlightBrush = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    IconVariation = Utils.IconVariation.White,
                    UseSubitemThemeWithMenus = false,
                    UseSubitemThemeWithPanels = false,
                    UseSubitemThemeWithRibbons = false,
                };

                ssat.AccentTheme = ssat.Copy();
                ssat.SubitemTheme = ssat.Copy();

                return ssat;
            }
        }

        /// <summary>
        /// A <see cref="SsuiAppTheme"/> with high contrast colors, with green text on a black background.
        /// </summary>
        public static SsuiAppTheme HighContrastGreenOnBlack
        {
            get
            {
                SsuiAppTheme ssat = new SsuiAppTheme()
                {
                    WindowBackground = Colors.Black.ToBrush(),
                    BaseBackground = Colors.Black.ToBrush(),
                    PanelBackground = Colors.Black.ToBrush(),
                    ControlBackground = Colors.Black.ToBrush(),
                    ControlSatBrush = Colors.Green.ToBrush(),
                    WindowCaptionsBackground = Colors.Black.ToBrush(),
                    WindowTitleBackground = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    WindowInactiveBackground = ColorsHelper.HighContrastPurple.ToBrush(),
                    WindowTitleForeground = Colors.Black.ToBrush(),
                    ButtonBackground = Colors.Black.ToBrush(),
                    BorderBrush = Colors.White.ToBrush(),
                    Foreground = ColorsHelper.HighContrastGreen.ToBrush(),
                    HighlightForeground = Colors.Black.ToBrush(),
                    HighlightBrush = ColorsHelper.HighContrastBlue.ToBrush(),
                    WindowCaptionsHighlight = ColorsHelper.HighContrastBlue.ToBrush(),
                    ClickBrush = ColorsHelper.HighContrastBlue.ToBrush(),
                    WindowCaptionsClickBrush = ColorsHelper.HighContrastBlue.ToBrush(),
                    HighlightBorderBrush = Colors.White.ToBrush(),
                    WindowInactiveForeground = Colors.White.ToBrush(),
                    WindowCaptionsForeground = Colors.Black.ToBrush(),
                    WindowCaptionsHighlightForeground = Colors.Black.ToBrush(),
                    ControlPopBrush = Colors.White.ToBrush(),
                    SelectedBorderBrush = Colors.White.ToBrush(),
                    SelectedBackgroundBrush = ColorsHelper.HighContrastBlue.ToBrush(),
                    DisabledBackground = Colors.Black.ToBrush(),
                    DisabledBorderBrush = ColorsHelper.HighContrastGray.ToBrush(),
                    DisabledForeground = ColorsHelper.HighContrastGray.ToBrush(),
                    LightBorderBrush = Colors.White.ToBrush(),
                    TabBackground = Colors.Black.ToBrush(),
                    TabHighlightBorderBrush = Colors.White.ToBrush(),
                    TabSelectedBrush = ColorsHelper.HighContrastBlue.ToBrush(),
                    TabHighlightBrush = ColorsHelper.HighContrastBlue.ToBrush(),
                    CommandBarBackground = Colors.Black.ToBrush(),
                    CommandBarBorderBrush = Colors.White.ToBrush(),
                    CheckBrush = Colors.Black.ToBrush(),
                    CheckHighlightBrush = Colors.White.ToBrush(),
                    CheckBackgroundHighlightBrush = ColorsHelper.HighContrastBlue.ToBrush(),
                    IconVariation = Utils.IconVariation.White,
                    UseSubitemThemeWithMenus = false,
                    UseSubitemThemeWithPanels = false,
                    UseSubitemThemeWithRibbons = false,
                };

                ssat.AccentTheme = ssat.Copy();
                ssat.SubitemTheme = ssat.Copy();

                return ssat;
            }
        }

        /// <summary>
        /// A <see cref="SsuiAppTheme"/> with high contrast colors, with black text on a white background.
        /// </summary>
        public static SsuiAppTheme HighContrastBlackOnWhite
        {
            get
            {
                SsuiAppTheme ssat = new SsuiAppTheme()
                {
                    WindowBackground = Colors.White.ToBrush(),
                    BaseBackground = Colors.White.ToBrush(),
                    PanelBackground = Colors.White.ToBrush(),
                    ControlBackground = Colors.White.ToBrush(),
                    ControlSatBrush = Colors.White.ToBrush(),
                    WindowCaptionsBackground = Colors.White.ToBrush(),
                    WindowTitleBackground = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    WindowInactiveBackground = ColorsHelper.HighContrastLightBlue.ToBrush(),
                    WindowTitleForeground = Colors.Black.ToBrush(),
                    ButtonBackground = Colors.White.ToBrush(),
                    BorderBrush = Colors.Black.ToBrush(),
                    Foreground = Colors.Black.ToBrush(),
                    HighlightForeground = Colors.Black.ToBrush(),
                    HighlightBrush = ColorsHelper.HighContrastLightPurple.ToBrush(),
                    WindowCaptionsHighlight = ColorsHelper.HighContrastLightPurple.ToBrush(),
                    ClickBrush = ColorsHelper.HighContrastLightPurple.ToBrush(),
                    WindowCaptionsClickBrush = ColorsHelper.HighContrastLightPurple.ToBrush(),
                    HighlightBorderBrush = Colors.Black.ToBrush(),
                    WindowInactiveForeground = Colors.White.ToBrush(),
                    WindowCaptionsForeground = Colors.Black.ToBrush(),
                    WindowCaptionsHighlightForeground = Colors.Black.ToBrush(),
                    ControlPopBrush = Colors.Black.ToBrush(),
                    SelectedBorderBrush = Colors.Black.ToBrush(),
                    SelectedBackgroundBrush = ColorsHelper.HighContrastLightPurple.ToBrush(),
                    DisabledBackground = Colors.Black.ToBrush(),
                    DisabledBorderBrush = ColorsHelper.HighContrastRed.ToBrush(),
                    DisabledForeground = ColorsHelper.HighContrastRed.ToBrush(),
                    LightBorderBrush = Colors.White.ToBrush(),
                    TabBackground = Colors.White.ToBrush(),
                    TabHighlightBorderBrush = Colors.Black.ToBrush(),
                    TabSelectedBrush = ColorsHelper.HighContrastLightPurple.ToBrush(),
                    TabHighlightBrush = ColorsHelper.HighContrastLightPurple.ToBrush(),
                    CommandBarBackground = Colors.White.ToBrush(),
                    CommandBarBorderBrush = Colors.Black.ToBrush(),
                    CheckBrush = Colors.Black.ToBrush(),
                    CheckHighlightBrush = Colors.Black.ToBrush(),
                    CheckBackgroundHighlightBrush = ColorsHelper.HighContrastLightPurple.ToBrush(),
                    IconVariation = Utils.IconVariation.Black,
                    UseSubitemThemeWithMenus = false,
                    UseSubitemThemeWithPanels = false,
                    UseSubitemThemeWithRibbons = false,
                };

                ssat.AccentTheme = ssat.Copy();
                ssat.SubitemTheme = ssat.Copy();

                return ssat;
            }
        }

        /// <summary>
        /// Get a high contrast <see cref="SsuiAppTheme"/> based upon the type of high contrast option selected.
        /// </summary>
        /// <param name="option">the type of high contrast theme to get</param>
        public static SsuiAppTheme GetHighContrastTheme(HighContrastOption option)
        {
            switch (option)
            {
                case HighContrastOption.WhiteOnBlack:
                    return HighContrastWhiteOnBlack;
                case HighContrastOption.GreenOnBlack:
                    return HighContrastGreenOnBlack;
                case HighContrastOption.BlackOnWhite:
                    return HighContrastBlackOnWhite;
                default:
                    return HighContrastWhiteOnBlack;
            }
        }
    }
}
