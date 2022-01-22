using System;
using System.Collections.Generic;
using static SolidShineUi.ColorsHelper;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// Available options for creating high-contrast color schemes. These are based upon color schemes available through Windows's High Contrast options.
    /// </summary>
    public enum HighContrastOption
    {
        WhiteOnBlack = 0,
        GreenOnBlack = 1,
        BlackOnWhite = 2
    }

    public class ColorScheme
    {
        /// <summary>
        /// Create a color scheme with no colors preset; default values are all grey. Can be used to custom build a scheme color by color.
        /// </summary>
        public ColorScheme()
        {

        }

        /// <summary>
        /// Create a color scheme with a single preset color as the basis. All other colors in the scheme are based off this color.
        /// </summary>
        /// <param name="mainColor"></param>
        public ColorScheme(Color mainColor)
        {
            CreatePalette(mainColor);
            CreateAccentPalette(mainColor);
        }

        /// <summary>
        /// Create a color scheme with a preset color for the basis, and an additional preset color to use as a secondary accent. All other colors are based off one of these two colors.
        /// </summary>
        /// <param name="mainColor"></param>
        /// <param name="accentColor"></param>
        public ColorScheme(Color mainColor, Color accentColor)
        {
            CreatePalette(mainColor);
            CreateAccentPalette(accentColor);
        }

        /// <summary>
        /// Get or set whether this is a high-contrast color scheme.
        /// </summary>
        public bool IsHighContrast { get; set; } = false;

        /// <summary>
        /// Main interface color. With High Contrast color schemes, this will be the same as BackgroundColor.
        /// </summary>
        public Color MainColor { get; set; } = Gray;
        /// <summary>
        /// Secondary color. Do not use with High Contrast color schemes
        /// </summary>
        public Color SecondaryColor { get; set; } = CreateFromHex("DDDDDD");
        public Color BackgroundColor { get; set; } = CreateFromHex("F4F4F4");
        public Color SelectionColor { get; set; } = CreateFromHex("4C4C4C");
        public Color HighlightColor { get; set; } = DarkGray;
        public Color BorderColor { get; set; } = CreateFromHex("333333");
        public Color SecondHighlightColor { get; set; } = Gray;
        public Color ThirdHighlightColor { get; set; } = DarkGray;
        public Color LightBackgroundColor { get; set; } = White;

        public Color ForegroundColor { get; set; } = Black;
        public Color ItemColor { get; set; } = Black;

        public Color LightDisabledColor { get; set; } = CreateFromHex("F5F5F5");
        public Color DarkDisabledColor { get; set; } = CreateFromHex("AAAAAF");

        public Color AccentMainColor { get; set; } = CreateFromHex("E34234");
        public Color AccentSecondaryColor { get; set; } = CreateFromHex("FFB9B2");
        public Color AccentBackgroundColor { get; set; } = CreateFromHex("FFF1F0");
        public Color AccentHighlightColor { get; set; } = CreateFromHex("C4392D");
        public Color AccentSelectionColor { get; set; } = CreateFromHex("B03328");
        public Color AccentBorderColor { get; set; } = CreateFromHex("7D241D");
        public Color AccentSecondHighlightColor { get; set; } = CreateFromHex("FFE3E0");
        public Color AccentThirdHighlightColor { get; set; } = CreateFromHex("FFD9D6");
        public Color AccentLightBackgroundColor { get; set; } = CreateFromHex("FFF8F7");

        public Color WindowTitleBarColor { get; set; } = Gray;
        public Color WindowTitleBarTextColor { get; set; } = Black;
        public Color WindowInactiveColor { get; set; } = DarkGray;

        public bool MenusUseAccent { get; set; } = false;
        //public bool PanelUseAccent { get; set; } = false;

        void CreatePalette(Color baseColor)
        {

            ToHSV(baseColor, out double h, out double s, out double v);

            double s1 = -0.12;
            double s2 = -0.2;
            double s3 = -0.4;

            double t1 = 0.15;
            double t2 = 0.45;
            double t3 = 0.27;
            double t4 = 0.9;

            double ssc = 0.3;
            double sec = 0.12;
            double stc = 0.16;
            double sc = 0.059;
            double sbc = 0.03;

            MainColor = baseColor;
            WindowTitleBarColor = baseColor;
            WindowInactiveColor = baseColor;
            HighlightColor = AddValue(h, s, v, s1);
            SelectionColor = AddValue(h, s, v, s2);
            BorderColor = AddValue(h, s, v, s3);

            if (s > ssc)
            {
                SecondaryColor = (AddValue(h, ssc, v, t1)); // 4 - Secondary Color
            }
            else
            {
                SecondaryColor = (AddValue(h, s, v, t1)); // 4
            }

            if (s > sc)
            {
                BackgroundColor = (AddValue(h, sc, v, t2)); // 5 - Background Color
            }
            else
            {
                BackgroundColor = (AddValue(h, s, v, t2)); // 5
            }

            if (s > sec)
            {
                SecondHighlightColor = (AddValue(h, sec, v, t3)); // 6 - Second Highlight COlor
            }
            else
            {
                SecondHighlightColor = (AddValue(h, s, v, t3)); // 6
            }

            if (s > stc)
            {
                ThirdHighlightColor = (AddValue(h, stc, v, t3)); // 7 - Menu Highlight Color
            }
            else
            {
                ThirdHighlightColor = (AddValue(h, s, v, t3)); // 7
            }

            if (s > sbc)
            {
                LightBackgroundColor = (AddValue(h, sbc, v, t4)); // 8 - Menu Background Color
            }
            else
            {
                LightBackgroundColor = (AddValue(h, s, v, t4)); // 8
            }
        }

        void CreateAccentPalette(Color baseColor)
        {

            ToHSV(baseColor, out double h, out double s, out double v);

            double s1 = -0.12;
            double s2 = -0.2;
            double s3 = -0.4;

            double t1 = 0.15;
            double t2 = 0.45;
            double t3 = 0.27;
            double t4 = 0.9;

            double ssc = 0.3;
            double sec = 0.12;
            double stc = 0.16;
            double sc = 0.059;
            double sbc = 0.03;

            AccentMainColor = baseColor;
            AccentHighlightColor = AddValue(h, s, v, s1);
            AccentSelectionColor = AddValue(h, s, v, s2);
            AccentBorderColor = AddValue(h, s, v, s3);

            if (s > ssc)
            {
                AccentSecondaryColor = (AddValue(h, ssc, v, t1)); // 4 - Secondary Color
            }
            else
            {
                AccentSecondaryColor = (AddValue(h, s, v, t1)); // 4
            }

            if (s > sc)
            {
                AccentBackgroundColor = (AddValue(h, sc, v, t2)); // 5 - Background Color
            }
            else
            {
                AccentBackgroundColor = (AddValue(h, s, v, t2)); // 5
            }

            if (s > sec)
            {
                AccentSecondHighlightColor = (AddValue(h, sec, v, t3)); // 6 - Second Highlight COlor
            }
            else
            {
                AccentSecondHighlightColor = (AddValue(h, s, v, t3)); // 6
            }

            if (s > stc)
            {
                AccentThirdHighlightColor = (AddValue(h, stc, v, t3)); // 7 - Menu Highlight Color
            }
            else
            {
                AccentThirdHighlightColor = (AddValue(h, s, v, t3)); // 7
            }

            if (s > sbc)
            {
                AccentLightBackgroundColor = (AddValue(h, sbc, v, t4)); // 8 - Menu Background Color
            }
            else
            {
                AccentLightBackgroundColor = (AddValue(h, s, v, t4)); // 8
            }
        }

        void CreateAccentPalette(ColorScheme baseScheme)
        {
            AccentBackgroundColor = baseScheme.BackgroundColor;
            AccentBorderColor = baseScheme.BorderColor;
            AccentHighlightColor = baseScheme.HighlightColor;
            AccentLightBackgroundColor = baseScheme.LightBackgroundColor;
            AccentMainColor = baseScheme.MainColor;
            AccentSecondaryColor = baseScheme.SecondaryColor;
            AccentSecondHighlightColor = baseScheme.SecondHighlightColor;
            AccentSelectionColor = baseScheme.SelectionColor;
            AccentThirdHighlightColor = baseScheme.ThirdHighlightColor;
        }

        public static ColorScheme CreateLightTheme()
        {
            return CreateLightTheme(CreateFromHex("A8A8A8"));
        }

        public static ColorScheme CreateLightTheme(Color accentColor)
        {
            ColorScheme cs = new ColorScheme(accentColor);
            cs.BackgroundColor = Color.FromRgb(246,246,246);
            cs.LightBackgroundColor = Color.FromRgb(255, 255, 255);
            cs.MainColor = Color.FromRgb(200, 200, 200);
            cs.WindowTitleBarColor = Color.FromRgb(200, 200, 200);
            cs.WindowInactiveColor = Color.FromRgb(200, 200, 200);
            cs.WindowTitleBarTextColor = Color.FromRgb(0, 0, 0);
            cs.SecondaryColor = Color.FromRgb(220, 220, 220);
            cs.BorderColor = Color.FromRgb(128, 128, 128);
            cs.ForegroundColor = Color.FromRgb(0, 0, 0);

            return cs;
        }

        public static ColorScheme CreateDarkTheme()
        {
            return CreateDarkTheme(CreateFromHex("C8C8C8"));
        }

        public static ColorScheme CreateDarkTheme(Color accentColor)
        {
            ColorScheme cs = new ColorScheme(accentColor);
            Color dark = cs.BorderColor;
            ToHSV(dark, out double h, out double s, out double v);

            Color dark2 = AddValue(h, s, v, -0.2);

            cs.BackgroundColor = Color.FromRgb(15, 15, 15);
            cs.LightBackgroundColor = Color.FromRgb(0, 0, 0);
            cs.MainColor = Color.FromRgb(55, 55, 55);
            cs.WindowTitleBarColor = Color.FromRgb(55, 55, 55);
            cs.WindowInactiveColor = Color.FromRgb(55, 55, 55);
            cs.WindowTitleBarTextColor = Color.FromRgb(255, 255, 255);
            cs.SecondaryColor = Color.FromRgb(40, 40, 40);
            cs.SecondHighlightColor = dark;
            cs.ThirdHighlightColor = dark2;
            cs.BorderColor = Color.FromRgb(128, 128, 128);
            cs.ForegroundColor = Color.FromRgb(255, 255, 255);
            cs.LightDisabledColor = CreateFromHex("9D9D9D");

            return cs;
        }

        /// <summary>
        /// Create a color scheme with high-contrast colors, based upon one of the available options.
        /// </summary>
        /// <param name="hco">The high-contrast scheme option to create.</param>
        /// <returns></returns>
        public static ColorScheme GetHighContrastScheme(HighContrastOption hco)
        {
            ColorScheme cs = new ColorScheme();
            cs.IsHighContrast = true;

            switch (hco)
            {
                case HighContrastOption.WhiteOnBlack:
                    cs.MainColor = Black; //HighContrastPurple;
                    cs.SecondaryColor = Gray; // items that use main or secondary color should use other options
                    cs.BackgroundColor = Black;
                    cs.LightBackgroundColor = Black;
                    cs.SelectionColor = HighContrastLightBlue;
                    cs.HighlightColor = HighContrastLightBlue;
                    cs.SecondHighlightColor = HighContrastLightBlue;
                    cs.ThirdHighlightColor = HighContrastLightBlue;
                    cs.ForegroundColor = White;
                    cs.BorderColor = White;
                    cs.LightDisabledColor = HighContrastLightGreen;
                    cs.DarkDisabledColor = HighContrastLightGreen;
                    cs.WindowTitleBarColor = HighContrastPurple;
                    cs.WindowTitleBarTextColor = White;
                    cs.WindowInactiveColor = HighContrastPurple;
                    break;
                case HighContrastOption.GreenOnBlack:
                    cs.MainColor = Black; //HighContrastLightBlue;
                    cs.SecondaryColor = Gray; // items that use main or secondary color should use other options
                    cs.BackgroundColor = Black;
                    cs.LightBackgroundColor = Black;
                    cs.SelectionColor = HighContrastBlue;
                    cs.HighlightColor = HighContrastBlue;
                    cs.SecondHighlightColor = HighContrastBlue;
                    cs.ThirdHighlightColor = HighContrastBlue;
                    cs.ForegroundColor = HighContrastGreen;
                    cs.BorderColor = White;
                    cs.LightDisabledColor = HighContrastGray;
                    cs.DarkDisabledColor = HighContrastGray;
                    cs.WindowTitleBarColor = HighContrastLightBlue;
                    cs.WindowTitleBarTextColor = Black;
                    cs.WindowInactiveColor = HighContrastPurple;
                    break;
                case HighContrastOption.BlackOnWhite:
                    cs.MainColor = White; //HighContrastLightBlue;
                    cs.SecondaryColor = Gray; // items that use main or secondary color should use other options
                    cs.BackgroundColor = White;
                    cs.LightBackgroundColor = White;
                    cs.SelectionColor = HighContrastLightPurple;
                    cs.HighlightColor = HighContrastLightPurple;
                    cs.SecondHighlightColor = HighContrastLightPurple;
                    cs.ThirdHighlightColor = HighContrastLightPurple;
                    cs.ForegroundColor = Black;
                    cs.BorderColor = Black;
                    cs.LightDisabledColor = HighContrastRed;
                    cs.DarkDisabledColor = HighContrastRed;
                    cs.WindowTitleBarColor = HighContrastLightBlue;
                    cs.WindowTitleBarTextColor = Black;
                    cs.WindowInactiveColor = HighContrastLightBlue;
                    break;
                default:
                    break;
            }

            cs.CreateAccentPalette(cs);

            return cs;
        }
    }
}
