using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{

    /// <summary>
    /// Adds additional methods that can be used to hook into the menu item's IsEnabledChanged event.
    /// </summary>
    public static class MenuItemExtensions
    {

        /// <summary>
        /// Attach a handler for the IsEnabledChanged event for a particular MenuItem.
        /// </summary>
        /// <param name="mi">The MenuItem to attach to.</param>
        /// <param name="eh">The event handler to use.</param>
        /// <remarks>When done listening to this event or when deconstructing or cleaning up the UI, call <c>DetachEnabledHandler</c> to detach this event handler.</remarks>
        public static void AttachEnabledHandler(this System.Windows.Controls.MenuItem mi, System.Windows.DependencyPropertyChangedEventHandler eh)
        {
            mi.IsEnabledChanged += eh;
        }

        /// <summary>
        /// Detach a handler for the IsEnabledChanged event for a particular MenuItem.
        /// </summary>
        /// <param name="mi">The MenuItem to detach from.</param>
        /// <param name="eh">The event handler to detach.</param>
        public static void DetachEnabledHandler(this System.Windows.Controls.MenuItem mi, System.Windows.DependencyPropertyChangedEventHandler eh)
        {
            mi.IsEnabledChanged -= eh;
        }
    }
}
