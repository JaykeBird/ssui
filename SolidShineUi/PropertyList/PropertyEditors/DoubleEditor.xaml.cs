using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for <see cref="double"/> and <see cref="float"/> objects.
    /// </summary>
    public partial class DoubleEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create a new DoubleEditor.
        /// </summary>
        public DoubleEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
#if NET5_0_OR_GREATER
        public List<Type> ValidTypes => (new[] { typeof(float), typeof(double), typeof(Half), typeof(double?), typeof(float?), typeof(Half?) }).ToList();
#else
        public List<Type> ValidTypes => (new[] { typeof(float), typeof(double), typeof(double?), typeof(float?) }).ToList();
#endif

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { /* _host = host; */ }

        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            dblSpinner.SsuiTheme = theme;
            btnMenu.SsuiTheme = theme;
            imgMenu.Source = IconLoader.LoadIcon("ThreeDots", theme.IconVariation);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => dblSpinner.IsEnabled;
            set { dblSpinner.IsEnabled = value; mnuSetNan.IsEnabled = value; }
        }

        Type _propType = typeof(double);
        bool _internalAction = false;

        /// <inheritdoc/>
#if NETCOREAPP
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
        {
            if (_propType == typeof(double))
            {
                return mnuSetNan.IsChecked ? double.NaN : dblSpinner.Value;
            }
            else if (_propType == typeof(float))
            {
                return mnuSetNan.IsChecked ? float.NaN : (float)dblSpinner.Value;
            }
#if NET5_0_OR_GREATER
            else if (_propType == typeof(Half))
            {
                return mnuSetNan.IsChecked ? double.NaN : (Half)dblSpinner.Value;
            }
            else if (_propType == typeof(double?))
            {
                return mnuSetNull.IsChecked ? null : (mnuSetNan.IsChecked ? double.NaN : dblSpinner.Value);
            }
            else if (_propType == typeof(float?))
            {
                return mnuSetNull.IsChecked ? null : (mnuSetNan.IsChecked ? float.NaN : (float)dblSpinner.Value);
            }
            else if (_propType == typeof(Half?))
            {
                return mnuSetNull.IsChecked ? null : (mnuSetNan.IsChecked ? double.NaN : (Half)dblSpinner.Value);
            }
#else
            else if (_propType == typeof(double?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else if (mnuSetNan.IsChecked)
                {
                    return double.NaN;
                }
                else
                {
                    return dblSpinner.Value;
                }
            }
            else if (_propType == typeof(float?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else if (mnuSetNan.IsChecked)
                {
                    return float.NaN;
                }
                else
                {
                    return (float)dblSpinner.Value;
                }
            }
#endif
            else
            {
                return dblSpinner.Value;
            }
        }

        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
#if NET5_0_OR_GREATER
            if (type == typeof(double?) || type == typeof(float?) || type == typeof(Half?))
#else
            if (type == typeof(double?) || type == typeof(float?))
#endif
            {
                mnuSetNull.IsEnabled = true;
                if (value == null)
                {
                    SetAsNull();
                }
                else if (value is double d && double.IsNaN(d))
                {
                    SetAsNaN();
                }
                else if (value is float f && float.IsNaN(f))
                {
                    SetAsNaN();
                }
#if NET5_0_OR_GREATER
                else if (value is Half h && Half.IsNaN(h))
                {
                    SetAsNaN();
                }
#endif
                else
                {
                    //UnsetAsNaN();
                    UnsetAsNull();
                }
            }
            else if (value is double d && double.IsNaN(d))
            {
                SetAsNaN();
            }
            else if (value is float f && float.IsNaN(f))
            {
                SetAsNaN();
            }
#if NET5_0_OR_GREATER
            else if (value is Half h && Half.IsNaN(h))
            {
                SetAsNaN();
            }
#endif

            _propType = type;
            _internalAction = true;
            dblSpinner.Value = (double)(value ?? 0);
            _internalAction = false;
        }
#else
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
        {
            if (_propType == typeof(double))
            {
                if (mnuSetNan.IsChecked)
                {
                    return double.NaN;
                }
                else
                {
                    return dblSpinner.Value;
                }
            }
            else if (_propType == typeof(float))
            {
                if (mnuSetNan.IsChecked)
                {
                    return float.NaN;
                }
                else
                {
                    return (float)dblSpinner.Value;
                }
            }
            else if (_propType == typeof(double?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else if (mnuSetNan.IsChecked)
                {
                    return double.NaN;
                }
                else
                {
                    return dblSpinner.Value;
                }
            }
            else if (_propType == typeof(float?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else if (mnuSetNan.IsChecked)
                {
                    return float.NaN;
                }
                else
                {
                    return (float)dblSpinner.Value;
                }
            }
            else
            {
                return dblSpinner.Value;
            }
        }

        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
            if (type == typeof(double?) || type == typeof(float?))
            {
                mnuSetNull.IsEnabled = true;
                if (value == null)
                {
                    SetAsNull();
                }
                else if (value is double d && double.IsNaN(d))
                {
                    SetAsNaN();
                }
                else if (value is float f && float.IsNaN(f))
                {
                    SetAsNaN();
                }
                else
                {
                    //UnsetAsNaN();
                    UnsetAsNull();
                }
            }
            else if (value is double d && double.IsNaN(d))
            {
                SetAsNaN();
            }
            else if (value is float f && float.IsNaN(f))
            {
                SetAsNaN();
            }

            _propType = type;
            _internalAction = true;
            dblSpinner.Value = (double)(value ?? 0);
            _internalAction = false;
        }
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void dblSpinner_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            mnuSetNan.IsChecked = false;
            dblSpinner.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            dblSpinner.IsEnabled = true;
        }

        void SetAsNaN()
        {
            mnuSetNan.IsChecked = true;
            mnuSetNull.IsChecked = false;
            _internalAction = true;
            if (!double.IsNaN(dblSpinner.Value)) dblSpinner.Value = double.NaN;
            _internalAction = false;
            //dblSpinner.IsEnabled = false;
        }

        void UnsetAsNaN()
        {
            mnuSetNan.IsChecked = false;
            _internalAction = true;
            if (double.IsNaN(dblSpinner.Value)) dblSpinner.Value = 0;
            _internalAction = false;
            //dblSpinner.IsEnabled = true;
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

        private void mnuSetNan_Click(object sender, RoutedEventArgs e)
        {
            if (mnuSetNan.IsChecked)
            {
                UnsetAsNaN();
            }
            else
            {
                SetAsNaN();
            }
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
