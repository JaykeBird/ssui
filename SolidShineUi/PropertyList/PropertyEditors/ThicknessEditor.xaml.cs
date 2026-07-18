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
        public bool IsPropertyWritable
        { 
            get => btnMenu.IsEnabled;
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
        public void SetHostControl(IPropertyEditorHost host) { /* _host = host; */ }

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            nudLeft.SsuiTheme = theme;
            nudTop.SsuiTheme = theme;
            nudRight.SsuiTheme = theme;
            nudBottom.SsuiTheme = theme;
            btnMenu.SsuiTheme = theme;

            imgLeft.Source = LoadIcon("LeftArrow", theme.IconVariation);
            imgRight.Source = LoadIcon("RightArrow", theme.IconVariation);
            imgTop.Source = LoadIcon("UpArrow", theme.IconVariation);
            imgBottom.Source = LoadIcon("DownArrow", theme.IconVariation);
            imgFontEdit.Source = LoadIcon("ThreeDots", theme.IconVariation);
        }

        /// <inheritdoc/>
        public ThicknessEditor()
        {
            InitializeComponent();

            // load in string values
            nudLeft.ToolTip = Strings.Left;
            nudTop.ToolTip = Strings.Top;
            nudRight.ToolTip = Strings.Right;
            nudBottom.ToolTip = Strings.Bottom;

            mnuAddOne.Header = Strings.IncreaseAllByOne;
            mnuSubtractOne.Header = Strings.DecreaseAllByOne;
            mnuSetOne.Header = Strings.SetAllToOne;
            mnuSetZero.Header = Strings.SetAllToZero;
            mnuSetNull.Header = Strings.SetAsNull;
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
