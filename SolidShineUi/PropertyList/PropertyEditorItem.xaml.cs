using System;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// A control hosting a visual representation of an object's property, including the property's name, type, and (where possible) an editor control for editing the property.
    /// For use with the <see cref="ExperimentalPropertyList"/>, not generally for use directly by itself.
    /// </summary>
    public partial class PropertyEditorItem : UserControl
    {
        /// <summary>
        /// Create a new PropertyEditorItem
        /// </summary>
        public PropertyEditorItem()
        {
            InitializeComponent();
        }

#if NETCOREAPP
        /// <summary>
        /// Get or set the PropertyInfo representing the property shown.
        /// </summary>
        public PropertyInfo? PropertyInfo { get; set; } = null;

        object? _value = null;

        /// <summary>
        /// Get or set the value of the property shown.
        /// </summary>
        public object? PropertyValue { get => _value; set { _value = value; txtValue.Text = (value ?? "(null)").ToString(); } }

        /// <summary>
        /// Get or set the editor control to use to allow editing the value of this property.
        /// </summary>
        public IPropertyEditor? PropertyEditorControl { get; set; }

        public delegate void PropertyEditorValueChangedEventHandler(object? sender, PropertyEditorValueChangedEventArgs e);

        /// <summary>
        /// Raised when the value of this property is changed, by use of a <see cref="PropertyEditorControl"/>.
        /// </summary>
        public event PropertyEditorValueChangedEventHandler? PropertyEditorValueChanged;

        /// <summary>
        /// Get or set the type that actually declared the property shown.
        /// </summary>
        public Type? DeclaringType { get; set; } = null;

#else
        /// <summary>
        /// Get or set the PropertyInfo representing the property shown.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; } = null;

        object _value = null;

        /// <summary>
        /// Get or set the value of the property shown.
        /// </summary>
        public object PropertyValue { get => _value; set { _value = value; txtValue.Text = (value ?? "(null)").ToString(); } }

        /// <summary>
        /// Get or set the editor control to use to allow editing the value of this property.
        /// </summary>
        public IPropertyEditor PropertyEditorControl { get; set; }

        public delegate void PropertyEditorValueChangedEventHandler(object sender, PropertyEditorValueChangedEventArgs e);

        /// <summary>
        /// Raised when the value of this property is changed, by use of a <see cref="PropertyEditorControl"/>.
        /// </summary>
        public event PropertyEditorValueChangedEventHandler PropertyEditorValueChanged;

        /// <summary>
        /// Get or set the type that actually declared the property shown.
        /// </summary>
        public Type DeclaringType { get; set; } = null;
#endif

        /// <summary>
        /// Get or set if this property is an inherited property (i.e. the <see cref="DeclaringType"/> doesn't match the type of the object that is being observed in the parent <see cref="ExperimentalPropertyList"/>).
        /// </summary>
        public bool IsInherited { get; set; } = false;

        /// <summary>
        /// Get or set the name of the property being shown.
        /// </summary>
        public string PropertyName { get => txtName.Text; set => txtName.Text = value; }

        /// <summary>
        /// Get or set the type of the property being shown.
        /// </summary>
        public string PropertyType { get => txtType.Text; set => txtType.Text = value; }

        /// <summary>
        /// Load in a property to show in this PropertyEditorItem, with (if possible) a IPropertyEditor control to allow editing the property value.
        /// </summary>
        /// <param name="property">The information about this property being shown. This is used to fill most data in this control.</param>
        /// <param name="value">The value of the property, in regards to the object being observed in the parent <see cref="ExperimentalPropertyList"/>.</param>
        /// <param name="editor">The IPropertyEditor control, if present, that is useable for editing the value of this property.</param>
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
                uie.VerticalAlignment = VerticalAlignment.Stretch;
                uie.Width = double.NaN; // Auto
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

        /// <summary>
        /// Set the value of this property, in regards to the target object.
        /// </summary>
        /// <param name="targetObject">The object to set the value of the property on.</param>
        /// <exception cref="ArgumentNullException">Thrown if the target object is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the PropertyInfo is null. If this is null, a property cannot be set.</exception>
        public void ApplyPropertyValue(object targetObject)
        {
            if (targetObject == null) throw new ArgumentNullException(nameof(targetObject), "Targetted object cannot be null");
            if (PropertyInfo == null) throw new InvalidOperationException("The property to set on the object hasn't been defined. Please set the " + nameof(PropertyInfo) + " property.");

            PropertyInfo.SetValue(targetObject, PropertyValue);
        }

        /// <summary>
        /// Update the widths of the internal columns in this control, to match the widths in some other location.
        /// </summary>
        /// <param name="namesCol">The width for the Name column.</param>
        /// <param name="typesCol">The width for the Type column.</param>
        /// <param name="valueCol">The width for the Value column.</param>
        public void UpdateColumnWidths(GridLength namesCol, GridLength typesCol, GridLength valueCol)
        {
            colNames.Width = namesCol;
            colType.Width = typesCol;
            colValues.Width = valueCol;
        }

    }

    /// <summary>
    /// The event arguments for the PropertyEditorValueChanged event, which takes place when the value of a property is changed using a property editor control.
    /// </summary>
