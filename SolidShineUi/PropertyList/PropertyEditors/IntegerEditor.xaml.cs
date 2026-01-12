using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for <see cref="int"/>, <see cref="short"/>, and <see cref="byte"/> types.
    /// </summary>
    public partial class IntegerEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create an IntegerEditor.
        /// </summary>
        public IntegerEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] { typeof(int), typeof(short), typeof(ushort), typeof(byte), typeof(sbyte), 
            typeof(int?), typeof(short?), typeof(ushort?), typeof(byte?), typeof(sbyte?) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { /* _host = host; */ }
        
        /// <inheritdoc/>
        public void ApplySsuiTheme(SsuiTheme theme)
        {
            intSpinner.SsuiTheme = theme;
            btnMenu.SsuiTheme = theme;
            imgMenu.Source = Utils.IconLoader.LoadIcon("ThreeDots", theme.IconVariation);
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => intSpinner.IsEnabled;
            set => intSpinner.IsEnabled = value;
        }

        Type _propType = typeof(int);

#if NETCOREAPP
        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
        
        /// <inheritdoc/>
        public object? GetValue()
        {
            if (_propType == typeof(int))
            {
                return intSpinner.Value;
            }
            else if (_propType == typeof(short))
            {
                return (short)intSpinner.Value;
            }
            else if (_propType == typeof(ushort))
            {
                return (ushort)intSpinner.Value;
            }
            else if (_propType == typeof(byte))
            {
                return (byte)intSpinner.Value;
            }
            else if (_propType == typeof(sbyte))
            {
                return (sbyte)intSpinner.Value;
            }
            else if (_propType == typeof(int?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return intSpinner.Value;
                }
            }
            else if (_propType == typeof(short?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return (short)intSpinner.Value;
                }
            }
            else if (_propType == typeof(ushort?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return (ushort)intSpinner.Value;
                }
            }
            else if (_propType == typeof(byte?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return (byte)intSpinner.Value;
                }
            }
            else if (_propType == typeof(sbyte?))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return (sbyte)intSpinner.Value;
                }
            }
            else
            {
                return intSpinner.Value;
            }
        }
#else
        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
        {
            if (_propType == typeof(int))
            {
                return intSpinner.Value;
            }
            else if (_propType == typeof(short))
            {
                return (short)intSpinner.Value;
            }
            else if (_propType == typeof(ushort))
            {
                return (ushort)intSpinner.Value;
            }
            else if (_propType == typeof(byte))
            {
                return (byte)intSpinner.Value;
            }
            else if (_propType == typeof(sbyte))
            {
                return (sbyte)intSpinner.Value;
            }
            else if (_propType == typeof(int?) || _propType == typeof(Nullable<int>))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return intSpinner.Value;
                }
            }
            else if (_propType == typeof(short?) || _propType == typeof(Nullable<short>))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return (short)intSpinner.Value;
                }
            }
            else if (_propType == typeof(ushort?) || _propType == typeof(Nullable<ushort>))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return (ushort)intSpinner.Value;
                }
            }
            else if (_propType == typeof(byte?) || _propType == typeof(Nullable<byte>))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return (byte)intSpinner.Value;
                }
            }
            else if (_propType == typeof(sbyte?) || _propType == typeof(Nullable<sbyte>))
            {
                if (mnuSetNull.IsChecked)
                {
                    return null;
                }
                else
                {
                    return (sbyte)intSpinner.Value;
                }
            }
            else
            {
                return intSpinner.Value;
            }
        }
#endif

#if NETCOREAPP
        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
#else
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
#endif
        {
            _propType = type;

            if (value == null)
            {
                SetAsNull();
            }

            LoadUi();

            _internalAction = true;
            intSpinner.Value = (int)(value ?? 0);
            _internalAction = false;
        }

        void LoadUi()
        {
            if (_propType == typeof(int))
            {
                SetMaxMin(int.MaxValue, int.MinValue);
            }
            else if (_propType == typeof(short))
            {
                SetMaxMin(short.MaxValue, short.MinValue);
            }
            else if (_propType == typeof(ushort))
            {
                SetMaxMin(ushort.MaxValue, ushort.MinValue);
            }
            else if (_propType == typeof(byte))
            {
                SetMaxMin(byte.MaxValue, byte.MinValue);
            }
            else if (_propType == typeof(sbyte))
            {
                SetMaxMin(sbyte.MaxValue, sbyte.MinValue);
            }
            else if (_propType == typeof(int?) || _propType == typeof(Nullable<int>))
            {
                SetMaxMin(int.MaxValue, int.MinValue);
                mnuSetNull.IsEnabled = true;
            }
            else if (_propType == typeof(short?) || _propType == typeof(Nullable<short>))
            {
                SetMaxMin(short.MaxValue, short.MinValue);
                mnuSetNull.IsEnabled = true;
            }
            else if (_propType == typeof(ushort?) || _propType == typeof(Nullable<ushort>))
            {
                SetMaxMin(ushort.MaxValue, ushort.MinValue);
                mnuSetNull.IsEnabled = true;
            }
            else if (_propType == typeof(byte?) || _propType == typeof(Nullable<byte>))
            {
                SetMaxMin(byte.MaxValue, byte.MinValue);
                mnuSetNull.IsEnabled = true;
            }
            else if (_propType == typeof(sbyte?) || _propType == typeof(Nullable<sbyte>))
            {
                SetMaxMin(sbyte.MaxValue, sbyte.MinValue);
                mnuSetNull.IsEnabled = true;
            }

            void SetMaxMin(int max, int min)
            {
                intSpinner.MaxValue = max;
                intSpinner.MinValue = min;
            }
        }

        bool _internalAction = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void intSpinner_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_internalAction)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        void SetAsNull()
        {
            mnuSetNull.IsEnabled = true;
            mnuSetNull.IsChecked = true;
            intSpinner.IsEnabled = false;
        }

        void UnsetAsNull()
        {
            mnuSetNull.IsChecked = false;
            intSpinner.IsEnabled = true;
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

        private void mnuDisplayHex_Click(object sender, RoutedEventArgs e)
        {
            if (intSpinner.DisplayAsHex)
            {
                intSpinner.DisplayAsHex = false;
                mnuDisplayHex.IsChecked = false;
            }
            else
            {
                intSpinner.DisplayAsHex = true;
                mnuDisplayHex.IsChecked = true;
            }
        }
    }
}
