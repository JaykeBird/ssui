using System;
using System.Collections.Generic;
using System.Text;

namespace SolidShineUi.Utils
{

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
