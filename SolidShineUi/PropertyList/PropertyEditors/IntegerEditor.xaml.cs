using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for IntegerEditor.xaml
    /// </summary>
    public partial class IntegerEditor : UserControl, IPropertyEditor
    {
        public IntegerEditor()
        {
            InitializeComponent();
        }
        public List<Type> ValidTypes => (new[] { typeof(int), typeof(short), typeof(ushort), typeof(byte), typeof(sbyte) }).ToList();

        public bool EditorAllowsModifying => true;

        public ColorScheme ColorScheme { set => intSpinner.ColorScheme = value; }

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }
        public bool IsPropertyReadOnly
        {
            get => intSpinner.IsEnabled;
            set => intSpinner.IsEnabled = value;
        }

        Type _propType = typeof(int);

#if NETCOREAPP
        public event EventHandler? ValueChanged;

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
            else
            {
                return intSpinner.Value;
            }
        }

        public void LoadValue(object? value, Type type)
        {
            _propType = type;
            intSpinner.Value = (int)(value ?? 0); // TODO: properly handle null
        }
#else
        public event EventHandler ValueChanged;

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
            else
            {
                return intSpinner.Value;
            }
        }

        public void LoadValue(object value, Type type)
        {
            _propType = type;
            intSpinner.Value = (int)(value ?? 0); // TODO: properly handle null
        }
#endif

        private void intSpinner_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
