using System;
using System.Collections.Generic;
using System.Windows;
using SolidShineUi;

namespace SolidShineUi.PropertyList
{

    /// <summary>
    /// Represents the common interface that all property editor controls inherit from, to provide methods in regards to loading, editing, and setting properties.
    /// Classes that implement this interface should inherit from <see cref="System.Windows.Controls.Control"/> or <see cref="System.Windows.Controls.UserControl"/>.
    /// </summary>
    public interface IPropertyEditor : IInputElement, IFrameworkInputElement
    {

        /// <summary>
        /// Get a list of all valid types that this editor supports editing.
        /// </summary>
        List<Type> ValidTypes { get; }

        /// <summary>
        /// Get whether this editor control actually has the capability to edit the value of a property, not just viewing it.
        /// </summary>
        bool EditorAllowsModifying { get; }

        /// <summary>
        /// Get or set if the property being loaded can actually be edited (writeable).
        /// </summary>
        bool IsPropertyWritable { get; set; }

        /// <summary>
        /// Get the FrameworkElement for this IPropertyEditor control.
        /// </summary>
        /// <remarks>
        /// This is present due to how the WPF architecture was designed, and limitations with C# itself.
        /// Please simply return <c>this</c> in your code when implementing this interface.
        /// </remarks>
        FrameworkElement GetFrameworkElement();

        /// <summary>
        /// Set the ColorScheme, to set the visual appearance of the control.
        /// </summary>
        /// <remarks>
        /// This is a setter-only property in the interface as a getter is never needed. Implementers can add a getter for this property if needed.
        /// </remarks>
        ColorScheme ColorScheme { set; }

        /// <summary>
        /// Set the parent PropertyList control for this IPropertyEditor. This allows the IPropertyEditor to connect to the PropertyList directly, to get info or set certain values.
        /// </summary>
        ExperimentalPropertyList ParentPropertyList { set; }

        /// <summary>
        /// Raised when the value is changed, by changing the data in this editor control.
        /// Handlers should then call <see cref="GetValue"/> to then get the value to set the underlying property to.
        /// </summary>
        event EventHandler ValueChanged;

        /// <summary>
        /// Load in the value of a property.
        /// </summary>
        /// <param name="value">The value to load.</param>
        /// <param name="type">The type of the property; this may not exactly match the type of the value itself, but the value's type will always implement this type.</param>
#if NETCOREAPP
        public void LoadValue(object? value, Type type);

        /// <summary>
        /// Get the value of the property, as set in this editor control.
        /// </summary>
        public object? GetValue();

#else
        void LoadValue(object value, Type type);

        /// <summary>
        /// Get the value of the property, as set in this editor control.
        /// </summary>
        object GetValue();
#endif
    }
}
