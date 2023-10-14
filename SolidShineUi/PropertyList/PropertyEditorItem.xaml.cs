using System;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// A control hosting a visual representation of an object's property, including the property's name, type, and (where possible) an editor control for editing the property.
    /// This is meant for use with the <see cref="ExperimentalPropertyList"/>, not generally for use directly by itself.
    /// </summary>
    [Localizability(LocalizationCategory.ListBox)]
    public partial class PropertyEditorItem : UserControl
    {
        /// <summary>
        /// Create a new PropertyEditorItem
        /// </summary>
        public PropertyEditorItem()
        {
            InitializeComponent();
        }

        #region Property Properties
#if NETCOREAPP
        /// <summary>
        /// Get or set the PropertyInfo representing the property shown.
        /// </summary>
        public PropertyInfo? PropertyInfo { get; private set; } = null;

        object? _value = null;

        /// <summary>
        /// Get or set the value of the property shown.
        /// </summary>
        public object? PropertyValue { get => _value; set { _value = value; txtValue.Text = (value ?? "(null)").ToString(); } }

        private object? _oldValue = null;

        /// <summary>
        /// Get or set the editor control to use to allow editing the value of this property.
        /// </summary>
        public IPropertyEditor? PropertyEditorControl { get; set; }

        /// <summary>
        /// Get if this property is read only, meaning it cannot be edited or changed.
        /// </summary>
        public bool IsReadOnly { get; private set; }


        /// <summary>
        /// A delegate to be used with events regarding the value of a property editor's property changing.
        /// </summary>
        /// <param name="sender">The object where the event was raised.</param>
        /// <param name="e">The event arguments associated with this event.</param>
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
        public PropertyInfo PropertyInfo { get; private set; } = null;

        object _value = null;

        /// <summary>
        /// Get or set the value of the property shown.
        /// </summary>
        public object PropertyValue { get => _value; set { _value = value; txtValue.Text = (value ?? "(null)").ToString(); } }

        private object _oldValue = null;

        /// <summary>
        /// Get or set the editor control to use to allow editing the value of this property.
        /// </summary>
        public IPropertyEditor PropertyEditorControl { get; set; }

        /// <summary>
        /// Get if this property is read only, meaning it cannot be edited or changed.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// A delegate to be used with events regarding the value of a property editor's property changing.
        /// </summary>
        /// <param name="sender">The object where the event was raised.</param>
        /// <param name="e">The event arguments associated with this event.</param>
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
        public Type PropertyType { get; private set; } = typeof(object);

        /// <summary>
        /// Get or set the type of the property being shown, in a more user-friendly textual format.
        /// </summary>
        public string PropertyTypeText { get => txtType.Text; set => txtType.Text = value; }
        #endregion

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
            PropertyType = property.PropertyType;
            PropertyTypeText = ExperimentalPropertyList.PrettifyPropertyType(property.PropertyType);
            txtType.ToolTip = ExperimentalPropertyList.PrettifyPropertyType(property.PropertyType, true);
            PropertyValue = value;
            IsReadOnly = !property.CanWrite;
            //var propVal = property.GetCustomAttribute<DescriptionAttribute>();
            //if (propVal != null)
            //{
            //    ToolTip = propVal.Description;
            //}

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
#if NETCOREAPP
                object? potentialOldValue = PropertyValue;
#else
                object potentialOldValue = PropertyValue;
#endif
                PropertyValue = PropertyEditorControl.GetValue();
                var ev = new PropertyEditorValueChangedEventArgs(potentialOldValue, PropertyValue, PropertyName, PropertyInfo);
                PropertyEditorValueChanged?.Invoke(this, ev);

                if (ev.ChangeFailed)
                {
                    PropertyValue = ev.FailedChangePropertyValue;

                    if (PropertyInfo != null)
                    {
                        PropertyEditorControl.LoadValue(PropertyValue, PropertyInfo.PropertyType);
                    }
                }
                else
                {
                    _oldValue = potentialOldValue;
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

        //string PrettifyPropertyType(Type type, bool fullName = false)
        //{
        //    string typeString = type.FullName ?? "(no type name)";
        //    string baseName = type.Name;

        //    if (type.IsGenericType)
        //    {
        //        var generics = type.GetGenericArguments();

        //        if (typeString.StartsWith("System.Nullable"))
        //        {
        //            return (fullName ? generics[0].FullName : generics[0].Name) + "?";
        //        }

        //        string basebase = (fullName ? (type.GetGenericTypeDefinition().FullName ?? "System.Object") : baseName).Replace("`1", "").Replace("`2", "").Replace("`3", "").Replace("`4", "").Replace("`5", "");

        //        if (generics.Length == 1)
        //        {
        //            return basebase + "<" + (fullName ? generics[0].FullName : generics[0].Name) + ">";
        //        }
        //        else
        //        {
        //            return basebase + "<" + string.Join(",", generics.Select(x => fullName ? x.FullName : x.Name)) + ">";
        //        }
        //    }
        //    else
        //    {
        //        return fullName ? typeString : baseName;
        //    }
        //}

        #region Visuals Settings
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

        /// <summary>
        /// Get or set if gridlines should be displayed in the editor entry. Showing the gridlines will make the editor appear more like the WinForms PropertyGrid control.
        /// </summary>
        public bool ShowGridlines { get => (bool)GetValue(ShowGridlinesProperty); set => SetValue(ShowGridlinesProperty, value); }

        /// <summary>
        /// A depedency property that backs a related property. See the related property for more details.
        /// </summary>
        public static DependencyProperty ShowGridlinesProperty
            = DependencyProperty.Register("ShowGridlines", typeof(bool), typeof(PropertyEditorItem),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set the brush to use for the gridlines. Use <see cref="ShowGridlines"/> to determine if the gridlines will be visible or not.
        /// </summary>
        public Brush GridlineBrush { get => (Brush)GetValue(GridlineBrushProperty); set => SetValue(GridlineBrushProperty, value); }

        /// <summary>
        /// A depedency property that backs a related property. See the related property for more details.
        /// </summary>
        public static DependencyProperty GridlineBrushProperty
            = DependencyProperty.Register("GridlineBrush", typeof(Brush), typeof(PropertyEditorItem),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LightGray)));


        #endregion
    }

}
