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
        public ColorSwatchButton()
        {
            Color = Colors.White;
            ColorSchemeChanged += ColorSwatchButton_ColorSchemeChanged;
        }

        /// <summary>
        /// Create a ColorSwatchButton, with the color preset.
        /// </summary>
        /// <param name="c">The color to display in this button.</param>
        public ColorSwatchButton(Color c)
        {
            Color = c;
            ColorSchemeChanged += ColorSwatchButton_ColorSchemeChanged;
        }

        private void ColorSwatchButton_ColorSchemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateColor();
        }

        /// <summary>
        /// Get or set the color shown in this button.
        /// </summary>
        public Color Color { get => (Color)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

        /// <summary>The backing dependency property for <see cref="Color"/>. See the related property for details.</summary>
        public static DependencyProperty ColorProperty
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
        public static DependencyProperty AlwaysHexTooltipsProperty
            = DependencyProperty.Register("AlwaysHexTooltips", typeof(bool), typeof(ColorSwatchButton),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => d.PerformAs<ColorSwatchButton>((b) => b.UpdateColor()))));

        /// <summary>
        /// Get or set if this button's ToolTip should be changed when <see cref="Color"/> is changed.
        /// </summary>
        public bool SetToolTip { get => (bool)GetValue(SetToolTipProperty); set => SetValue(SetToolTipProperty, value); }

        /// <summary>The backing dependency property for <see cref="SetToolTip"/>. See the related property for details.</summary>
        public static DependencyProperty SetToolTipProperty
            = DependencyProperty.Register("SetToolTip", typeof(bool), typeof(ColorSwatchButton),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => d.PerformAs<ColorSwatchButton>((b) => b.UpdateColor()))));


        void UpdateColor()
        {
            Background = Color.ToBrush();
            HighlightBrush = Color.ToBrush();
            ClickBrush = Color.ToBrush();
            SelectedBrush = Color.ToBrush();
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
