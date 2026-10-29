using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using SolidShineUi.Utils;

namespace SolidShineUi;

/// <summary>
/// A dialog for the user to be able to select a color, either from a preset list, or via sliders, an image source, or from a color palette file.
/// </summary>
public partial class ColorPickerDialog : ThemedUserControl
{

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
    /// <param name="color">The Color to preset as the selected color (i.e. as an existing or default value).</param>
    public ColorPickerDialog(Color color)
    {
        InitializeComponent();

        LoadInSelectedColor(color);
    }

    void LoadInSelectedColor(Color col)
    {
        grdCurColor.Background = BrushFactory.Create(col);
        nudAlpha.Value = col.A;
        UpdateSelectedColor(col);
    }

    #region Properties / Events

    ///// <inheritdoc/>
    //protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    //{
    //    base.OnPropertyChanged(change);

    //    switch (change.Property.Name)
    //    {
    //
    //    }
    //}

    #region SelectedColor

    private Color _selectedColor = Colors.Black;

    /// <summary>
    /// Get or set the color that is selected in the dialog.
    /// </summary>
    public Color SelectedColor { get => _selectedColor; private set => SetAndRaise(SelectedColorProperty, ref _selectedColor, value); }

    /// <summary>The backing direct property for <see cref="SelectedColor"/>. See the related property for details.</summary>
    public static readonly DirectProperty<ColorPickerDialog, Color> SelectedColorProperty
        = AvaloniaProperty.RegisterDirect<ColorPickerDialog, Color>(nameof(SelectedColor), (s) => s.SelectedColor, unsetValue: Colors.Black);

    /// <summary>
    /// Update the value of <see cref="SelectedColor"/>.
    /// </summary>
    /// <param name="col">the new value for <see cref="SelectedColor"/></param>
    /// <param name="includeSliders">whether the sliders on the Sliders tab should be updated; this should usually be <c>true</c></param>
    protected void UpdateSelectedColor(Color col, bool includeSliders = true)
    {
        if (nudAlpha != null && nudAlpha.Value != col.A)
        {
            UpdateSelectedColor(col.With(a: (byte)nudAlpha.Value));
            return;
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

    #region Transparency

    /// <summary>
    /// Get or set if clicking on a swatch in the Swatches tab resets the transparency (alpha) value to 255, or totally opaque. If not, the transparency value remains unchanged.
    /// </summary>
    public bool SwatchesResetTransparency { get => GetValue(SwatchesResetTransparencyProperty); set => SetValue(SwatchesResetTransparencyProperty, value); }

    /// <summary>The backing styled property for <see cref="SwatchesResetTransparency"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> SwatchesResetTransparencyProperty
        = AvaloniaProperty.Register<ColorPickerDialog, bool>(nameof(SwatchesResetTransparency), true);

    /// <summary>
    /// Get or set if transparency controls should be shown in the dialog. If true, then the user will be able to select and change the transparency (alpha) value of the selected color.
    /// </summary>
    public bool ShowTransparencyControls { get => GetValue(ShowTransparencyControlsProperty); set => SetValue(ShowTransparencyControlsProperty, value); }

    /// <summary>The backing styled property for <see cref="ShowTransparencyControls"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> ShowTransparencyControlsProperty
        = AvaloniaProperty.Register<ColorPickerDialog, bool>(nameof(ShowTransparencyControls), true);

    #region UI Value Changes

    bool _internalAlphaChange = false;

    private void nudAlpha_ValueChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_internalAlphaChange) return;
        if (sldAlpha == null) return;

        _internalAlphaChange = true;
        sldAlpha.Value = (e.NewValue is int oval) ? oval : 0;
        UpdateSelectedColor(SelectedColor);
        _internalAlphaChange = false;
    }

    private void sldAlpha_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (_internalAlphaChange) return;
        if (nudAlpha == null) return;

