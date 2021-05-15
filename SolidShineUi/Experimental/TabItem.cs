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

        private bool sel = false;

        public bool CanSelect { get; set; } = true;

        public bool IsSelected
        {
            get
            {
                return sel;
            }
        }

        public void Select()
        {
            sel = true;
            IsSelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        internal void Unselect()
        {
            sel = false;
            IsSelectedChanged?.Invoke(this, EventArgs.Empty);
        }

#if NETCOREAPP
        public ImageSource? Icon { get; set; } = null;

        public UIElement? Content { get; set; }

        public event EventHandler? TabPropertiesChanged;
        public event EventHandler? IsSelectedChanged;
#else
        public ImageSource Icon { get; set; } = null;

        public UIElement Content { get; set; }

        public event EventHandler TabPropertiesChanged;
        public event EventHandler IsSelectedChanged;
#endif
    }
}
