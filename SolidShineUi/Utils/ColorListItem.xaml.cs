using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.Utils
{

    public enum ColorListItemDisplay
    {
        Hex = 0, Rgb = 1, Hsv = 2,
    }

    /// <summary>
    /// Interaction logic for ColorListItem.xaml
    /// </summary>
    public partial class ColorListItem : SelectableUserControl
    {
        public ColorListItem()
        {
            InitializeComponent();
        }

        public ColorListItem(Color col)
        {
            InitializeComponent();

            Color = col;
        }

        public ColorListItem(Color col, ColorListItemDisplay displayMode)
        {
            InitializeComponent();

            Color = col;
            DisplayMode = displayMode;
        }

        private ColorListItemDisplay cld = ColorListItemDisplay.Hex;

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
