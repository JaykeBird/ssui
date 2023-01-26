using System;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// An attribute that can be applied to properties; 
    /// when PropertyList loads an object, any properties in that object with this attribute are skipped.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PropertyListHideAttribute : Attribute
    {
        /// <summary>
        /// Create a PropertyListHideAttribute.
        /// </summary>
        public PropertyListHideAttribute() { }
    }

    /// <summary>
    /// An attribute that can be applied to properties; 
    /// if a PropertyList is set to only load properties with this attribute, all properties in an object are skipped unless they have this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PropertyListShowAttribute : Attribute
    {
        /// <summary>
        /// Create a PropertyListShowAttribute.
        /// </summary>
        public PropertyListShowAttribute() { }
    }
}
