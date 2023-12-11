using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi.PropertyList
{

    /// <summary>
    /// The event arguments for the PropertyEditorValueChanged event, which is raised when the value of a property is changed using a property editor control.
    /// </summary>
#if NETCOREAPP
    public class PropertyEditorValueChangedEventArgs : PropertyValueChangedEventArgs
    {
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
        /// <param name="oldValue">The old value of the property being changed.</param>
        /// <param name="newValue">The new value of the property being changed.</param>
        /// <param name="propertyName">The name of the property being changed.</param>
        /// <param name="propertyInfo">The PropertyInfo representing the property being changed.</param>
        public PropertyEditorValueChangedEventArgs(object? oldValue, object? newValue, string propertyName, PropertyInfo? propertyInfo) : base(oldValue, newValue, propertyName, propertyInfo) { }
    }
#else
    public class PropertyEditorValueChangedEventArgs : PropertyValueChangedEventArgs
    {
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
        /// <param name="oldValue">The old value of the property being changed.</param>
        /// <param name="newValue">The new value of the property being changed.</param>
        /// <param name="propertyName">The name of the property being changed.</param>
        /// <param name="propertyInfo">The PropertyInfo representing the property being changed.</param>
        public PropertyEditorValueChangedEventArgs(object oldValue, object newValue, string propertyName, PropertyInfo propertyInfo) : base(oldValue, newValue, propertyName, propertyInfo) { }
    }
#endif


    /// <summary>
    /// The event arguments for the PropertyValueChanged event, which is raised when the value of a property is changed in a PropertyList.
    /// </summary>
#if NETCOREAPP
    public class PropertyValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Get the new value to apply to this property.
        /// </summary>
        public object? NewValue { get; private set; }

        /// <summary>
        /// Get the old value of the property. Note that this may not always be set.
        /// </summary>
        public object? OldValue { get; private set; }

        /// <summary>
        /// Get the name of the property being changed.
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// Get the PropertyInfo representing the property being changed.
        /// </summary>
        public PropertyInfo? PropertyInfo { get; private set; }

        /// <summary>
        /// Create a PropertyValueChangedEventArgs.
        /// </summary>
        /// <param name="oldValue">The old value of the property being changed.</param>
        /// <param name="newValue">The new value of the property being changed.</param>
        /// <param name="propertyName">The name of the property being changed.</param>
        /// <param name="propertyInfo">The PropertyInfo representing the property being changed.</param>
        public PropertyValueChangedEventArgs(object? oldValue, object? newValue, string propertyName, PropertyInfo? propertyInfo)
        {
            OldValue = oldValue;
            NewValue = newValue;
            PropertyName = propertyName;
            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Create a PropertyValueChangedEventArgs, by copying the values of an existing PropertyValueChangedEventArgs class. Note that this isn't a deep copy.
        /// </summary>
        /// <param name="args">The class to copy the values from.</param>
        public PropertyValueChangedEventArgs(PropertyValueChangedEventArgs args)
        {
            OldValue = args.OldValue;
            NewValue = args.NewValue;
            PropertyName = args.PropertyName;
            PropertyInfo = args.PropertyInfo;
        }
    }
#else
    public class PropertyValueChangedEventArgs
    {
        /// <summary>
        /// Get the new value to apply to this property.
        /// </summary>
        public object NewValue { get; private set; }
        /// <summary>
        /// Get the old value of the property. Note that this may not always be set.
        /// </summary>
        public object OldValue { get; private set; }
        /// <summary>
        /// Get the name of the property being changed.
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// Get the PropertyInfo representing the property being changed.
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Create a PropertyValueChangedEventArgs.
        /// </summary>
        /// <param name="oldValue">The old value of the property being changed.</param>
        /// <param name="newValue">The new value of the property being changed.</param>
        /// <param name="propertyName">The name of the property being changed.</param>
        /// <param name="propertyInfo">The PropertyInfo representing the property being changed.</param>
        public PropertyValueChangedEventArgs(object oldValue, object newValue, string propertyName, PropertyInfo propertyInfo)
        {
            OldValue = oldValue;
            NewValue = newValue;
            PropertyName = propertyName;
            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Create a PropertyValueChangedEventArgs, by copying the values of an existing PropertyValueChangedEventArgs class. Note that this isn't a deep copy.
        /// </summary>
        /// <param name="args">The class to copy the values from.</param>
        public PropertyValueChangedEventArgs(PropertyValueChangedEventArgs args)
        {
            OldValue = args.OldValue;
            NewValue = args.NewValue;
            PropertyName = args.PropertyName;
            PropertyInfo = args.PropertyInfo;
        }
    }
#endif

    /// <summary>
    /// The event arguments for the LoadedObjectChanged event, when the loaded object changes in the PropertyList control.
    /// </summary>
    public class PropertyListObjectEventArgs : EventArgs
    {
        /// <summary>
        /// The object that was loaded into the PropertyList control.
        /// </summary>
        public object LoadedObject { get; private set; }

        /// <summary>
        /// The determined type of the object that was loaded into the PropertyList control.
        /// </summary>
        public Type LoadedObjectType { get; private set; }

        /// <summary>
        /// Get if this event was triggered by reloading the current object, rather than loading in a new object.
        /// </summary>
        public bool IsReload { get; private set; }

        /// <summary>
        /// Create a new PropertyListObjectEventArgs.
        /// </summary>
        /// <param name="loadedObject">The object being loaded in</param>
        /// <param name="loadedType">the type of the object being loaded</param>
        /// <param name="isReload">Set if this loading action actually a reload</param>
        public PropertyListObjectEventArgs(object loadedObject, Type loadedType, bool isReload)
        {
            LoadedObject = loadedObject;
            LoadedObjectType = loadedType;
            IsReload = isReload;
        }
    }
}
