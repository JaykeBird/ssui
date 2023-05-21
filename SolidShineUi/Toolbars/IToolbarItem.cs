using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.Toolbars
{
    public interface IToolbarItem
    {

        string Title { get; set; }

        Visibility Visibility { get; set; }

        ImageSource LargeIcon { get; set; }
        
        ImageSource SmallIcon { get; set; }

        int CompactOrder { get; set; }

        object ToolTip { get; set; }
    }
}
