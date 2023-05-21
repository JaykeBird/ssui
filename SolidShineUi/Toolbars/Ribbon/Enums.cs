using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi.Toolbars.Ribbon
{
    /// <summary>
    /// The display options for a <see cref="RibbonGroup"/>.
    /// </summary>
    public enum GroupSizeMode
    {
        /// <summary>
        /// Display the commands in this group at the largest size that they can fit.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// Compact down the commands in this group as small as they can.
        /// </summary>
        Compact = 1,

        /// <summary>
        /// Display the group only as an icon/menu, which will display the commands in a pop-up when clicked.
        /// </summary>
        IconOnly = 2,
    }

    /// <summary>
    /// The size of a Ribbon element within its group.
    /// </summary>
    public enum RibbonElementSize
    {
        /// <summary>
        /// Large size and large icon. This will take up the entire height of the Ribbon's control area.
        /// </summary>
        Large = 1,
        /// <summary>
        /// Small size and small icon. Multiple small elements can be displayed stacked together in the Ribbon's control area.
        /// </summary>
        Small = 2,
        /// <summary>
        /// Small icon and display no text. Multiple small elements can be displayed stacked together in the Ribbon's control area.
        /// </summary>
        IconOnly = 0,
    }
}
