using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SolidShineUi.Utils;

namespace SolidShineUi
{
    /// <summary>
    /// A dialog for the user to be able to select a color, either from a preset list, or via sliders, an image source, or from a color palette file.
    /// </summary>
    public partial class ColorPickerDialog : FlatWindow
    {
        ColorScheme cs = new ColorScheme();

        #region Window Actions

        [Obsolete("Please use the constructor with the ColorScheme argument.", false)]
        public ColorPickerDialog()
        {
            InitializeComponent();

            UpdateAppearance();
        }

        public ColorPickerDialog(ColorScheme cs)
        {
            this.cs = cs;
            InitializeComponent();

            UpdateAppearance();
        }

        public ColorPickerDialog(ColorScheme cs, Color color)
        {
            this.cs = cs;
            InitializeComponent();

            UpdateAppearance();
            LoadInSelectedColor(color);
        }

        void UpdateAppearance()
        {
            ColorScheme = cs;
            grid.Background = cs.BackgroundColor.ToBrush();
            //Background = BrushFactory.Create(cs.MainColor);
            //grid.Background = BrushFactory.Create(cs.BackgroundColor);
            //SelectionBrush = BrushFactory.Create(cs.SelectionColor);
            //CaptionButtonsBrush = BrushFactory.Create(cs.ForegroundColor);
            //HighlightBrush = BrushFactory.Create(cs.HighlightColor);
            //BorderBrush = BrushFactory.Create(cs.BorderColor);
            //Foreground = BrushFactory.Create(cs.ForegroundColor);
            //InactiveTextBrush = BrushFactory.Create("#505050");

            colorList.ApplyColorScheme(cs);

            nudB.ApplyColorScheme(cs);
            nudG.ApplyColorScheme(cs);
            nudR.ApplyColorScheme(cs);

            nudH.ApplyColorScheme(cs);
            nudS.ApplyColorScheme(cs);
            nudV.ApplyColorScheme(cs);

            btnInvert.ApplyColorScheme(cs);
            btnLoadPal.ApplyColorScheme(cs);
            btnOpenImage.ApplyColorScheme(cs);

            btnOK.ApplyColorScheme(cs);
            btnCancel.ApplyColorScheme(cs);
        }

        #endregion

        #region General

        public Color SelectedColor { get; private set; } = ColorsHelper.Black;
        public new bool DialogResult { get; set; } = false;

        void LoadInSelectedColor(Color col)
        {
            grdCurColor.Background = BrushFactory.Create(col);
            UpdateSelectedColor(col);
        }

        void UpdateSelectedColor(Color col, bool includeSliders = true)
        {
            SelectedColor = col;

            if (grdSelColor != null)
            {
                grdSelColor.Background = BrushFactory.Create(col);
            }

            if (includeSliders)
            {
                UpdateValues(col, "");
            }
        }

        #endregion

        #region Tab Visibility

        public bool ShowSwatchesTab
        {
            get => tabSwatches.Visibility == Visibility.Visible;
            set => tabSwatches.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool ShowSlidersTab
        {
            get => tabSliders.Visibility == Visibility.Visible;
            set => tabSliders.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool ShowImageTab
        {
            get => tabImage.Visibility == Visibility.Visible;
            set => tabImage.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool ShowPaletteFileTab
        {
            get => tabPalette.Visibility == Visibility.Visible;
            set => tabPalette.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Swatches
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
#if NETCOREAPP
            UpdateSelectedColor(((sender as Button)!.Background as SolidColorBrush)!.Color);
#else
            UpdateSelectedColor(((sender as Button).Background as SolidColorBrush).Color);
#endif
        }

        #endregion

        #region Sliders

        private void btnInvert_Click(object sender, RoutedEventArgs e)
        {
            UpdateSelectedColor(SelectedColor.GetInversion());
        }

        bool updating = false;

        private void sldR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudR.Value = (int)e.NewValue;
            }
        }

        private void sldG_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudG.Value = (int)e.NewValue;
            }
        }

