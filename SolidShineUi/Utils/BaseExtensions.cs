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
        public static void PerformAs<T>(this DependencyObject dp, Action<T> action)
        {
            if (dp is null) return;
            if (dp is T o)
            {
                action.Invoke(o);
            }
        }

        /// <summary>
        /// Perform an action on an object that inherits from DependencyObject, with an extra check on the value's type. This includes a null and type check on the DependencyObject,
        /// and a type check on the value.
        /// </summary>
        /// <remarks>
        /// This is designed to work alongside the WPF dependency property system, since dependency property changed events only
        /// pass in a DependencyObject object, and the DependencyPropertyChangedEventArgs only provides an object as the newValue. 
        /// This method acts as an alternative to direct casting for both of those, allowing you to access your
        /// object's methods and events, in a manner that also satisfies .NET's nullability feature.
        /// <para/>
        /// If the DependencyObject is null or isn't of type <typeparamref name="T"/>, or if <paramref name="newValue"/> isn't of type <typeparamref name="TValue"/>, then the action does not run.
        /// </remarks>
        /// <typeparam name="T">The type of the object to cast to.</typeparam>
        /// <typeparam name="TValue">The type of the value object to cast to.</typeparam>
        /// <param name="dp">The dependency object to cast from.</param>
        /// <param name="action">The action to take on the casted object.</param>
        /// <param name="newValue">The new value of the property that has changed</param>
        public static void PerformAs<T, TValue>(this DependencyObject dp, object newValue, Action<T, TValue> action)
        {
            if (dp is null) return;
            if (newValue is TValue v && dp is T o)
            {
                action.Invoke(o, v);
            }
        }

        /// <summary>
        /// Get a specific value from the dictionary, or return <c>null</c> if a value isn't present with that key.
        /// </summary>
        /// <typeparam name="TKey">the key type of the dictionary</typeparam>
        /// <typeparam name="TValue">the value type of the dictionary</typeparam>
        /// <param name="dictionary">the dictionary to pull from</param>
        /// <param name="key">the key value to look up</param>
        /// <returns>the value of <paramref name="key"/> in the <paramref name="dictionary"/>, or <c>null</c> if that key is not present</returns>
#if NETCOREAPP
        public static TValue? GetValueOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull where TValue : class
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return dictionary.TryGetValue(key, out TValue? value) ? value : null;
        }
#else
        public static TValue GetValueOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return dictionary.TryGetValue(key, out TValue value) ? value : null;
        }
#endif
    }
}
