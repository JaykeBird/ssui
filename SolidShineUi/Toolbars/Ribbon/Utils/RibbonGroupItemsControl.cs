using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SolidShineUi.Toolbars.Ribbon.Utils
{
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
