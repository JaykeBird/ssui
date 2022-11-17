using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// A property editor for editing <see cref="Enum"/> objects.
    /// </summary>
    public partial class EnumEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create an EnumEditor.
        /// </summary>
        public EnumEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => (new[] {typeof(Enum)}).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { } }

        /// <inheritdoc/>
        public ColorScheme ColorScheme { set { } }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool IsPropertyWritable
        {
            get => cbbEnums.IsEnabled;
            set => cbbEnums.IsEnabled = value;
        }

        /// <inheritdoc/>
#if NETCOREAPP
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
        
        /// <inheritdoc/>
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
        
        /// <inheritdoc/>
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