#if NETCOREAPP
    public class PropertyEditorValueChangedEventArgs
    {
        /// <summary>
        /// Get the new value to apply to this property.
        /// </summary>
        public object? NewValue { get; private set; }
        /// <summary>
        /// Get the name of the property being changed.
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// Get the PropertyInfo representing the property being changed.
        /// </summary>
        public PropertyInfo? PropertyInfo { get; private set; }

        /// <summary>
        /// Get or set if the change to the property's value has failed. In cases where an exception occurred while attempting to set the value or something else unexpected happened and
        /// the new value could not be set, then set this value to <c>true</c> and then set the <see cref="FailedChangePropertyValue"/> to what the updated value actually is.
        /// </summary>
        public bool ChangeFailed { get; set; } = false;
        /// <summary>
        /// Get or set what the value of the property is after a failure to change the property to the new value above.
        /// </summary>
        public object? FailedChangePropertyValue { get; set; } = null;

        /// <summary>
        /// Create a PropertyEditorValueChangedEventArgs.
        /// </summary>
        /// <param name="newValue">The new value of the property being changed.</param>
        /// <param name="propertyName">The name of the property being changed.</param>
        /// <param name="propertyInfo">The PropertyInfo representing the property being changed.</param>
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
        /// <summary>
        /// Get the new value to apply to this property.
        /// </summary>
        public object NewValue { get; private set; }
        /// <summary>
        /// Get the name of the property being changed.
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// Get the PropertyInfo representing the property being changed.
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Get or set if the change to the property's value has failed. In cases where an exception occurred while attempting to set the value or something else unexpected happened and
        /// the new value could not be set, then set this value to <c>true</c> and then set the <see cref="FailedChangePropertyValue"/> to what the updated value actually is.
        /// </summary>
        public bool ChangeFailed { get; set; } = false;
        /// <summary>
        /// Get or set what the value of the property is after a failure to change the property to the new value above.
        /// </summary>
        public object FailedChangePropertyValue { get; set; } = null;

        /// <summary>
        /// Create a PropertyEditorValueChangedEventArgs.
        /// </summary>
        /// <param name="newValue">The new value of the property being changed.</param>
        /// <param name="propertyName">The name of the property being changed.</param>
        /// <param name="propertyInfo">The PropertyInfo representing the property being changed.</param>
        public PropertyEditorValueChangedEventArgs(object newValue, string propertyName, PropertyInfo propertyInfo)
        {
            NewValue = newValue;
            PropertyName = propertyName;
            PropertyInfo = propertyInfo;
        }
    }
#endif
}
