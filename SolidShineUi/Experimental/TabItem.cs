using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xaml;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi.Experimental
{
    [ContentProperty("Content")]
    public class TabItem
    {
        public string Header { get; set; } = "New Tab";

        public bool IsDirty { get; set; } = false;

        public bool CanClose { get; set; } = true;

#if NETCOREAPP
        public ImageSource? Icon { get; set; } = null;

        public UIElement? Content { get; set; }
#else
        public ImageSource Icon { get; set; } = null;

        public UIElement Content { get; set; }
#endif

        // TODO: add Changed events for these properties
    }
}
