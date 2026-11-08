using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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

        switch (change.Property.Name)
        {
            case nameof(SelectedFontFamily):
                UpdateFontFamily(change);
                break;
            case nameof(SelectedFontSize):
                UpdateFontSize(change);
                break;
            case nameof(SelectedFontStyle):
                UpdateFontStyle(change);
                break;
            case nameof(SelectedFontWeight):
                UpdateFontWeight(change);
                break;
            case nameof(SelectedTextDecorations):
                UpdateTextDecorations(change);
                break;
        }
    }

    #region Selection Properties

    /// <summary>
    /// Get or set if the UI should update a <c>Selected...</c> property when it is changed.
    /// <para/>
    /// Note that in the current implementation, what this does is prevent the UI from updating the 
    /// property again if the property is changing the appearance of the UI.
    /// </summary>
    bool _updateFromUi = true;

    private FontFamily? _selectedFontFamily = null;

    /// <summary>
    /// Get or set the font family selected in the dialog.
    /// </summary>
    public FontFamily? SelectedFontFamily { get => _selectedFontFamily; set => SetAndRaise(SelectedFontFamilyProperty, ref _selectedFontFamily, value); }

    /// <summary>The backing direct property for <see cref="SelectedFontFamily"/>. See the related property for details.</summary>
    public static readonly DirectProperty<FontSelectDialog, FontFamily?> SelectedFontFamilyProperty
        = AvaloniaProperty.RegisterDirect<FontSelectDialog, FontFamily?>(nameof(SelectedFontFamily), (s) => s.SelectedFontFamily, (s, v) => s.SelectedFontFamily = v, unsetValue: null);

    void UpdateFontFamily(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_updateFromUi) return;
        if (e.NewValue is not FontFamily ff) return;

        _updateFromUi = false;
        txtFont.Text = ff.Name;
        _updateFromUi = true;
    }

    private double _selectedFontSize = 12.0;

    /// <summary>
    /// Get or set the selected font size in the dialog.
    /// </summary>
    public double SelectedFontSize { get => _selectedFontSize; set => SetAndRaise(SelectedFontSizeProperty, ref _selectedFontSize, value); }

    /// <summary>The backing direct property for <see cref="SelectedFontSize"/>. See the related property for details.</summary>
    public static readonly DirectProperty<FontSelectDialog, double> SelectedFontSizeProperty
        = AvaloniaProperty.RegisterDirect<FontSelectDialog, double>(nameof(SelectedFontSize), (s) => s.SelectedFontSize, (s, v) => s.SelectedFontSize = v, unsetValue: 12.0);

    void UpdateFontSize(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_updateFromUi) return;
        if (e.NewValue is not double size) return;

        _updateFromUi = false;
        txtSize.Value = size;
        _updateFromUi = true;
    }

    private FontWeight _selectedFontWeight = (FontWeight)400;

    /// <summary>
    /// Get or set the selected font weight in the dialog.
    /// </summary>
    public FontWeight SelectedFontWeight { get => _selectedFontWeight; set => SetAndRaise(SelectedFontWeightProperty, ref _selectedFontWeight, value); }

    /// <summary>The backing direct property for <see cref="SelectedFontWeight"/>. See the related property for details.</summary>
    public static readonly DirectProperty<FontSelectDialog, FontWeight> SelectedFontWeightProperty
        = AvaloniaProperty.RegisterDirect<FontSelectDialog, FontWeight>(nameof(SelectedFontWeight), (s) => s.SelectedFontWeight, (s, v) => s.SelectedFontWeight = v, unsetValue: (FontWeight)400);

    void UpdateFontWeight(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_updateFromUi) return;
        if (e.NewValue is not FontWeight weight) return;

        _updateFromUi = false;
        txtWeight.Value = (int)weight;
        _updateFromUi = true;
    }

    private FontStyle _selectedFontStyle = FontStyle.Normal;

    /// <summary>
    /// Get or set the selected font style in the dialog.
    /// </summary>
    public FontStyle SelectedFontStyle { get => _selectedFontStyle; set => SetAndRaise(SelectedFontStyleProperty, ref _selectedFontStyle, value); }

    /// <summary>The backing direct property for <see cref="SelectedFontStyle"/>. See the related property for details.</summary>
    public static readonly DirectProperty<FontSelectDialog, FontStyle> SelectedFontStyleProperty
        = AvaloniaProperty.RegisterDirect<FontSelectDialog, FontStyle>(nameof(SelectedFontStyle), (s) => s.SelectedFontStyle, (s, v) => s.SelectedFontStyle = v, unsetValue: FontStyle.Normal);

    void UpdateFontStyle(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_updateFromUi) return;
        if (e.NewValue is not FontStyle style) return;

        _updateFromUi = false;
        chkItalic.IsChecked = false;
        chkOverline.IsChecked = false;
        switch (style)
        {
            case FontStyle.Normal:
                break;
            case FontStyle.Italic:
                chkItalic.IsChecked = true;
                break;
            case FontStyle.Oblique:
                chkOverline.IsChecked = true;
                break;
            default:
                break;
        }
        _updateFromUi = true;
    }

    private TextDecorationCollection _selectedTextDecorations = [];

    /// <summary>
    /// Get or set the selected text decorations in the dialog.
    /// </summary>
    public TextDecorationCollection SelectedTextDecorations { get => _selectedTextDecorations; set => SetAndRaise(SelectedTextDecorationsProperty, ref _selectedTextDecorations, value); }

    /// <summary>The backing direct property for <see cref="SelectedTextDecorations"/>. See the related property for details.</summary>
    public static readonly DirectProperty<FontSelectDialog, TextDecorationCollection> SelectedTextDecorationsProperty
        = AvaloniaProperty.RegisterDirect<FontSelectDialog, TextDecorationCollection>(nameof(SelectedTextDecorations), (s) => s.SelectedTextDecorations, (s, v) => s.SelectedTextDecorations = v, unsetValue: []);

    void UpdateTextDecorations(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_updateFromUi) return;
        if (e.NewValue is not TextDecorationCollection tdc) return;

        _updateFromUi = false;

        chkBaseline.IsChecked = false;
        chkOverline.IsChecked = false;
        chkStrike.IsChecked = false;
        chkUnderline.IsChecked = false;

        if (tdc.Any(d => d.Location == TextDecorationLocation.Baseline)) chkBaseline.IsChecked = true;
        if (tdc.Any(d => d.Location == TextDecorationLocation.Overline)) chkOverline.IsChecked = true;
        if (tdc.Any(d => d.Location == TextDecorationLocation.Strikethrough)) chkStrike.IsChecked = true;
        if (tdc.Any(d => d.Location == TextDecorationLocation.Underline)) chkUnderline.IsChecked = true;

        _updateFromUi = true;
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

        if (_updateFromUi)
        {
            SelectedFontFamily = new FontFamily(txtFont.Text);
        }
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

        if (_updateFromUi)
        {
            SelectedFontSize = txtSize.Value;
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

        if (_updateFromUi)
        {
            SelectedFontWeight = (FontWeight)txtWeight.Value;
        }
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

    private void chkOverline_Checked(object sender, RoutedEventArgs e)
    {
        txtPreview.TextDecorations ??= [];
        txtPreview.TextDecorations.AddRange(TextDecorations.Overline);
        UpdateSelectedTextDecorationsFromUi();
    }

    private void chkStrike_Checked(object sender, RoutedEventArgs e)
    {
        txtPreview.TextDecorations ??= [];
        txtPreview.TextDecorations.AddRange(TextDecorations.Strikethrough);
        UpdateSelectedTextDecorationsFromUi();
    }

    private void chkUnderline_Checked(object sender, RoutedEventArgs e)
    {
        txtPreview.TextDecorations ??= [];
        txtPreview.TextDecorations.AddRange(TextDecorations.Underline);
        UpdateSelectedTextDecorationsFromUi();
    }

    private void chkBaseline_Checked(object sender, RoutedEventArgs e)
    {
        txtPreview.TextDecorations ??= [];
        txtPreview.TextDecorations.AddRange(TextDecorations.Baseline);
        UpdateSelectedTextDecorationsFromUi();
    }

    private void chkOverline_Unchecked(object sender, RoutedEventArgs e)
    {
        txtPreview.TextDecorations?.RemoveWhere(td => td.Location == TextDecorationLocation.Overline);
        UpdateSelectedTextDecorationsFromUi();
    }

    private void chkStrike_Unchecked(object sender, RoutedEventArgs e)
    {
        txtPreview.TextDecorations?.RemoveWhere(td => td.Location == TextDecorationLocation.Strikethrough);
        UpdateSelectedTextDecorationsFromUi();
    }

    private void chkUnderline_Unchecked(object sender, RoutedEventArgs e)
    {
        txtPreview.TextDecorations?.RemoveWhere(td => td.Location == TextDecorationLocation.Underline);
        UpdateSelectedTextDecorationsFromUi();
    }

    private void chkBaseline_Unchecked(object sender, RoutedEventArgs e)
    {
        txtPreview.TextDecorations?.RemoveWhere(td => td.Location == TextDecorationLocation.Baseline);
        UpdateSelectedTextDecorationsFromUi();
    }

    void UpdateSelectedTextDecorationsFromUi()
    {
        if (!_updateFromUi) return;

        if (txtPreview.TextDecorations == null)
        {
            SelectedTextDecorations = [];
        }
        else
        {
            SelectedTextDecorations = [.. txtPreview.TextDecorations];
        }
    }

    #endregion

    #region Italic / Oblique

    private void chkItalic_Checked(object sender, RoutedEventArgs e)
    {
        chkOblique.IsChecked = false;
        txtPreview.FontStyle = FontStyle.Italic;
        UpdateSelectedFontStyleFromUi();
    }

    private void chkOblique_Checked(object sender, RoutedEventArgs e)
    {
        chkItalic.IsChecked = false;
        txtPreview.FontStyle = FontStyle.Oblique;
        UpdateSelectedFontStyleFromUi();
    }

    private void chkItalic_Unchecked(object sender, RoutedEventArgs e)
    {
        txtPreview.FontStyle = chkOblique.IsChecked ? FontStyle.Oblique : FontStyle.Normal;
        UpdateSelectedFontStyleFromUi();
    }

    private void chkOblique_Unchecked(object sender, RoutedEventArgs e)
    {
        txtPreview.FontStyle = chkItalic.IsChecked ? FontStyle.Italic : FontStyle.Normal;
        UpdateSelectedFontStyleFromUi();
    }

    void UpdateSelectedFontStyleFromUi()
    {
        if (!_updateFromUi) return;

        SelectedFontStyle = txtPreview.FontStyle;

        //switch (txtPreview.FontStyle)
        //{
        //    case FontStyle.Normal:
        //        SelectedFontStyle = FontStyle.Normal;
        //        break;
        //    case FontStyle.Italic:
        //        SelectedFontStyle = FontStyle.Italic;
        //        break;
        //    case FontStyle.Oblique:
        //        SelectedFontStyle = FontStyle.Oblique;
        //        break;
        //    default:
        //        SelectedFontStyle = FontStyle.Normal;
        //        break;
        //}
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