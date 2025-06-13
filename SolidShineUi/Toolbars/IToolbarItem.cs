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
    /// <summary>
    /// An item that can be displayed on a toolbar or <see cref="Ribbon"/>.
    /// </summary>
    public interface IToolbarItem
    {
        /// <summary>
        /// Get or set the title or label of the command. If not displayed directly on the UI, this is usually shown in a tooltip while the mouse is over the item.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Get or set if this command is visible in the UI.
        /// </summary>
        Visibility Visibility { get; set; }

        /// <summary>
        /// Get or set if this command is currently enabled and can be interacted with. A disabled command cannot be activated, but is still visible in the UI.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Get or set the icon to use with this command when large icons are being used.
        /// </summary>
        /// <remarks>
        /// For Ribbons, the large icon should be 32x32 in size.
        /// </remarks>
        ImageSource LargeIcon { get; set; }

        /// <summary>
        /// Get or set the icon to use with this command when small icons are being used.
        /// </summary>
        /// <remarks>
        /// For Ribbons, the small icon should be 16x16 in size.
        /// </remarks>
        ImageSource SmallIcon { get; set; }

        /// <summary>
        /// Get or set the priority for hiding or compacting this command, when the toolbar or Ribbon it is on is too wide to be displayed in full in its container.
        /// </summary>
        int CompactOrder { get; set; }

        /// <summary>
        /// Get or set the ColorScheme to use for setting the appearance of this command on the toolbar or Ribbon.
        /// </summary>
        ColorScheme ColorScheme { get; set; }
    }
}
