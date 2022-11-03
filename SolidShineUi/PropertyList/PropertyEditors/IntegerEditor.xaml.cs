using System;
using System.Collections.Generic;
using System.Text;
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
            typeof(Nullable<int>), typeof(Nullable<short>), typeof(Nullable<ushort>), typeof(Nullable<byte>), typeof(Nullable<sbyte>) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        /// <inheritdoc/>
        public ColorScheme ColorScheme { set => intSpinner.ColorScheme = value; }

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
        
        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
        {
            _propType = type;

            if (value == null)
            {
                SetAsNull();
            }

            LoadUi();

            intSpinner.Value = (int)(value ?? 0);
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

        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
        {
            _propType = type;

            if (value == null)
            {
                SetAsNull();
            }

            LoadUi();

            intSpinner.Value = (int)(value ?? 0);
        }
#endif

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void intSpinner_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
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
        }
    }
}
