using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{

    /// <summary>
    /// References the action to take when the currently-selected tab is closed.
    /// </summary>
    public enum SelectedTabCloseAction
    {
        /// <summary>Do not select anything; all tabs are deselected and nothing is shown.</summary>
        SelectNothing = 0,
        /// <summary>Select the first (leftmost) tab on the TabControl.</summary>
        SelectFirstTab = 1,
        /// <summary>Select the last (rightmost) tab on the TabControl.</summary>
        SelectLastTab = 2,
        /// <summary>Select the tab to the left of the one being closed.</summary>
        SelectTabToLeft = 3,
        /// <summary>Select the tab to the right of the one being closed.</summary>
        SelectTabToRight = 4,
    }

    /// <summary>
    /// Represents the scrolling action to take when executing the TabScroll command.
    /// </summary>
    public enum TabScrollCommandAction
    {
        /// <summary>
        /// Scroll to the left a set amount.
        /// </summary>
        Left = 0,
        /// <summary>
        /// Scroll to the right a set amount.
        /// </summary>
        Right = 1,
        /// <summary>
        /// Scroll to the very left end.
        /// </summary>
        Home = 2,
        /// <summary>
        /// Scroll to the very right end.
        /// </summary>
        End = 3
    }

}
