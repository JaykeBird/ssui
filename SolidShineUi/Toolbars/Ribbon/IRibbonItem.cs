using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi.Toolbars.Ribbon
{
    /// <summary>
    /// An item that can be displayed on a <see cref="Ribbon"/>.
    /// </summary>
    public interface IRibbonItem : IToolbarItem
    {
        /// <summary>
        /// Get or set if the item is currently in its compacted state.
        /// </summary>
        bool IsCompacted { get; set; }

        /// <summary>
        /// Get or set the item's size to use when it is in its standard (non-compacted) state.
        /// </summary>
        RibbonElementSize StandardSize { get; set; }

        /// <summary>
        /// Get or set the item's size to use when it is in its compacted state.
        /// </summary>
        RibbonElementSize CompactSize { get; set; }

        /// <summary>
        /// Get or set the names of the key(s) that can be pressed to activate this command via keyboard interaction.
        /// </summary>
        string AccessKey { get; set; }

        /// <summary>
        /// Get or set the tooltip to display when mousing over this command.
        /// </summary>
        object ToolTip { get; set; }

    }
}
