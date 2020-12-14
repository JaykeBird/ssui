using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{
    //[Obsolete("Please use the standard System.Windows.Controls.MenuItem control as they are identical. This control (SolidShineUi.MenuItem) will be removed in a later version.", false)]
    //public class MenuItem : System.Windows.Controls.MenuItem
    //{

    //    public void AttachEnabledHandler(System.Windows.DependencyPropertyChangedEventHandler eh)
    //    {
    //        IsEnabledChanged += eh;
    //    }

    //    public void DetachEnabledHandler(System.Windows.DependencyPropertyChangedEventHandler eh)
    //    {
    //        IsEnabledChanged -= eh;
    //    }

    //}

    /// <summary>
    /// Adds additional methods that can be used to hook into the menu item's IsEnabledChanged event.
    /// </summary>
    public static class MenuItemExtensions
    {
        public static void AttachEnabledHandler(this System.Windows.Controls.MenuItem mi, System.Windows.DependencyPropertyChangedEventHandler eh)
        {
            mi.IsEnabledChanged += eh;
        }

        public static void DetachEnabledHandler(this System.Windows.Controls.MenuItem mi, System.Windows.DependencyPropertyChangedEventHandler eh)
        {
            mi.IsEnabledChanged -= eh;
        }
    }
}
