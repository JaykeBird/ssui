using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Extension methods for <see cref="DependencyObject"/>.
    /// </summary>
    public static class DependencyObjectExtensions
    {

        /// <summary>
        /// Perform an action on an object that inherits from DependencyObject. This includes a null-value and type check.
        /// </summary>
        /// <remarks>
        /// This is designed to work alongside the WPF dependency property system, since dependency property changed events only
        /// pass in a DependencyObject object. This method acts as an alternative to direct casting, allowing you to access your
        /// object's methods and events, in a manner that also satisfies .NET's nullability feature.
        /// </remarks>
        /// <typeparam name="T">The type of the object to cast to.</typeparam>
        /// <param name="dp">The dependency object to cast from.</param>
        /// <param name="action">The action to take on the casted object.</param>
        public static void AsThis<T>(this DependencyObject dp, Action<T> action)
        {
            if (dp is null) return;
            if (dp is T o)
            {
                action.Invoke(o);
            }
        }
    }
}
