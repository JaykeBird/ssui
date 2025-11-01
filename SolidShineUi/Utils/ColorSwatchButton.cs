using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
        public ColorSwatchButton() : this(Colors.White)
        {
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
            SsuiThemeApplied += ColorSwatchButton_SsuiThemeApplied;

            UseLayoutRounding = true;
        }

        private void ColorSwatchButton_SsuiThemeApplied(object sender, RoutedEventArgs e)
        {
            // override SsuiTheme's background brushes with our checkerboard pattern ones
            UpdateBrushes();
        }

        private void ColorSwatchButton_ColorSchemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateBrushes();
            //UpdateColor();
        }

        /// <summary>
        /// Get or set the color shown in this button.
        /// </summary>
        public Color Color { get => (Color)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

        /// <summary>The backing dependency property for <see cref="Color"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ColorProperty
            = DependencyProperty.Register("Color", typeof(Color), typeof(ColorSwatchButton),
            new FrameworkPropertyMetadata(Colors.White, new PropertyChangedCallback((d, e) => d.PerformAs<ColorSwatchButton>((b) => b.UpdateColor()))));

        /// <summary>
        /// Get or set if a hexadecimal value (i.e. #FF0033) is shown on the tooltip for this button, even if the color exists in <see cref="Colors"/>.
        /// This property does nothing if <see cref="SetToolTip"/> is set to false.
        /// </summary>
        /// <seealso cref="SetToolTip"/>
        /// <seealso cref="FrameworkElement.ToolTip"/>
        public bool AlwaysHexTooltips { get => (bool)GetValue(AlwaysHexTooltipsProperty); set => SetValue(AlwaysHexTooltipsProperty, value); }

        /// <summary>The backing dependency property for <see cref="AlwaysHexTooltips"/>. See the related property for details.</summary>
        public static readonly DependencyProperty AlwaysHexTooltipsProperty
            = DependencyProperty.Register("AlwaysHexTooltips", typeof(bool), typeof(ColorSwatchButton),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => d.PerformAs<ColorSwatchButton>((b) => b.UpdateColor()))));

        /// <summary>
        /// Get or set if this button's ToolTip should be changed when <see cref="Color"/> is changed.
        /// </summary>
        public bool SetToolTip { get => (bool)GetValue(SetToolTipProperty); set => SetValue(SetToolTipProperty, value); }

        /// <summary>The backing dependency property for <see cref="SetToolTip"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SetToolTipProperty
            = DependencyProperty.Register("SetToolTip", typeof(bool), typeof(ColorSwatchButton),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => d.PerformAs<ColorSwatchButton>((b) => b.UpdateColor()))));

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
            var b = new System.Windows.Controls.Border();
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
                    ToolTip = "#" + Color.GetHexStringWithAlpha();
                }
                else
                {
                    string name = ColorsHelper.GetX11NameFromColor(Color);
                    if (string.IsNullOrEmpty(name))
                    {
                        ToolTip = "#" + Color.GetHexStringWithAlpha();
                    }
                    else
                    {
                        ToolTip = name;
                    }
                }
            }
        }
    }
}
