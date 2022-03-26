using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for DoubleEditor.xaml
    /// </summary>
    public partial class DoubleEditor : UserControl, IPropertyEditor
    {
        public DoubleEditor()
        {
            InitializeComponent();
        }

#if NET5_0_OR_GREATER
        public List<Type> ValidTypes => (new[] { typeof(float), typeof(double), typeof(Half) }).ToList();
#else
        public List<Type> ValidTypes => (new[] {typeof(float), typeof(double)}).ToList();
#endif

        public bool EditorAllowsModifying => true;

        public ColorScheme ColorScheme { set => dblSpinner.ColorScheme = value; }

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }
        public bool IsPropertyWritable
        {
            get => dblSpinner.IsEnabled;
            set => dblSpinner.IsEnabled = value;
        }

        Type _propType = typeof(double);

#if NETCOREAPP
        public event EventHandler? ValueChanged;

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
#endif
            else
            {
                return dblSpinner.Value;
            }
        }

        public void LoadValue(object? value, Type type)
        {
            _propType = type;
            dblSpinner.Value = (double)(value ?? 0); // TODO: properly handle null
        }
#else
        public event EventHandler ValueChanged;

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
    }
}
