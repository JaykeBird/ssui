using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
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
        #region Window Constructors / Loaded

        /// <summary>
        /// Create a ColorPickerDialog.
        /// </summary>
        public ColorPickerDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a ColorPickerDialog.
        /// </summary>
        /// <param name="cs">The ColorScheme to use with this dialog.</param>
        public ColorPickerDialog(ColorScheme cs)
        {
            InitializeComponent();
            ColorScheme = cs;
        }

        /// <summary>
        /// Create a ColorPickerDialog.
        /// </summary>
        /// <param name="color">The Color to preset as the selected color (i.e. as an existing or default value).</param>
        public ColorPickerDialog(Color color)
        {
            InitializeComponent();

            LoadInSelectedColor(color);
        }

        /// <summary>
        /// Create a ColorPickerDialog.
        /// </summary>
        /// <param name="cs">The ColorScheme to use with this dialog.</param>
        /// <param name="color">The Color to preset as the selected color (i.e. as an existing or default value).</param>
        public ColorPickerDialog(ColorScheme cs, Color color)
        {
            InitializeComponent();
            ColorScheme = cs;
            
            LoadInSelectedColor(color);
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Icon == null && Owner != null && Owner.Icon != null)
            {
                Icon = Owner.Icon.Clone();
            }
            else
            {
                ShowIcon = false;
            }
        }

        #endregion

        #region General

        /// <summary>
        /// Get or set the color that is selected in the dialog.
        /// </summary>
        public Color SelectedColor { get; private set; } = ColorsHelper.Black;

        /// <summary>
        /// Get or set the result of this dialog when closed. Set to true when the user clicks OK.
        /// </summary>
        public new bool DialogResult { get; set; } = false;

        void LoadInSelectedColor(Color col)
        {
            grdCurColor.Background = BrushFactory.Create(col);
            nudAlpha.Value = col.A;
            UpdateSelectedColor(col);
        }

        void UpdateSelectedColor(Color col, bool includeSliders = true)
        {
            if (nudAlpha != null)
            {
                col.A = (byte)nudAlpha.Value;
            }

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

        #region Tab Visibility / Titles

        /// <summary>
        /// Get or set if the Swatches tab is visible in the dialog.
        /// </summary>
        public bool ShowSwatchesTab
        {
            get => tabSwatches.Visibility == Visibility.Visible;
            set => tabSwatches.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Get or set if the Sliders tab is visible in the dialog.
        /// </summary>
        public bool ShowSlidersTab
        {
            get => tabSliders.Visibility == Visibility.Visible;
            set => tabSliders.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Get or set if the From Image tab is visible in the dialog.
        /// </summary>
        public bool ShowImageTab
        {
            get => tabImage.Visibility == Visibility.Visible;
            set => tabImage.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Get or set if the Palette File tab is visible in the dialog.
        /// </summary>
        public bool ShowPaletteFileTab
        {
            get => tabPalette.Visibility == Visibility.Visible;
            set => tabPalette.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Get or set the title of the Swatches tab. Default is "Swatches".
        /// </summary>
        public string SwatchesTabTitle { get => tabSwatches.Title; set => tabSwatches.Title = value; }

        /// <summary>
        /// Get or set the title of the Sliders tab. Default is "Sliders".
        /// </summary>
        public string SlidersTabTitle { get => tabSliders.Title; set => tabSliders.Title = value; }

        /// <summary>
        /// Get or set the title of the From Image tab. Default is "From Image".
        /// </summary>
        public string ImageTabTitle { get => tabImage.Title; set => tabImage.Title = value; }

        /// <summary>
        /// Get or set the title of the Palette File tab. Default is "Palette File".
        /// </summary>
        public string PaletteFileTabTitle { get => tabPalette.Title; set => tabPalette.Title = value; }


        #endregion

        #region Swatches
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (SwatchesResetTransparency)
            {
                nudAlpha.Value = 255;
            }

            if (sender is ColorSwatchButton csb)
            {
                UpdateSelectedColor(csb.Color);
                return;
            }

#if NETCOREAPP
            UpdateSelectedColor(((sender as Button)!.Background as SolidColorBrush)!.Color);
#else
            UpdateSelectedColor(((sender as Button).Background as SolidColorBrush).Color);
#endif
        }

        #endregion

        #region Sliders

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter

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
            grdSlColor.Background = BrushFactory.Create(col);

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
            grdSlColor.Background = BrushFactory.Create(col);

            updating = false;
        }

#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0060 // Remove unused parameter
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

            foreach (IClickSelectableControl item in colorList.Items)
            {
                if (item is ColorListItem cli)
                {
                    cli.DisplayMode = cld;
                }
            }
        }

        private void colorList_SelectionChanged(object sender, RoutedSelectionChangedEventArgs<IClickSelectableControl> e)
        {
            if (colorList.Items.Count == 0) return;
            UpdateSelectedColor(colorList.Items.SelectedItems.OfType<ColorListItem>().First().Color);
        }

        #endregion

        #region Transparency

        /// <summary>
        /// Get or set if transparency controls should be shown in the dialog. If true, then the user will be able to select and change the transparency (alpha) value of the selected color.
        /// </summary>
        public bool ShowTransparencyControls { get => (bool)GetValue(ShowTransparencyControlsProperty); set => SetValue(ShowTransparencyControlsProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static DependencyProperty ShowTransparencyControlsProperty
            = DependencyProperty.Register("ShowTransparencyControls", typeof(bool), typeof(ColorPickerDialog),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set if clicking on a swatch in the Swatches tab resets the transparency (alpha) value to 255, or totally opaque. If not, the transparency value remains unchanged.
        /// </summary>
        public bool SwatchesResetTransparency { get => (bool)GetValue(SwatchesResetTransparencyProperty); set => SetValue(SwatchesResetTransparencyProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static DependencyProperty SwatchesResetTransparencyProperty
            = DependencyProperty.Register("SwatchesResetTransparency", typeof(bool), typeof(ColorPickerDialog),
            new FrameworkPropertyMetadata(true));


        /// <summary>
        /// Get or set the label to display next to the Transparency slider. Default is "Transparency:".
        /// </summary>
        public string TransparencyLabel { get => (string)GetValue(TransparencyLabelProperty); set => SetValue(TransparencyLabelProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static DependencyProperty TransparencyLabelProperty
            = DependencyProperty.Register("TransparencyLabel", typeof(string), typeof(ColorPickerDialog),
            new FrameworkPropertyMetadata("Transparency:"));

        bool _internalAlphaChange = false;

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
        private void nudAlpha_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAlphaChange) return;
            if (sldAlpha == null) return;

            _internalAlphaChange = true;
            sldAlpha.Value = (e.NewValue is int oval) ? oval : 0;
            UpdateSelectedColor(SelectedColor);
            _internalAlphaChange = false;
        }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private members

        private void sldAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_internalAlphaChange) return;
            if (nudAlpha == null) return;

            _internalAlphaChange = true;
            nudAlpha.Value = (int)e.NewValue;
            UpdateSelectedColor(SelectedColor);
            _internalAlphaChange = false;
        }

        #endregion

        #region Other Text Labels

        /// <summary>
        /// Get or set the label to display next to the currently selected color. Default is "Selected Color:".
        /// </summary>
        public string SelectedColorLabel { get => (string)GetValue(SelectedColorLabelProperty); set => SetValue(SelectedColorLabelProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static DependencyProperty SelectedColorLabelProperty
            = DependencyProperty.Register("SelectedColorLabel", typeof(string), typeof(ColorPickerDialog),
            new FrameworkPropertyMetadata("Selected Color:"));

        /// <summary>
        /// Get or set the label to display next to the current/preset color (as in, the color that the dialog was loaded with). Default is "Current Color:".
        /// </summary>
        public string CurrentColorLabel { get => (string)GetValue(CurrentColorLabelProperty); set => SetValue(CurrentColorLabelProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static DependencyProperty CurrentColorLabelProperty
            = DependencyProperty.Register("CurrentColorLabel", typeof(string), typeof(ColorPickerDialog),
            new FrameworkPropertyMetadata("Current Color:"));

        /// <summary>
        /// Get or set the label to display on the "Invert" button in the Sliders tab; clicking this button inverts the current color. Default is "Invert".
        /// </summary>
        public string InvertButtonLabel { get => btnInvert.Content.ToString() ?? ""; set => btnInvert.Content = value; }

        /// <summary>
        /// Get or set the label to display on the "Invert" button in the Sliders tab; clicking this button inverts the current color. Default is "Invert".
        /// </summary>
        public string OkButtonLabel { get => btnOK.Content.ToString() ?? ""; set => btnOK.Content = value; }

        /// <summary>
        /// Get or set the label to display on the "Invert" button in the Sliders tab; clicking this button inverts the current color. Default is "Invert".
        /// </summary>
        public string CancelButtonLabel { get => btnCancel.Content.ToString() ?? ""; set => btnCancel.Content = value; }


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
