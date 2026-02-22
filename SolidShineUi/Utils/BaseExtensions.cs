using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Extension methods for various WPF or C# objects.
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
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(dictionary);
#else
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
#endif

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

        /// <summary>
        /// Get a shorter version of this string, by removing extra characters beyond a certain maximum value.
        /// </summary>
        /// <param name="str">the string to truncaste</param>
        /// <param name="maxLength">the max number of characters to have in the truncated version</param>
        /// <param name="includeEllipses">if truncated, display the ellipses (<c>"..."</c>) at the end</param>
        /// <returns>A truncated string (or the string itself if the string is less than the maximum length)</returns>
        /// <remarks>
        /// This is good for scenarios where you wish to display a long string in the UI but have a space constraint. Note that this is inherently a loss of data,
        /// as this simply removes characters from the end of the string. As a result, this should only be used for temporary purposes, like displaying in UI,
        /// and not used in relation to memory or data storage. Alternatively, I recommend utilizing <see cref="System.Windows.Controls.TextBlock.TextTrimming"/>
        /// as an in-built way to temporary truncate long strings in the UI, where possible.
        /// <para/>
        /// This function checks if the <see cref="char"/> at the specified <paramref name="maxLength"/> is part of a surrogate pair. Truncating strings in the middle 
        /// of a surrogate pair results in corrupted data at the end of a string. Instead, this function decreases the length by 1 and truncates there.
        /// </remarks>
        public static string Truncate(this string str, int maxLength, bool includeEllipses = true)
        {
            if (str.Length > maxLength)
            {
                // TODO: transition to using IsHighSurrogate or IsLowSurrogate as needed
#if NETCOREAPP
                if (char.IsSurrogate(str[maxLength - 1]))
                {
                    return string.Concat(str.AsSpan(0, maxLength - 1), includeEllipses ? "..." : "");
                }
                else
                {
                    return string.Concat(str.AsSpan(0, maxLength - 2), includeEllipses ? "..." : "");
                }
#else
                if (char.IsSurrogate(str[maxLength - 1]))
                {
                    return str.Substring(0, maxLength - 1) + (includeEllipses ? "..." : "");
                }
                else
                {
                    return str.Substring(0, maxLength - 2) + (includeEllipses ? "..." : "");
                }
#endif
            }
            else
            {
                return str;
            }
        }

    }
}