        _internalAlphaChange = true;
        nudAlpha.Value = (int)e.NewValue;
        UpdateSelectedColor(SelectedColor);
        _internalAlphaChange = false;
    }


    #endregion

    #endregion

    #region Text Labels

    /// <summary>
    /// Get or set the label to display next to the Transparency slider. Default is "Transparency:".
    /// </summary>
    public string TransparencyLabel { get => GetValue(TransparencyLabelProperty); set => SetValue(TransparencyLabelProperty, value); }

    /// <summary>The backing styled property for <see cref="TransparencyLabel"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> TransparencyLabelProperty
        = AvaloniaProperty.Register<ColorPickerDialog, string>(nameof(TransparencyLabel), "Transparency:");

    /// <summary>
    /// Get or set the label to display next to the currently selected color. Default is "Selected Color:".
    /// </summary>
    public string SelectedColorLabel { get => GetValue(SelectedColorLabelProperty); set => SetValue(SelectedColorLabelProperty, value); }

    /// <summary>The backing styled property for <see cref="SelectedColorLabel"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> SelectedColorLabelProperty
        = AvaloniaProperty.Register<ColorPickerDialog, string>(nameof(SelectedColorLabel), "Selected Color:");

    /// <summary>
    /// Get or set the label to display next to the current/preset color (as in, the color that the dialog was loaded with). Default is "Current Color:".
    /// </summary>
    public string CurrentColorLabel { get => GetValue(CurrentColorLabelProperty); set => SetValue(CurrentColorLabelProperty, value); }

    /// <summary>The backing styled property for <see cref="CurrentColorLabel"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> CurrentColorLabelProperty
        = AvaloniaProperty.Register<ColorPickerDialog, string>(nameof(CurrentColorLabel), "Current Color:");


    #endregion

    #region Title

    /// <summary>
    /// Get or set the title to use for this dialog, if used as a window.
    /// </summary>
    public string Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    /// <summary>The backing styled property for <see cref="Title"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<ColorPickerDialog, string>(nameof(Title), "");

    #endregion

    #region DialogResult

    /// <summary>
    /// Get the result of the dialog when it is closed. 
    /// "False" refers to the user cancelling the operation, while "True" refers to the user confirming, by clicking "OK" or pressing the Enter key.
    /// </summary>
    public bool DialogResult { get => GetValue(DialogResultProperty); set => SetValue(DialogResultProperty, value); }

    /// <summary>The backing styled property for <see cref="DialogResult"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> DialogResultProperty
        = AvaloniaProperty.Register<ColorPickerDialog, bool>(nameof(DialogResult), false);

    #endregion

    #region Closing

    /// <summary>
    /// Request for this dialog to be closed. This raising the <c>Closing</c> event.
    /// <para/>
    /// If this dialog is in a window via <see cref="DisplayAsWindow(WindowBase?)"/>, then it will trigger closing the window. 
    /// Otherwise if you are not using a window, you should then add a listener to the Closing event, so that you can modify
    /// your UI as needed when this Close function is called.
    /// </summary>
    public void Close()
    {
        CancelEventArgs ce = new CancelEventArgs();
        Closing?.Invoke(this, ce);
        if (ce.Cancel) return;

        CloseWindow?.Invoke(this, ce);
    }

    /// <summary>
    /// Raised when the dialog is requested to be closed, with the ability to cancel the closing.
    /// </summary>
    public event CancelEventHandler? Closing;

    private event EventHandler? CloseWindow;

    #endregion

    #endregion

    #region Window

    /// <summary>
    /// Display this dialog as a window.
    /// </summary>
    /// <param name="owner">The owner of the window, if there is one.</param>
    public ThemedWindow DisplayAsWindow(WindowBase? owner = null)
    {
        ThemedWindow tw = owner != null ? new ThemedWindow(owner) : new ThemedWindow();
        tw.Content = this;
        tw.Title = this.Title;

        tw.Width = this.Width;
        tw.Height = this.Height;

        CloseWindow += (s, e) => tw.Close();

        if (this.SsuiTheme is SsuiAppTheme)
        {
            tw.SsuiTheme = this.SsuiTheme;
        }
        Bind(SsuiThemeProperty, CompiledBinding.Create<ThemedWindow, SsuiTheme?>(win => win.SsuiTheme, source: tw));

        return tw;
    }

    #endregion

    #region Color Tabs

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

        UpdateSelectedColor(((sender as Button)!.Background as SolidColorBrush)!.Color);
    }

    #endregion

    #region Sliders

    private void btnInvert_Click(object sender, RoutedEventArgs e)
    {
        UpdateSelectedColor(SelectedColor.GetInversion());
    }

    bool updating = false;

    private void sldR_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (!updating)
        {
            nudR.Value = (int)e.NewValue;
        }
    }

    private void sldG_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (!updating)
        {
            nudG.Value = (int)e.NewValue;
        }
    }

    private void sldB_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (!updating)
        {
            nudB.Value = (int)e.NewValue;
        }
    }

    private void sldH_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (!updating)
        {
            nudH.Value = (int)e.NewValue;
        }
    }

    private void sldS_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (!updating)
        {
            nudS.Value = e.NewValue / 1000;
        }
    }

    private void sldV_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (!updating)
        {
            nudV.Value = e.NewValue / 1000;
        }
    }

    private void txtHex_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(txtHex.Text))
        {
            lblQ.IsVisible = true;
            return;
        }

        try
        {
            Color col = ColorsHelper.CreateFromHex(txtHex.Text);
            if (lblQ != null)
            {
                // apparently, even though I catch NullReferenceExceptions here, the debugger still wants to throw this error?
                // so I'll just add in a null reference check anyway
                lblQ.IsVisible = false;
            }

            UpdateValues(col, "Hex");
        }
        catch (FormatException)
        {
            lblQ.IsVisible = true;
        }
        catch (ArgumentOutOfRangeException)
        {
            lblQ.IsVisible = true;
        }
        catch (NullReferenceException)
        {

        }
    }
    private void nudR_ValueChanged(object sender, AvaloniaPropertyChangedEventArgs e)
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

    private void nudG_ValueChanged(object sender, AvaloniaPropertyChangedEventArgs e)
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

    private void nudB_ValueChanged(object sender, AvaloniaPropertyChangedEventArgs e)
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

    private void nudH_ValueChanged(object sender, AvaloniaPropertyChangedEventArgs e)
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

    private void nudS_ValueChanged(object sender, AvaloniaPropertyChangedEventArgs e)
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

    private void nudV_ValueChanged(object sender, AvaloniaPropertyChangedEventArgs e)
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

    #endregion

    #region From Palette

    private async void btnLoadPal_Click(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null) { return; } // not part of a window or larger UI element with a TopLevel

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Palette",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("All Compatible Files") { Patterns = ["*.pal", "*.aco", "*.hex", "*.txt", "*.gpl"] },
                new FilePickerFileType("JASC/RIFF Palette") { Patterns = ["*.pal"] },
                new FilePickerFileType("Photoshop Palette Files") { Patterns = ["*.aco"] },
                new FilePickerFileType("Basic Hex/RGB Color List") { Patterns = ["*.hex", "*.txt"] },
                new FilePickerFileType("Paint.NET Palette") { Patterns = ["*.txt"] },
                new FilePickerFileType("PowerToys Exported Color List") { Patterns = ["*.txt"] },
                new FilePickerFileType("GIMP Palette") { Patterns = ["*.gpl"] },
            ]

            // in case I need this for pre-.NET 8
            //FileTypeFilter = new List<FilePickerFileType>()
            //{
            //    new FilePickerFileType("All Compatible Files") { Patterns = new List<string>() { "*.pal", "*.aco", "*.hex", "*.txt", "*.gpl" } },
            //    new FilePickerFileType("JASC/RIFF Palette") { Patterns = new List<string>() { "*.pal" } },
            //    new FilePickerFileType("Photoshop Palette Files") { Patterns = new List<string>() { "*.aco" } },
            //    new FilePickerFileType("Basic Hex/RGB Color List") { Patterns = new List<string>() { "*.hex", "*.txt" } },
            //    new FilePickerFileType("Paint.NET Palette") { Patterns = new List<string>() { "*.txt" } },
            //    new FilePickerFileType("PowerToys Exported Color List") { Patterns = new List<string>() { "*.txt" } },
            //    new FilePickerFileType("GIMP Palette") { Patterns = new List<string>() { "*.gpl" } },
            //}
        });

        if (files.Count > 0)
        {
            // determine file based upon extension
            // not best thing, but it's what I have for right now
            string? file = files[0].TryGetLocalPath();

            if (file == null) { return; }

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
                    default:
                        // couldn't determine, just leave it empty
                        break;
                }
            }
            catch (FormatException)
            {
                //MessageBox.Show(ex.Message, "Color Palette", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            catch (InvalidDataException)
            {
                //MessageBox.Show(ex.Message, "Color Palette", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            catch (ArgumentNullException)
            {
                //MessageBox.Show("An error occurred while trying to read the file. Make sure the file still exists.", "Color Palette", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            catch (FileNotFoundException)
            {
                //MessageBox.Show("The file you selected could not be opened.", "Color Palette", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            catch (AccessViolationException)
            {
                //MessageBox.Show("The file you selected could not be opened.", "Color Palette", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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

    #endregion

    #region Dialog Actions

    /// <summary>
    /// Accept the current input in <see cref="SelectedColor"/>, and attempt to close the dialog.
    /// </summary>
    /// <remarks>
    /// This sets <see cref="DialogResult"/> to <c>true</c>.
    /// </remarks>
    public void Accept()
    {
        DialogResult = true;
        Close();
    }

    /// <summary>
    /// Cancel the input, and attempt to close the dialog.
    /// </summary>
    /// <remarks>
    /// This sets <see cref="DialogResult"/> to <c>false</c>.
    /// </remarks>
    public void Cancel()
    {
        DialogResult = false;
        Close();
    }

    #endregion

    #region Dialog Buttons

    /// <summary>
    /// Get or set if the dialog buttons ("OK" and "Cancel") should be shown.
    /// <para/>
    /// If this is set to <c>false</c>, you should have your own UI for allowing the user to accept or reject their input via mouse
    /// or touch, and also listen to the <see cref="Closing"/> event to determine when you should hide the dialog.
    /// </summary>
    public bool ShowDialogButtons { get => GetValue(ShowDialogButtonsProperty); set => SetValue(ShowDialogButtonsProperty, value); }

    /// <summary>The backing styled property for <see cref="ShowDialogButtons"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> ShowDialogButtonsProperty
        = AvaloniaProperty.Register<ColorPickerDialog, bool>(nameof(ShowDialogButtons), true);

    /// <summary>
    /// Get or set the text to display for the "OK" button, which is used for accepting the input.
    /// </summary>
    public string OkButtonText { get => GetValue(OkButtonTextProperty); set => SetValue(OkButtonTextProperty, value); }

    /// <summary>The backing styled property for <see cref="OkButtonText"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> OkButtonTextProperty
        = AvaloniaProperty.Register<ColorPickerDialog, string>(nameof(OkButtonText), "OK");

    /// <summary>
    /// Get or set the text to display for the "Cancel" button, which is used for rejecting the input.
    /// </summary>
    public string CancelButtonText { get => GetValue(CancelButtonTextProperty); set => SetValue(CancelButtonTextProperty, value); }

    /// <summary>The backing styled property for <see cref="CancelButtonText"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> CancelButtonTextProperty
        = AvaloniaProperty.Register<ColorPickerDialog, string>(nameof(CancelButtonText), "Cancel");


    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
        Accept();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Cancel();
    }

    #endregion
}