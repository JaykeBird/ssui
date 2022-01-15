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

        public List<Type> ValidTypes => (new[] {typeof(float), typeof(double)}).ToList();

        public bool CanEdit => true;

        public ColorScheme ColorScheme { set => dblSpinner.ColorScheme = value; }

        public UIElement GetUiElement()
        {
            return this;
        }

        Type _propType = typeof(double);

#if NETCOREAPP
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
    }
}
