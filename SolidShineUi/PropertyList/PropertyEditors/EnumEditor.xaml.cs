using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Reflection;

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
        public List<Type> ValidTypes => (new[] { typeof(Enum) }).ToList();

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        /// <inheritdoc/>
        public void SetHostControl(IPropertyEditorHost host) { /* _host = host; */ }

        /// <inheritdoc/>
        public ColorScheme ColorScheme { set { ApplyColorScheme(value); } }

        /// <inheritdoc/>
        public void ApplyColorScheme(ColorScheme cs) { }

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
#else
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public object GetValue()
#endif
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

#if NETCOREAPP
        /// <inheritdoc/>
        public void LoadValue(object? value, Type type)
#else
        /// <inheritdoc/>
        public void LoadValue(object value, Type type)
#endif
        {
            cbbEnums.Enum = type;
            if (type.GetCustomAttribute<FlagsAttribute>() != null)
            {
                // this is an enum that supports flags
                // in the future, I'll need to enable a way to select multiple items
            }

            if (value == null)
            {
                cbbEnums.SelectedIndex = -1;
            }
            else
            {
                cbbEnums.SelectedEnumValue = value;
            }
        }

        private void cbbEnums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}
