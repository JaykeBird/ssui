using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using SolidShineUi.Utils;

namespace SolidShineUi;

/// <summary>
/// A dialog for the user to select a font, as well as being able to choose a font size and styling if desired.
/// </summary>
public partial class FontSelectDialog : ThemedUserControl
{
    /// <summary>
    /// Create a FontSelectDialog.
    /// </summary>
    public FontSelectDialog()
    {
        InitializeComponent();
        List<string> fonts = AvailableFonts();
        AutoCompleteBehavior.SetAutoCompleteItemsSource(txtFont, fonts);
        foreach (var fontName in fonts)
        {
            lstFont.Items.Add(fontName);
        }
    }

    /// <summary>
    /// Get a list of system fonts.
    /// </summary>
    /// <returns></returns>
    public static List<string> AvailableFonts()
    {
        return FontManager.Current.SystemFonts.Select(item => item.Name).ToList();
    }

    /// <summary>
    /// The list of font weights that are displayed in the dialog. Note that there's a lot of repeats of FontWeights in the base enum.
    /// </summary>
    public static List<FontWeight> AvailableFontWeights = new List<FontWeight>()
    {
        FontWeight.Thin,
        FontWeight.ExtraLight,
        FontWeight.Light,
        FontWeight.Normal,
        FontWeight.Medium,
        FontWeight.SemiBold,
        FontWeight.Bold,
        FontWeight.ExtraBold,
        FontWeight.Black,
        FontWeight.ExtraBlack

        /*
        FontWeight.Black,
        FontWeight.Bold,
        FontWeight.DemiBold,
        FontWeight.ExtraBlack,
        FontWeight.ExtraBold,
        FontWeight.ExtraLight,
        FontWeight.Heavy,
        FontWeight.Light,
        FontWeight.Medium,
        FontWeight.Normal,
        FontWeight.Regular,
        FontWeight.SemiBold,
        FontWeight.Thin,
        FontWeight.UltraBlack,
        FontWeight.UltraBold,
        FontWeight.UltraLight
        */
    };

