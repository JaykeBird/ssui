using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SolidShineUi.Utils.IconLoader;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for editing <see cref="Thickness"/> objects.
    /// </summary>
    public partial class ThicknessEditor : UserControl, IPropertyEditor
    {
        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(Thickness), typeof(Thickness?) };

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => nudLeft.IsEnabled;
            set
            {
                nudLeft.IsEnabled = value;
                nudTop.IsEnabled = value;
                nudBottom.IsEnabled = value;
                nudRight.IsEnabled = value;
                btnMenu.IsEnabled = value;
            }
        }

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        /// <summary>
        /// Set the visual appearance of this control via a ColorScheme.
        /// </summary>
        /// <param name="cs">the color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            nudLeft.ColorScheme = cs;
            nudTop.ColorScheme = cs;
            nudRight.ColorScheme = cs;
            nudBottom.ColorScheme = cs;
            btnMenu.ColorScheme = cs;

            if (cs.BackgroundColor == Colors.Black || cs.ForegroundColor == Colors.White)
            {
                imgLeft.Source = LoadIcon("LeftArrow", Utils.IconVariation.White);
                imgRight.Source = LoadIcon("RightArrow", Utils.IconVariation.White);
                imgTop.Source = LoadIcon("UpArrow", Utils.IconVariation.White);
                imgBottom.Source = LoadIcon("DownArrow", Utils.IconVariation.White);
                imgFontEdit.Source = LoadIcon("ThreeDots", Utils.IconVariation.White);
            }
            else if (cs.BackgroundColor == Colors.White)
            {
                imgLeft.Source = LoadIcon("LeftArrow", Utils.IconVariation.Black);
                imgRight.Source = LoadIcon("RightArrow", Utils.IconVariation.Black);
                imgTop.Source = LoadIcon("UpArrow", Utils.IconVariation.Black);
                imgBottom.Source = LoadIcon("DownArrow", Utils.IconVariation.Black);
                imgFontEdit.Source = LoadIcon("ThreeDots", Utils.IconVariation.Black);
            }
            else
            {
                imgLeft.Source = LoadIcon("LeftArrow", Utils.IconVariation.Color);
                imgRight.Source = LoadIcon("RightArrow", Utils.IconVariation.Color);
                imgTop.Source = LoadIcon("UpArrow", Utils.IconVariation.Color);
                imgBottom.Source = LoadIcon("DownArrow", Utils.IconVariation.Color);
                imgFontEdit.Source = LoadIcon("ThreeDots", Utils.IconVariation.Color);
            }
        }

        /// <inheritdoc/>
        public ColorScheme ColorScheme
        {
            set
            {
                ApplyColorScheme(value);
            }
        }

        /// <inheritdoc/>
        public ThicknessEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement() { return this; }
        
        /// <inheritdoc/>
#if NETCOREAPP
        public void LoadValue(object? value, Type type)
#else
        public void LoadValue(object value, Type type)
#endif
        {
            if (type == typeof(Thickness) || type == typeof(Thickness?))
            {
                if (type == typeof(Thickness?))
                {
                    mnuSetNull.IsEnabled = true;
                }

                if (value != null)
                {
                    if (value is Thickness t)
                    {
                        _internalAction = true;
                        nudLeft.Value = t.Left;
                        nudTop.Value = t.Top;
                        nudRight.Value = t.Right;
                        nudBottom.Value = t.Bottom;
                        _internalAction = false;
                    }
                    else
                    {
                        // uhhh?
                        SetAllToValue(0);
                    }
                }
                else
                {
                    // null
                    SetAsNull();
                    SetAllToValue(0);
                }
            }
            else
            {
                // uhhh?
                SetAllToValue(0);
            }
        }

        /// <inheritdoc/>
#if NETCOREAPP
        public object? GetValue()
        {
            if (mnuSetNull.IsChecked == true)
            {
                return null;
            }
            else
            {
                return new Thickness(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value);
            }
        }

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
#else
        public object GetValue()
        {
            if (mnuSetNull.IsChecked == true)
            {
                return null;
            }
            else
            {
                return new Thickness(nudLeft.Value, nudTop.Value, nudRight.Value, nudBottom.Value);
            }
        }
        
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
#endif

        bool _internalAction = false;

        private void mnuSetZero_Click(object sender, RoutedEventArgs e)
        {
            SetAllToValue(0);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void mnuSetOne_Click(object sender, RoutedEventArgs e)
        {
            SetAllToValue(1);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void nudLeft_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_internalAction)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        void SetAllToValue(double d)
        {
            _internalAction = true;
            nudLeft.Value = d;
            nudTop.Value = d;
            nudRight.Value = d;
            nudBottom.Value = d;
            UnsetAsNull();
            _internalAction = false;
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            nudLeft.IsEnabled = false;
            nudTop.IsEnabled = false;
            nudRight.IsEnabled = false;
            nudBottom.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            nudLeft.IsEnabled = true;
            nudTop.IsEnabled = true;
            nudRight.IsEnabled = true;
            nudBottom.IsEnabled = true;
        }

        private void mnuSetNull_Click(object sender, RoutedEventArgs e)
        {
            if (mnuSetNull.IsChecked)
            {
                UnsetAsNull();
            }
            else
            {
                SetAsNull();
            }
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private const double ARROW_MIN_WIDTH = 280;

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth <= ARROW_MIN_WIDTH)
            {
                nudBottom.ShowArrows = false;
                nudLeft.ShowArrows = false;
                nudRight.ShowArrows = false;
                nudTop.ShowArrows = false;
            }
            else
            {
                nudBottom.ShowArrows = true;
                nudLeft.ShowArrows = true;
                nudRight.ShowArrows = true;
                nudTop.ShowArrows = true;
            }
        }

        private void mnuAddOne_Click(object sender, RoutedEventArgs e)
        {
            nudBottom.Value += 1;
            nudLeft.Value += 1;
            nudRight.Value += 1;
            nudTop.Value += 1;
        }

        private void mnuSubtractOne_Click(object sender, RoutedEventArgs e)
        {
            if (nudBottom.Value > 0) { nudBottom.Value -= 1; }
            if (nudLeft.Value > 0) { nudLeft.Value -= 1; }
            if (nudRight.Value > 0) { nudRight.Value -= 1; }
            if (nudTop.Value > 0) { nudTop.Value -= 1; }
        }
    }
}
