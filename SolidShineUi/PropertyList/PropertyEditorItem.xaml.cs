using System;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// Interaction logic for PropertyEditorItem.xaml
    /// </summary>
    public partial class PropertyEditorItem : UserControl
    {
        public PropertyEditorItem()
        {
            InitializeComponent();
        }

#if NETCOREAPP
        public PropertyInfo? PropertyInfo { get; set; } = null;

        object? _value = null;
        public object? PropertyValue { get => _value; set { _value = value; txtValue.Text = (value ?? "(null)").ToString(); } }

        public IPropertyEditor? PropertyEditorControl { get; set; }

        public delegate void PropertyEditorValueChangedEventHandler(object? sender, PropertyEditorValueChangedEventArgs e);

        public event PropertyEditorValueChangedEventHandler? PropertyEditorValueChanged;
#else
        public PropertyInfo PropertyInfo { get; set; } = null;

        object _value = null;
        public object PropertyValue { get => _value; set { _value = value; txtValue.Text = (value ?? "(null)").ToString(); } }

        public IPropertyEditor PropertyEditorControl { get; set; }
        
        public delegate void PropertyEditorValueChangedEventHandler(object sender, PropertyEditorValueChangedEventArgs e);

        public event PropertyEditorValueChangedEventHandler PropertyEditorValueChanged;
#endif

        public string PropertyName { get => txtName.Text; set => txtName.Text = value; }
        public string PropertyType { get => txtType.Text; set => txtType.Text = value; }

#if NETCOREAPP
        public void LoadProperty(PropertyInfo property, object? value, IPropertyEditor? editor)
#else
        public void LoadProperty(PropertyInfo property, object value, IPropertyEditor editor)
#endif
        {
            PropertyInfo = property;
            PropertyName = property.Name;
            PropertyType = property.PropertyType.ToString();
            PropertyValue = value;
            if (editor != null)
            {
                PropertyEditorControl = editor;
                editor.LoadValue(value, property.PropertyType);
                txtValue.Visibility = Visibility.Collapsed;
                FrameworkElement uie = PropertyEditorControl.GetFrameworkElement();
                PropertyEditorControl.ValueChanged += PropertyEditorControl_ValueChanged;
                grdValue.Children.Add(uie);
            }
        }

#if NETCOREAPP
        private void PropertyEditorControl_ValueChanged(object? sender, EventArgs e)
#else
        private void PropertyEditorControl_ValueChanged(object sender, EventArgs e)
#endif
        {
            if (PropertyEditorControl != null)
            {
                PropertyValue = PropertyEditorControl.GetValue();
                var ev = new PropertyEditorValueChangedEventArgs(PropertyValue, PropertyName, PropertyInfo);
                PropertyEditorValueChanged?.Invoke(this, ev);

                if (ev.ChangeFailed)
                {
                    PropertyValue = ev.FailedChangePropertyValue;

                    if (PropertyInfo != null)
                    {
                        PropertyEditorControl.LoadValue(PropertyValue, PropertyInfo.PropertyType);
                    }
                }
            }
        }

        public void ApplyPropertyValue(object targetObject)
        {
            if (targetObject == null) throw new ArgumentNullException(nameof(targetObject), "Targetted object cannot be null");
            if (PropertyInfo == null) throw new InvalidOperationException("The property to set on the object hasn't been defined. Please set the " + nameof(PropertyInfo) + " property.");

            PropertyInfo.SetValue(targetObject, PropertyValue);
        }
    }

#if NETCOREAPP
    public class PropertyEditorValueChangedEventArgs
    {
        public object? NewValue { get; private set; }
        public string PropertyName { get; private set; }
        public PropertyInfo? PropertyInfo { get; private set; }

        public bool ChangeFailed { get; set; } = false;
        public object? FailedChangePropertyValue { get; set; } = null;

        public PropertyEditorValueChangedEventArgs(object? newValue, string propertyName, PropertyInfo? propertyInfo)
        {
            NewValue = newValue;
            PropertyName = propertyName;
            PropertyInfo = propertyInfo;
        }
    }
#else
    public class PropertyEditorValueChangedEventArgs
    {
        public object NewValue { get; private set; }
        public string PropertyName { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }

        public bool ChangeFailed { get; set; } = false;
        public object FailedChangePropertyValue { get; set; } = null;

        public PropertyEditorValueChangedEventArgs(object newValue, string propertyName, PropertyInfo propertyInfo)
        {
            NewValue = newValue;
            PropertyName = propertyName;
            PropertyInfo = propertyInfo;
        }
    }
#endif
}
