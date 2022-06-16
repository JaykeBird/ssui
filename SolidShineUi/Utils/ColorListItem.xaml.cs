using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Indicates what text data to display with a ColorListItem.
    /// </summary>
    public enum ColorListItemDisplay
    {
        /// <summary>
        /// Display the HEX values.
        /// </summary>
        Hex = 0,
        /// <summary>
        /// Display the RGB values.
        /// </summary>
        Rgb = 1,
        /// <summary>
        /// Display the HSV values.
        /// </summary>
        Hsv = 2,
    }

    /// <summary>
    /// A SelectableUserControl that displays a color along with text representing that color. This can be used in a SelectPanel.
    /// </summary>
    public partial class ColorListItem : SelectableUserControl
    {
        /// <summary>
        /// Create a ColorListItem.
        /// </summary>
        public ColorListItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a ColorListItem, with the Color property preset.
        /// </summary>
        /// <param name="col">The color to display in this ColorListItem.</param>
        public ColorListItem(Color col)
        {
            InitializeComponent();

            Color = col;
        }

        /// <summary>
        /// Create a ColorListItem, with the Color and DisplayMode properties preset.
        /// </summary>
        /// <param name="col">The color to display in this ColorListItem.</param>
        /// <param name="displayMode">The display mode to use for the text in the ColorListItem.</param>
        public ColorListItem(Color col, ColorListItemDisplay displayMode)
        {
            InitializeComponent();

            Color = col;
            DisplayMode = displayMode;
        }

        private ColorListItemDisplay cld = ColorListItemDisplay.Hex;

        /// <summary>
        /// Get or set the text to display with the color, such as RGB values or HEX values.
        /// </summary>
        public ColorListItemDisplay DisplayMode
        {
            get
            {
                return cld;
            }
            set
            {
                cld = value;

                switch (cld)
                {
                    case ColorListItemDisplay.Hex:
                        lblColor.Text = "#" + col.GetHexString();
                        break;
                    case ColorListItemDisplay.Rgb:
                        lblColor.Text = col.R.ToString() + ", " + col.G.ToString() + ", " + col.B.ToString();
                        break;
                    case ColorListItemDisplay.Hsv:
                        ColorsHelper.ToHSV(col, out double h, out double s, out double v);
                        lblColor.Text = h.ToString("N0") + ", " + s.ToString("N3") + ", " + v.ToString("N3");
                        break;
                    default:
                        lblColor.Text = "#" + col.GetHexString();
                        break;
                }
            }
        }

        private Color col;

        /// <summary>
        /// Get or set the color to display in the ColorListItem.
        /// </summary>
        public Color Color
        {
            get
            {
                return col;
            }
            set
            {
                col = value;
                brdrColor.Background = col.ToBrush();

                switch (DisplayMode)
                {
                    case ColorListItemDisplay.Hex:
                        lblColor.Text = "#" + col.GetHexString();
                        break;
                    case ColorListItemDisplay.Rgb:
                        lblColor.Text = col.R.ToString() + ", " + col.G.ToString() + ", " + col.B.ToString();
                        break;
                    case ColorListItemDisplay.Hsv:
                        ColorsHelper.ToHSV(col, out double h, out double s, out double v);
                        lblColor.Text = h.ToString("N0") + ", " + s.ToString("N3") + ", " + v.ToString("N3");
                        break;
                    default:
                        lblColor.Text = "#" + col.GetHexString();
                        break;
                }
            }
        }
    }
}
