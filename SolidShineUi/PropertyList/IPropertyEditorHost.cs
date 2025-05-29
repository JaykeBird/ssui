using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// Rerpresents an interface for all controls that can host one or more <see cref="IPropertyEditor"/> items.
    /// </summary>
    public interface IPropertyEditorHost
    {


        /// <summary>
        /// Get the object that is currently being observed in this control.
        /// </summary>
#if NETCOREAPP
        object? GetCurrentlyLoadedObject();
#else
        object GetCurrentlyLoadedObject();
#endif

        /// <summary>
        /// Get the window containing the host control, if available.
        /// </summary>
        /// <returns>The window containing the host control, if there is one; <c>null</c> otherwise.</returns>
        /// <remarks>
        /// For implementers of this interface, just call <see cref="Window.GetWindow(DependencyObject)"/> and return that value.
        /// </remarks>
#if NETCOREAPP
        Window? GetWindow();
#else
        Window GetWindow();
#endif

        /// <summary>
        /// Create a new IPropertyEditor object appropriate for the passed-in type. This is based upon what types are registered in the host control.
        /// </summary>
        /// <param name="propType">The type for which to get a IPropertyEditor for.</param>
        /// <returns>An IPropertyEditor that can be used for editing the type, if available; <c>null</c> otherwise</returns>
#if NETCOREAPP
        IPropertyEditor? CreateEditorForType(Type propType);
#else
        IPropertyEditor CreateEditorForType(Type propType);
#endif

    }
}
