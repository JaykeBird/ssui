using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Extension methods for various WPF or C# objects. (While newer .NET may have some of these methods included, these are not available on .NET Framework).
    /// </summary>
    public static class BaseExtensions

    {

        /// <summary>
        /// Perform an action on an object that inherits from DependencyObject. This includes a null-value and type check.
        /// </summary>
        /// <remarks>
        /// This is designed to work alongside the WPF dependency property system, since dependency property changed events only
        /// pass in a DependencyObject object. This method acts as an alternative to direct casting, allowing you to access your
        /// object's methods and events, in a manner that also satisfies .NET's nullability feature.
        /// <para/>
        /// If the DependencyObject is null or isn't of type <typeparamref name="T"/>, then the action does not run.
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

        /// <summary>
        /// Get a specific value from the dictionary, or return <c>null</c> if a value isn't present with that key.
        /// </summary>
        /// <typeparam name="T1">the key type of the dictionary</typeparam>
        /// <typeparam name="T2">the value type of the dictionary</typeparam>
        /// <param name="dictionary">the dictionary to pull from</param>
        /// <param name="key">the key value to look up</param>
        /// <returns>the value of <paramref name="key"/> in the <paramref name="dictionary"/>, or <c>null</c> if that key is not present</returns>
#if NETCOREAPP
        public static T2? GetValueOrNull<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key) where T1 : notnull where T2 : class
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return dictionary.TryGetValue(key, out T2? value) ? value : null;
        }
#else
        public static T2 GetValueOrNull<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key) where T2 : class
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return dictionary.TryGetValue(key, out T2 value) ? value : null;
        }
#endif
    }
}
