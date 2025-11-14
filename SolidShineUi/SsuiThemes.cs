using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
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
        /// Note that this will have a very Windows 98/2000-ish appearance on modern computers, 
        /// given that <see cref="SystemColors"/> only provides basic color brushes.
        /// </remarks>
        public static SsuiAppTheme SystemTheme
        {
            get
            {
                return new SsuiAppTheme()
                {
                    BaseBackground = SystemColors.WindowBrush,
                    PanelBackground = SystemColors.ControlLightLightBrush,
                    BorderBrush = SystemColors.ControlDarkDarkBrush,
                    LightBorderBrush = SystemColors.ControlDarkBrush,
                    ControlBackground = SystemColors.ControlBrush,
                    ControlPopBrush = SystemColors.HighlightBrush,
                    ControlSatBackground = SystemColors.ControlLightBrush,
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
                    UseSubitemThemeWithMenus = false,
                    UseSubitemThemeWithPanels = false,
                    UseSubitemThemeWithRibbons = false,
                    CornerRadius = new CornerRadius(0)
                };
            }
        }

        /// <summary>
        /// A SsuiAppTheme that uses the system's colors for the controls, and adds a slight rounded corners to the edges of many controls.
        /// </summary>
        /// <remarks>
        /// Note that this will have a very Windows 98/2000-ish appearance on modern computers, 
        /// given that <see cref="SystemColors"/> only provides basic color brushes.
        /// </remarks>
        public static SsuiAppTheme SystemThemeRoundedCorners
        {
            get
            {
                return new SsuiAppTheme()
                {
                    BaseBackground = SystemColors.WindowBrush,
                    PanelBackground = SystemColors.ControlLightLightBrush,
                    BorderBrush = SystemColors.ControlDarkDarkBrush,
                    LightBorderBrush = SystemColors.ControlDarkBrush,
                    ControlBackground = SystemColors.ControlBrush,
                    ControlPopBrush = SystemColors.HighlightBrush,
                    ControlSatBackground = SystemColors.ControlLightBrush,
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
                    UseSubitemThemeWithMenus = false,
                    UseSubitemThemeWithPanels = false,
                    UseSubitemThemeWithRibbons = false,
                    CornerRadius = new CornerRadius(3)
                };
            }
        }


        public static SsuiAppTheme LightTheme
        {
            get => CreateLightTheme(ColorsHelper.CreateFromHex("A8A8A8"));
        }

        public static SsuiAppTheme DarkTheme
        {
            get => CreateDarkTheme(ColorsHelper.CreateFromHex("C8C8C8"));
        }


        /// <summary>
        /// Create a premade light theme color scheme. Lighter gray colors are used, and a custom accent color can be provided to add some more color.
        /// </summary>
        /// <param name="accentColor">The accent color to use with the light theme, to add more color to certain elements.</param>
        /// <returns></returns>
        public static SsuiAppTheme CreateLightTheme(Color accentColor)
        {
            return new SsuiAppTheme(accentColor)
            {
                WindowBackground = Color.FromRgb(246, 246, 246).ToBrush(),
                BaseBackground = Color.FromRgb(246, 246, 246).ToBrush(),
                PanelBackground = Color.FromRgb(255, 255, 255).ToBrush(),
                ControlSatBackground = Color.FromRgb(200, 200, 200).ToBrush(),
                WindowCaptionsBackground = Color.FromRgb(200, 200, 200).ToBrush(),
                WindowTitleBackground = Color.FromRgb(200, 200, 200).ToBrush(),
                WindowInactiveBackground = Color.FromRgb(200, 200, 200).ToBrush(),
                WindowTitleForeground = Color.FromRgb(0, 0, 0).ToBrush(),
                ControlBackground = Color.FromRgb(220, 220, 220).ToBrush(),
                BorderBrush = Color.FromRgb(128, 128, 128).ToBrush(),
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
        /// <returns></returns>
        public static SsuiAppTheme CreateDarkTheme(Color accentColor)
        {
            ColorsHelper.ToHSV(accentColor, out double h, out double s, out double v);
            double vc3 = -0.4;
            Color dark = AddValue(h, s, v, vc3);

            SsuiAppTheme ssat = new SsuiAppTheme(accentColor);
            ColorsHelper.ToHSV(dark, out h, out s, out v);

            Color dark2 = AddValue(h, s, v, -0.2);

            ssat.WindowBackground = Color.FromRgb(15, 15, 15).ToBrush();
            ssat.BaseBackground = Color.FromRgb(15, 15, 15).ToBrush();
            ssat.PanelBackground = Color.FromRgb(0, 0, 0).ToBrush();
            ssat.ControlSatBackground = Color.FromRgb(55, 55, 55).ToBrush();
            ssat.WindowTitleBackground = Color.FromRgb(55, 55, 55).ToBrush();
            ssat.WindowCaptionsBackground = Color.FromRgb(55, 55, 55).ToBrush();
            ssat.WindowInactiveBackground = Color.FromRgb(55, 55, 55).ToBrush();
            ssat.WindowTitleForeground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.WindowInactiveForeground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.ControlBackground = Color.FromRgb(40, 40, 40).ToBrush();
            ssat.HighlightBrush = dark.ToBrush();
            ssat.ClickBrush = dark2.ToBrush();
            ssat.BorderBrush = Color.FromRgb(128, 128, 128).ToBrush();
            ssat.Foreground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.HighlightForeground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.WindowCaptionsForeground = Color.FromRgb(255, 255, 255).ToBrush();
            ssat.WindowCaptionsHighlightForeground = Color.FromRgb(255, 255, 255).ToBrush();
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

    }
}
