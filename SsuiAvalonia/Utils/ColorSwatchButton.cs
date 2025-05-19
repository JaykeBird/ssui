using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A simple button that displays a color, for usage in locations where users can select from a list of colors (such as <see cref="ColorPickerDialog"/>).
    /// </summary>
    public class ColorSwatchButton : FlatButton
    {

        /// <summary>
        /// Create a ColorSwatchButton.
        /// </summary>
        public ColorSwatchButton()
        {
            Color = Colors.White;
            UpdateBrushes();
            ColorSchemeChanged += ColorSwatchButton_ColorSchemeChanged;
        }

        /// <summary>
        /// Create a ColorSwatchButton, with the color preset.
        /// </summary>
        /// <param name="c">The color to display in this button.</param>
        public ColorSwatchButton(Color c)
        {
            Color = c;
            UpdateBrushes();
            ColorSchemeChanged += ColorSwatchButton_ColorSchemeChanged;
        }

        private void ColorSwatchButton_ColorSchemeChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            UpdateBrushes();
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(Color):
                case nameof(AlwaysHexTooltips):
                case nameof(SetToolTip):
                    UpdateColor();
                    break;
            }
        }

        /// <summary>
        /// Get or set the color shown in this button.
        /// </summary>
        public Color Color { get => GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

        /// <summary>The backing styled property for <see cref="Color"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Color> ColorProperty
            = AvaloniaProperty.Register<ColorSwatchButton, Color>(nameof(Color), Colors.White);

        /// <summary>
        /// Get or set if a hexadecimal value (i.e. #FF0033) is shown on the tooltip for this button, even if the color exists in <see cref="Colors"/>.
        /// This property does nothing if <see cref="SetToolTip"/> is set to false.
        /// </summary>
        /// <seealso cref="SetToolTip"/>
        public bool AlwaysHexTooltips { get => GetValue(AlwaysHexTooltipsProperty); set => SetValue(AlwaysHexTooltipsProperty, value); }

        /// <summary>The backing styled property for <see cref="AlwaysHexTooltips"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> AlwaysHexTooltipsProperty
            = AvaloniaProperty.Register<ColorSwatchButton, bool>(nameof(AlwaysHexTooltips), true);

        /// <summary>
        /// Get or set if this button's ToolTip should be changed when <see cref="Color"/> is changed.
        /// </summary>
        public bool SetToolTip { get => GetValue(SetToolTipProperty); set => SetValue(SetToolTipProperty, value); }

        /// <summary>The backing styled property for <see cref="SetToolTip"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> SetToolTipProperty
            = AvaloniaProperty.Register<ColorSwatchButton, bool>(nameof(SetToolTip), true);

        #region Button Brushes

        static Brush mainBrush = BrushFactory.CreateCheckerboardBrush(4, Colors.White.ToBrush(), Colors.LightGray.ToBrush());
        static Brush highBrush = BrushFactory.CreateCheckerboardBrush(4, Colors.White.ToBrush(), Colors.Gainsboro.ToBrush());
        static Brush clikBrush = BrushFactory.CreateCheckerboardBrush(4, Colors.White.ToBrush(), Colors.Gray.ToBrush());
        static Brush seleBrush = BrushFactory.CreateCheckerboardBrush(4, Colors.White.ToBrush(), Colors.LightGray.ToBrush());

        void UpdateBrushes()
        {
            Background = mainBrush;
            HighlightBrush = highBrush;
            ClickBrush = clikBrush;
            SelectedBrush = seleBrush;
        }

        #endregion

        void UpdateColor()
        {
            var b = new Border();
            b.Background = Color.ToBrush();
            b.HorizontalAlignment = HorizontalAlignment.Stretch;
            b.VerticalAlignment = VerticalAlignment.Stretch;
            Content = b;
            Padding = new Thickness(0);
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            if (SetToolTip)
            {
                if (AlwaysHexTooltips)
                {
                    ToolTip.SetTip(this, "#" + Color.GetHexStringWithAlpha());
                }
                else
                {
                    string name = ColorsHelper.GetX11NameFromColor(Color);
                    if (string.IsNullOrEmpty(name))
                    {
                        ToolTip.SetTip(this, "#" + Color.GetHexStringWithAlpha());
                    }
                    else
                    {
                        ToolTip.SetTip(this, name);
                    }
                }
            }
        }

    }
}
