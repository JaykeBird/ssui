using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace SolidShineUi.Utils;

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

    private IFormatProvider invar = System.Globalization.CultureInfo.InvariantCulture;

    /// <summary>
    /// Get or set the color to display in the ColorListItem.
    /// </summary>
    public Color Color { get => GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

    /// <summary>The backing styled property for <see cref="Color"/>. See the related property for details.</summary>
    public static readonly StyledProperty<Color> ColorProperty
        = AvaloniaProperty.Register<ColorListItem, Color>(nameof(Color), Colors.White);

    void OnColorChange(AvaloniaPropertyChangedEventArgs e)
    {
        brdrColor.Background = Color.ToBrush();
        UpdateColorDisplay();
    }

    /// <summary>
    /// Get or set the text to display with the color, such as RGB values or HEX values.
    /// </summary>
    public ColorListItemDisplay DisplayMode { get => GetValue(DisplayModeProperty); set => SetValue(DisplayModeProperty, value); }

    /// <summary>The backing styled property for <see cref="DisplayMode"/>. See the related property for details.</summary>
    public static readonly StyledProperty<ColorListItemDisplay> DisplayModeProperty
        = AvaloniaProperty.Register<ColorListItem, ColorListItemDisplay>(nameof(DisplayMode), ColorListItemDisplay.Hex);

    void OnDisplayModeChange(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateColorDisplay();
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        switch (change.Property.Name)
        {
            case nameof(Color):
                OnColorChange(change);
                break;
            case nameof(DisplayMode):
                OnDisplayModeChange(change);
                break;
        }
    }

    void UpdateColorDisplay()
    {
        switch (DisplayMode)
        {
            case ColorListItemDisplay.Hex:
                lblColor.Text = "#" + Color.GetHexStringWithAlpha();
                break;
            case ColorListItemDisplay.Rgb:
                lblColor.Text = Color.A.ToString(invar) + ", " + Color.R.ToString(invar) + ", " + Color.G.ToString(invar) + ", " + Color.B.ToString(invar);
                break;
            case ColorListItemDisplay.Hsv:
                ColorsHelper.ToHSV(Color, out double h, out double s, out double v);
                lblColor.Text = h.ToString("N0", invar) + ", " + s.ToString("N3", invar) + ", " + v.ToString("N3", invar);
                break;
            default:
                lblColor.Text = "#" + Color.GetHexStringWithAlpha();
                break;
        }
    }
}