    #region Properties

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
    }

    #region Selection Properties

    bool _updateUi = true;

    private FontFamily _selectedFontFamily = FontFamily.Default;

    /// <summary>
    /// Get or set the font family selected in the dialog.
    /// </summary>
    public FontFamily SelectedFontFamily { get => _selectedFontFamily; set => SetFontFamily(ref _selectedFontFamily, value); }

    /// <summary>The backing direct property for <see cref="SelectedFontFamily"/>. See the related property for details.</summary>
    public static readonly DirectProperty<FontSelectDialog, FontFamily> SelectedFontFamilyProperty
        = AvaloniaProperty.RegisterDirect<FontSelectDialog, FontFamily>(nameof(SelectedFontFamily), (s) => s.SelectedFontFamily, (s, v) => s.SelectedFontFamily = v, unsetValue: FontFamily.Default);

    void SetFontFamily(ref FontFamily fontFamilyValue, FontFamily newValue)
    {
        txtPreview.FontFamily = newValue;

        if (_updateUi)
        {
            txtFont.Text = newValue.Name;
        }

        SetAndRaise(SelectedFontFamilyProperty, ref fontFamilyValue, newValue);
    }

    private double _selectedFontSize = 12.0;

    /// <summary>
    /// Get or set the selected font size in the dialog.
    /// </summary>
    public double SelectedFontSize { get => _selectedFontSize; set => SetFontSize(ref _selectedFontSize, value); }

    /// <summary>The backing direct property for <see cref="SelectedFontSize"/>. See the related property for details.</summary>
    public static readonly DirectProperty<FontSelectDialog, double> SelectedFontSizeProperty
        = AvaloniaProperty.RegisterDirect<FontSelectDialog, double>(nameof(SelectedFontSize), (s) => s.SelectedFontSize, (s, v) => s.SelectedFontSize = v, unsetValue: 12.0);

    void SetFontSize(ref double fontSizeValue, double newValue)
    {
        txtPreview.FontSize = newValue;

        if (_updateUi)
        {
            txtSize.Value = newValue;
            if (txtSize.Value == 12.0 && lstSize.SelectedIndex == -1) // special handling if it's the default value
            {
                lstSize.SelectedItem = 12;
            }
        }

        SetAndRaise(SelectedFontSizeProperty, ref fontSizeValue, newValue);
    }

    private FontWeight _selectedFontWeight = (FontWeight)400;

    /// <summary>
    /// Get or set the selected font weight in the dialog.
    /// </summary>
    public FontWeight SelectedFontWeight { get => _selectedFontWeight; set => SetFontWeight(ref _selectedFontWeight, value); }

    /// <summary>The backing direct property for <see cref="SelectedFontWeight"/>. See the related property for details.</summary>
    public static readonly DirectProperty<FontSelectDialog, FontWeight> SelectedFontWeightProperty
        = AvaloniaProperty.RegisterDirect<FontSelectDialog, FontWeight>(nameof(SelectedFontWeight), (s) => s.SelectedFontWeight, (s, v) => s.SelectedFontWeight = v, unsetValue: (FontWeight)400);

    void SetFontWeight(ref FontWeight fontWeightValue, FontWeight newValue)
    {
        txtPreview.FontWeight = newValue;

        if (_updateUi)
        {
            txtWeight.Value = (int)newValue;
            if (txtWeight.Value == 400 && lstWeight.SelectedIndex == -1) // special handling if it's the default value
            {
                lstWeight.SelectedItem = 400;
            }
        }

        SetAndRaise(SelectedFontWeightProperty, ref fontWeightValue, newValue);
    }

    private FontStyle _selectedFontStyle = FontStyle.Normal;

    /// <summary>
    /// Get or set the selected font style in the dialog.
    /// </summary>
    public FontStyle SelectedFontStyle { get => _selectedFontStyle; set => SetFontStyle(ref _selectedFontStyle, value); }

    /// <summary>The backing direct property for <see cref="SelectedFontStyle"/>. See the related property for details.</summary>
    public static readonly DirectProperty<FontSelectDialog, FontStyle> SelectedFontStyleProperty
        = AvaloniaProperty.RegisterDirect<FontSelectDialog, FontStyle>(nameof(SelectedFontStyle), (s) => s.SelectedFontStyle, (s, v) => s.SelectedFontStyle = v, unsetValue: FontStyle.Normal);

    void SetFontStyle(ref FontStyle fontStyleValue, FontStyle newValue)
    {
        txtPreview.FontStyle = newValue;

        if (_updateUi)
        {
            chkItalic.IsChecked = false;
            chkOblique.IsChecked = false;

            switch (newValue)
            {
                case FontStyle.Normal:
                    break;
                case FontStyle.Italic:
                    chkItalic.IsChecked = true;
                    break;
                case FontStyle.Oblique:
                    chkOblique.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        SetAndRaise(SelectedFontStyleProperty, ref fontStyleValue, newValue);
    }

    private TextDecorationCollection _selectedTextDecorations = [];

    /// <summary>
    /// Get or set the selected text decorations in the dialog.
    /// </summary>
    public TextDecorationCollection SelectedTextDecorations { get => _selectedTextDecorations; set => SetTextDecorations(ref _selectedTextDecorations, value); }

    /// <summary>The backing direct property for <see cref="SelectedTextDecorations"/>. See the related property for details.</summary>
    public static readonly DirectProperty<FontSelectDialog, TextDecorationCollection> SelectedTextDecorationsProperty
        = AvaloniaProperty.RegisterDirect<FontSelectDialog, TextDecorationCollection>(nameof(SelectedTextDecorations), (s) => s.SelectedTextDecorations, (s, v) => s.SelectedTextDecorations = v, unsetValue: []);

    void SetTextDecorations(ref TextDecorationCollection textDecorationsValue, TextDecorationCollection newValue)
    {
        txtPreview.TextDecorations = newValue;

        if (_updateUi)
        {
            chkBaseline.IsChecked = false;
            chkOverline.IsChecked = false;
            chkStrike.IsChecked = false;
            chkUnderline.IsChecked = false;

            if (newValue.Any(d => d.Location == TextDecorationLocation.Baseline)) chkBaseline.IsChecked = true;
            if (newValue.Any(d => d.Location == TextDecorationLocation.Overline)) chkOverline.IsChecked = true;
            if (newValue.Any(d => d.Location == TextDecorationLocation.Strikethrough)) chkStrike.IsChecked = true;
            if (newValue.Any(d => d.Location == TextDecorationLocation.Underline)) chkUnderline.IsChecked = true;
        }

        SetAndRaise(SelectedTextDecorationsProperty, ref textDecorationsValue, newValue);
    }

    #endregion

    #region Title

    /// <summary>
    /// Get or set the title to use for this dialog, if used as a window.
    /// </summary>
    public string Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    /// <summary>The backing styled property for <see cref="Title"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<FontSelectDialog, string>(nameof(Title), "");

    #endregion

    #region DialogResult

    /// <summary>
    /// Get the result of the dialog when it is closed. 
    /// "False" refers to the user cancelling the operation, while "True" refers to the user confirming, by clicking "OK" or pressing the Enter key.
    /// </summary>
    public bool DialogResult { get => GetValue(DialogResultProperty); set => SetValue(DialogResultProperty, value); }

    /// <summary>The backing styled property for <see cref="DialogResult"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> DialogResultProperty
        = AvaloniaProperty.Register<FontSelectDialog, bool>(nameof(DialogResult), false);

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

    #region Show UI Elements

    /// <summary>
    /// Get or set if the styles options are visible.
    /// </summary>
    public bool ShowStyles { get => GetValue(ShowStylesProperty); set => SetValue(ShowStylesProperty, value); }

    /// <summary>The backing styled property for <see cref="ShowStyles"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> ShowStylesProperty
        = AvaloniaProperty.Register<FontSelectDialog, bool>(nameof(ShowStyles), true);


    /// <summary>
    /// Get or set if the text decoration options are visible.
    /// </summary>
    public bool ShowDecorations { get => GetValue(ShowDecorationsProperty); set => SetValue(ShowDecorationsProperty, value); }

    /// <summary>The backing styled property for <see cref="ShowDecorations"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> ShowDecorationsProperty
        = AvaloniaProperty.Register<FontSelectDialog, bool>(nameof(ShowDecorations), true);


    /// <summary>
    /// Get or set if the font weight options are visible.
    /// </summary>
    public bool ShowWeights { get => GetValue(ShowWeightsProperty); set => SetValue(ShowWeightsProperty, value); }

    /// <summary>The backing styled property for <see cref="ShowWeights"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> ShowWeightsProperty
        = AvaloniaProperty.Register<FontSelectDialog, bool>(nameof(ShowWeights), true);


    /// <summary>
    /// Get or set if the font size options are visible.
    /// </summary>
    public bool ShowSizes { get => GetValue(ShowSizesProperty); set => SetValue(ShowSizesProperty, value); }

    /// <summary>The backing styled property for <see cref="ShowSizes"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> ShowSizesProperty
        = AvaloniaProperty.Register<FontSelectDialog, bool>(nameof(ShowSizes), true);


    /// <summary>
    /// Get or set if the preview text box is visible.
    /// </summary>
    public bool ShowPreviewText { get => GetValue(ShowPreviewTextProperty); set => SetValue(ShowPreviewTextProperty, value); }

    /// <summary>The backing styled property for <see cref="ShowPreviewText"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> ShowPreviewTextProperty
        = AvaloniaProperty.Register<FontSelectDialog, bool>(nameof(ShowPreviewText), true);


    /// <summary>
    /// Get or set if the dialog buttons are visible.
    /// </summary>
    public bool ShowDialogButtons { get => GetValue(ShowDialogButtonsProperty); set => SetValue(ShowDialogButtonsProperty, value); }

    /// <summary>The backing styled property for <see cref="ShowDialogButtons"/>. See the related property for details.</summary>
    public static readonly StyledProperty<bool> ShowDialogButtonsProperty
        = AvaloniaProperty.Register<FontSelectDialog, bool>(nameof(ShowDialogButtons), true);

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

        tw.CanResize = false;

        CloseWindow += (s, e) => tw.Close();

        if (this.SsuiTheme is SsuiAppTheme)
        {
            tw.SsuiTheme = this.SsuiTheme;
        }
        Bind(SsuiThemeProperty, CompiledBinding.Create<ThemedWindow, SsuiTheme?>(win => win.SsuiTheme, source: tw));

        return tw;
    }

    #endregion

    #region UI Events

    #region List Boxes

    bool listBoxAvoidUpdate = false;

    private void txtFont_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(txtFont.Text)) return;

        if (!listBoxAvoidUpdate)
        {
            if (lstFont.Items.Contains(txtFont.Text))
            {
                lstFont.SelectedItem = txtFont.Text;
            }
        }

        _updateUi = false;
        SelectedFontFamily = new FontFamily(txtFont.Text);
        _updateUi = true;
    }

    private void lstFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lstFont.SelectedIndex == -1)
        {
            return;
        }
        else if (lstFont.SelectedItem is string s)
        {
            listBoxAvoidUpdate = true;
            txtFont.Text = s;
            listBoxAvoidUpdate = false;
        }
    }

    private void txtSize_ValueChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (lstSize == null)
        {
            return;
        }

        if (!listBoxAvoidUpdate)
        {
            if (Math.Floor(txtSize.Value) == txtSize.Value)
            {
                if (lstSize.Items.Contains((int)txtSize.Value))
                {
                    lstSize.SelectedItem = (int)txtSize.Value;
                }
                else
                {
                    lstSize.SelectedIndex = -1;
                }
            }
            else
            {
                lstSize.SelectedIndex = -1;
            }
        }

        _updateUi = false;
        SelectedFontSize = txtSize.Value;
        _updateUi = true;
    }

    private void lstSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lstSize.SelectedIndex == -1)
        {
            return;
        }
        else if (lstSize.SelectedItem is int i)
        {
            listBoxAvoidUpdate = true;
            txtSize.Value = i;
            listBoxAvoidUpdate = false;
        }
    }

    private void txtWeight_ValueChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (lstWeight == null)
        {
            return;
        }

        if (!listBoxAvoidUpdate)
        {
            if (lstWeight.Items.Contains(txtWeight.Value))
            {
                lstWeight.SelectedItem = txtWeight.Value;
            }
            else
            {
                lstWeight.SelectedIndex = -1;
            }
        }

        _updateUi = false;
        SelectedFontWeight = (FontWeight)txtWeight.Value;
        _updateUi = true;
    }

    private void lstWeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lstWeight.SelectedIndex == -1)
        {
            return;
        }
        else if (lstWeight.SelectedItem is int i)
        {
            listBoxAvoidUpdate = true;
            txtWeight.Value = i;
            listBoxAvoidUpdate = false;
        }
    }

    #endregion

    #region Text Decorations

    private void TextDecorationCheckChanged(object? sender, RoutedEventArgs e)
    {
        _updateUi = false;
        UpdateSelectedTextDecorations();
        _updateUi = true;
    }

    void UpdateSelectedTextDecorations()
    {
        TextDecorationCollection tdc = new TextDecorationCollection();

        if (chkBaseline.IsChecked == true) tdc.Add(new TextDecoration() { Location = TextDecorationLocation.Baseline });
        if (chkOverline.IsChecked == true) tdc.Add(new TextDecoration() { Location = TextDecorationLocation.Overline });
        if (chkStrike.IsChecked == true) tdc.Add(new TextDecoration() { Location = TextDecorationLocation.Strikethrough });
        if (chkUnderline.IsChecked == true) tdc.Add(new TextDecoration() { Location = TextDecorationLocation.Underline });

        SelectedTextDecorations = tdc;
    }

    #endregion

    #region Italic / Oblique

    private void chkItalic_CheckChanged(object? sender, RoutedEventArgs e)
    {
        if (!_updateUi) return;

        _updateUi = false;
        if (chkItalic.IsChecked) chkOblique.IsChecked = false; // mutually exclusive
        UpdateSelectedFontStyle();
        _updateUi = true;
    }

    private void chkOblique_CheckChanged(object? sender, RoutedEventArgs e)
    {
        if (!_updateUi) return;

        _updateUi = false;
        if (chkOblique.IsChecked) chkItalic.IsChecked = false; // mutually exclusive
        UpdateSelectedFontStyle();
        _updateUi = true;
    }

    void UpdateSelectedFontStyle()
    {
        if (chkItalic.IsChecked == true)
        {
            SelectedFontStyle = FontStyle.Italic;
        }
        else if (chkOblique.IsChecked == true)
        {
            SelectedFontStyle = FontStyle.Oblique;
        }
        else
        {
            SelectedFontStyle = FontStyle.Normal;
        }
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

    private void btnText_Click(object sender, RoutedEventArgs e)
    {
        //StringInputDialog sid = new StringInputDialog("Set Preview Text", "Set the text to appear in the Preview box.", txtPreview.Text);
        //sid.SsuiTheme = SsuiTheme;
        //sid.ShowDialog();
        //if (sid.DialogResult)
        //{
        //    txtPreview.Text = sid.Value;
        //}
    }

    /// <summary>
    /// Get or set the preview text displayed at the bottom of the dialog, to allow the user to see how their font selections look.
    /// </summary>
    public string PreviewText { get => GetValue(PreviewTextProperty); set => SetValue(PreviewTextProperty, value); }

    /// <summary>The backing styled property for <see cref="PreviewText"/>. See the related property for details.</summary>
    public static readonly StyledProperty<string> PreviewTextProperty
        = AvaloniaProperty.Register<FontSelectDialog, string>(nameof(PreviewText), "AaBbCc123!");


    #endregion
}