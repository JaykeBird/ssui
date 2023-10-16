using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi.PropertyList
{
    /// <summary>
    /// Represents the sorting method used for sorting properties in a <see cref="PropertyList"/>.
    /// </summary>
    public enum PropertySortOption
    {
        /// <summary>
        /// Sorted alphabetically by property name
        /// </summary>
        Name = 0,
        /// <summary>
        /// Sorted by category (as determined by a Category attribute being present)
        /// </summary>
        Category = 1
    }

    /// <summary>
    /// Represents what properties should be shown or hidden in a <see cref="PropertyList"/>, by checking the attributes set with the property.
    /// </summary>
    [Flags]
    public enum PropertyListDisplayFlags
    {
        /// <summary>
        /// Hides properties that have the PropertyListHide attribute (<see cref="PropertyListHideAttribute"/>) set.
        /// </summary>
        HidePropertyListHide = 1,
        /// <summary>
        /// Hides properties that have the Browseable or EditorBrowseable attributes set (and set to false or Never).
        /// </summary>
        HideBrowseableFalse = 2,
        /// <summary>
        /// Hides properties that have the Obsolete attribute set.
        /// </summary>
        HideObsolete = 4,
        /// <summary>
        /// Only properties that have the PropertyListShow attribute (<see cref="PropertyListShowAttribute"/>) will be displayed. This overrides all other flags.
        /// </summary>
        OnlyShowPropertyListShow = 8,
        /// <summary>
        /// Ignore attributes and display all properties in an object. This overrides all other flags.
        /// </summary>
        ShowAll = 16,
    }
}
