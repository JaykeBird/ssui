using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// Interaction logic for EnumEditor.xaml
    /// </summary>
    public partial class EnumEditor : UserControl, IPropertyEditor
    {
        public EnumEditor()
        {
            InitializeComponent();
        }

        public List<Type> ValidTypes => (new[] {typeof(Enum)}).ToList();

        public bool EditorAllowsModifying => true;

        public ExperimentalPropertyList ParentPropertyList { set { } }

        public ColorScheme ColorScheme { set { } }

        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }
        public bool IsPropertyWritable
        {
            get => cbbEnums.IsEnabled;
            set => cbbEnums.IsEnabled = value;
        }

#if NETCOREAPP
        public event EventHandler? ValueChanged;

        public object? GetValue()
        {
            if (cbbEnums.SelectedIndex == -1)
            {
                return null;
            }
            else
            {
                return cbbEnums.SelectedEnumValue;
            }
        }

        public void LoadValue(object? value, Type type)
        {
            cbbEnums.Enum = type;
            if (value == null)
            {
                cbbEnums.SelectedIndex = -1;
            }
            else
            {
                cbbEnums.SelectedEnumValue = value;
            }
        }
#else
        public event EventHandler ValueChanged;

        public object GetValue()
        {
            if (cbbEnums.SelectedIndex == -1)
            {
                return null;
            }
            else
            {
                return cbbEnums.SelectedEnumValue;
            }
        }

        public void LoadValue(object value, Type type)
        {
            cbbEnums.Enum = type;
            if (value == null)
            {
                cbbEnums.SelectedIndex = -1;
            }
            else
            {
                cbbEnums.SelectedEnumValue = value;
            }
        }
#endif

        private void cbbEnums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}
