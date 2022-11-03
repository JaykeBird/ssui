using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media;

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
        public List<Type> ValidTypes => (new[] {typeof(float), typeof(double), typeof(Nullable<double>), typeof(Nullable<float>)}).ToList();
#endif

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        /// <inheritdoc/>
        public ColorScheme ColorScheme { 
            set
            {
                dblSpinner.ColorScheme = value;
                btnMenu.ColorScheme = value;
                if (value.BackgroundColor == Colors.Black || value.ForegroundColor == Colors.White)
                {
                    imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsWhite.png", UriKind.Relative));
                }
                else if (value.BackgroundColor == Colors.White)
                {
                    imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsBlack.png", UriKind.Relative));
                }
                else
                {
                    imgMenu.Source = new BitmapImage(new Uri("/SolidShineUi;component/Images/ThreeDotsColor.png", UriKind.Relative));
                }
            } 
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
            set => dblSpinner.IsEnabled = value;
        }

        Type _propType = typeof(double);

        /// <inheritdoc/>
#if NETCOREAPP
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
        {
            if (_propType == typeof(double))
            {
                return dblSpinner.Value;
            }
            else if (_propType == typeof(float))
            {
                return (float)dblSpinner.Value;
            }
#if NET5_0_OR_GREATER
            else if (_propType == typeof(Half))
            {
                return (Half)dblSpinner.Value;
            }
            else if (_propType == typeof(double?))
            {
                return (mnuSetNull.IsChecked ? null : dblSpinner.Value);
            }
            else if (_propType == typeof(float?))
            {
                return (mnuSetNull.IsChecked ? null : (float)dblSpinner.Value);
            }
            else if (_propType == typeof(Half?))
            {
                return (mnuSetNull.IsChecked ? null : (Half)dblSpinner.Value);
            }
#else
            else if (_propType == typeof(Nullable<double>))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return dblSpinner.Value;
                }
            }
            else if (_propType == typeof(Nullable<float>))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
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
            if (type == typeof(Nullable<double>) || type == typeof(Nullable<float>))
#endif
            {
                mnuSetNull.IsEnabled = true;
                if (value == null)
                {
                    SetAsNull();
                }
            }

            _propType = type;
            dblSpinner.Value = (double)(value ?? 0); // TODO: properly handle null
        }
#else
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
        {
            if (_propType == typeof(double))
            {
                return dblSpinner.Value;
            }
            else if (_propType == typeof(float))
            {
                return (float)dblSpinner.Value;
            }
            else
            {
                return dblSpinner.Value;
            }
        }

        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
            _propType = type;
            dblSpinner.Value = (double)(value ?? 0); // TODO: properly handle null
        }
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void dblSpinner_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            dblSpinner.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            dblSpinner.IsEnabled = true;
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
        }
    }
}
