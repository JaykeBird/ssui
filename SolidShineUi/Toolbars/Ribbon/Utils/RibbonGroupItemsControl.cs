using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SolidShineUi.Toolbars.Ribbon.Utils
{

    /// <summary>
    /// A slightly modified version of an <see cref="ItemsControl"/> for usage with a <see cref="RibbonGroup"/>.
    /// </summary>
    public class RibbonGroupItemsControl : ItemsControl
    {

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return false;
            //return base.IsItemItsOwnContainerOverride(item);
        }

    }
}