        private void sldB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudB.Value = (int)e.NewValue;
            }
        }

        private void sldH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudH.Value = (int)e.NewValue;
            }
        }

        private void sldS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudS.Value = e.NewValue / 1000;
            }
        }

        private void sldV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudV.Value = e.NewValue / 1000;
            }
        }

        private void txtHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Color col = ColorsHelper.CreateFromHex(txtHex.Text);
                if (lblQ != null)
                {
                    // apparently, even though I catch NullReferenceExceptions here, the debugger still wants to throw this error?
                    // so I'll just add in a null reference check anyway
                    lblQ.Visibility = Visibility.Collapsed;
                }

                UpdateValues(col, "Hex");
            }
            catch (FormatException)
            {
                lblQ.Visibility = Visibility.Visible;
            }
            catch (ArgumentOutOfRangeException)
            {
                lblQ.Visibility = Visibility.Visible;
            }
            catch (NullReferenceException)
            {

            }
        }

        private void nudR_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!updating)
            {
                try
                {
                    Color col = ColorsHelper.CreateFromRgb((byte)nudR.Value, (byte)nudG.Value, (byte)nudB.Value);

                    UpdateValues(col, "RGB");
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void nudG_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!updating)
            {
                try
                {
                    Color col = ColorsHelper.CreateFromRgb((byte)nudR.Value, (byte)nudG.Value, (byte)nudB.Value);

                    UpdateValues(col, "RGB");
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void nudB_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!updating)
            {
                try
                {
                    Color col = ColorsHelper.CreateFromRgb((byte)nudR.Value, (byte)nudG.Value, (byte)nudB.Value);

                    UpdateValues(col, "RGB");
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void nudH_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!updating)
            {
                try
                {
                    //Color col = ColorsHelper.CreateFromHSV(nudH.Value, nudS.Value, nudV.Value);

                    UpdateValuesHsv(nudH.Value, nudS.Value, nudV.Value);

                    //UpdateValues(col, "HSV");
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void nudS_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!updating)
            {
                try
                {
                    //Color col = ColorsHelper.CreateFromHSV(nudH.Value, nudS.Value, nudV.Value);

                    UpdateValuesHsv(nudH.Value, nudS.Value, nudV.Value);

                    //UpdateValues(col, "HSV");
                }
                catch (NullReferenceException)
                {

                }
            }

        }

        private void nudV_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!updating)
            {
                try
                {
                    //Color col = ColorsHelper.CreateFromHSV(nudH.Value, nudS.Value, nudV.Value);

                    UpdateValuesHsv(nudH.Value, nudS.Value, nudV.Value);

                    //UpdateValues(col, "HSV");
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        void UpdateValues(Color col, string except)
        {
            if (except == "all")
            {
                return;
            }

            updating = true;

            if (except != "RGB")
            {
                nudR.Value = col.R;
                nudG.Value = col.G;
                nudB.Value = col.B;

                sldR.Value = col.R;
                sldG.Value = col.G;
                sldB.Value = col.B;
            }

            if (except != "HSV")
            {
                ColorsHelper.ToHSV(col, out double h, out double s, out double v);

                nudH.Value = (int)h;
                nudS.Value = s;
                nudV.Value = v;

                sldH.Value = h;
                sldS.Value = s * 1000;
                sldV.Value = v * 1000;
            }

            if (except != "Hex")
            {
                txtHex.Text = col.GetHexString();
            }

            UpdateSelectedColor(col, false);
            brdrSlColor.Background = BrushFactory.Create(col);

            updating = false;

        }

        void UpdateValuesHsv(double h, double s, double v)
        {
            updating = true;

            Color col = ColorsHelper.CreateFromHSV(h, s, v);

            nudR.Value = col.R;
            nudG.Value = col.G;
            nudB.Value = col.B;

            sldR.Value = col.R;
            sldG.Value = col.G;
            sldB.Value = col.B;

            txtHex.Text = col.GetHexString();

            updating = true;

            nudH.Value = (int)h;
            nudS.Value = s;
            nudV.Value = v;

            sldH.Value = h;
            sldS.Value = s * 1000;
            sldV.Value = v * 1000;

            UpdateSelectedColor(col, false);
            brdrSlColor.Background = BrushFactory.Create(col);

            updating = false;
        }

        #endregion

        #region From Image

        private void btnOpenImage_Click(object sender, RoutedEventArgs e)
        {
            string fPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Images (*.png,*.jpg,*.jpeg,*.bmp,*.gif,*.wmf)|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.wmf|All Files (*.*)|*.*";

            ofd.InitialDirectory = fPath;

            bool? res = ofd.ShowDialog();

            if (res.HasValue && res.Value)
            {
                imgPicker.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
            }
        }

        private void imgPicker_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            if (imgPicker.SelectedColor != null)
            {
                UpdateSelectedColor(imgPicker.SelectedColor.Value);
            }
        }

        #endregion

        #region From Palette

        private void btnLoadPal_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Title = "Open Palette";
            ofd.Filter = "All Compatible Files|*.pal;*.aco;*.hex;*.txt;*.gpl;*.gif;*.tif;*.tiff|" +
                "JASC/RIFF Palette|*.pal|Photoshop Palette Files|*.aco|" +
                "Basic Hex/RGB Color List|*.hex;*.txt|Paint.NET Palette|*.txt|PowerToys Exported Color List|*.txt|" +
                "GIMP Palette|*.gpl|GIF/TIFF Images (first 256 colors)|*.gif;*.tif;*.tiff";
            ofd.Multiselect = false;

            bool? result = ofd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                // determine file based upon extension
                // not best thing, but it's what I have for right now
                string file = ofd.FileName;

                List<Color> colors = new List<Color>();

                // get extension; remove the period (".txt" -> "txt")
                string ext = Path.GetExtension(file).Substring(1);

                try
                {
                    switch (ext)
                    {
                        case "pal":
                            colors = ColorPaletteFileReader.LoadPalFilePalette(file);
                            break;
                        case "aco":
                            colors = ColorPaletteFileReader.LoadPhotoshopPalette(file);
                            break;
                        case "txt":
                            colors = ColorPaletteFileReader.LoadPaintNetPalette(file);
                            if (colors.Count == 0)
                            {
                                colors = ColorPaletteFileReader.LoadPowerToysColorList(file);
                            }
                            break;
                        case "hex":
                            colors = ColorPaletteFileReader.LoadPaintNetPalette(file);
                            break;
                        case "gpl":
                            colors = ColorPaletteFileReader.LoadGimpPalette(file);
                            break;
                        case "gif":
                        case "tif":
                        case "tiff":
                            colors = ColorPaletteFileReader.LoadBitmapImagePalette(file);
                            break;
                        default:
                            // couldn't determine, just leave it empty
                            break;
                    }
                }
                catch (FormatException ex)
                {
                    MessageBox.Show(ex.Message, "Color Palette", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
                catch (InvalidDataException ex)
                {
                    MessageBox.Show(ex.Message, "Color Palette", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("An error occurred while trying to read the file. Make sure the file still exists.", "Color Palette", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("The file you selected could not be opened.", "Color Palette", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
                catch (AccessViolationException)
                {
                    MessageBox.Show("The file you selected could not be opened.", "Color Palette", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }

                ColorListItemDisplay cld;
                switch (cbbListDisplay.SelectedIndex)
                {
                    case 0: // Hex
                        cld = ColorListItemDisplay.Hex;
                        break;
                    case 1: // RGB
                        cld = ColorListItemDisplay.Rgb;
                        break;
                    case 2: // HSV
                        cld = ColorListItemDisplay.Hsv;
                        break;
                    default:
                        cld = ColorListItemDisplay.Hex;
                        break;
                }

                colorList.Items.Clear();

                // now that we have our list of colors
                foreach (Color item in colors)
                {
                    ColorListItem cli = new ColorListItem();
                    cli.Color = item;
                    cli.DisplayMode = cld;
                    colorList.Items.Add(cli);
                }
            }
        }

        private void cbbListDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbListDisplay == null || colorList == null)
            {
                return;
            }

            ColorListItemDisplay cld;
            switch (cbbListDisplay.SelectedIndex)
            {
                case 0: // Hex
                    cld = ColorListItemDisplay.Hex;
                    break;
                case 1: // RGB
                    cld = ColorListItemDisplay.Rgb;
                    break;
                case 2: // HSV
                    cld = ColorListItemDisplay.Hsv;
                    break;
                default:
                    cld = ColorListItemDisplay.Hex;
                    break;
            }

            foreach (ColorListItem item in colorList.Items)
            {
                item.DisplayMode = cld;
            }
        }

        private void colorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (colorList.Items.Count == 0) return;
            UpdateSelectedColor(colorList.Items.OfType<ColorListItem>().First().Color);
        }

        #endregion

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
