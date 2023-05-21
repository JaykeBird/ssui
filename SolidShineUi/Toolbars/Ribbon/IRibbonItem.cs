using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi.Toolbars.Ribbon
{
    public interface IRibbonItem : IToolbarItem
    {

        RibbonElementSize StandardSize { get; set; }

        RibbonElementSize CompactSize { get; set; }

        string AccessKey { get; set; }

    }
}
