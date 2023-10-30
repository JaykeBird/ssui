using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A base interface defining the common clicking and selecting methods and events used in Solid Shine UI controls.
    /// </summary>
    public interface IClickSelectableControl : IInputElement
    {
        /// <summary>
        /// Get or set the selection status of this control. Selected controls often have a different visual appearance as well.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Get or set if this control's <see cref="IsSelected"/> value changes when it is clicked.
        /// </summary>
        bool SelectOnClick { get; set; }

        /// <summary>
        /// Raised when the user clicks on this control.
        /// </summary>
        event RoutedEventHandler Click;

        /// <summary>
        /// Raised when the user right-clicks on this control. This usually activates a context menu, but could be used for other actions.
        /// </summary>
        event RoutedEventHandler RightClick;

        /// <summary>
        /// Perform a click programmatically. This control responds the same way as if it was clicked by the user.
        /// </summary>
        void DoClick();

        /// <summary>
        /// Get or set the background brush used for this control, when it isn't being clicked or selected.
        /// </summary>
        Brush Background { get; set; }

        /// <summary>
        /// Get or set the background brush used for this control, while it has the mouse over it or has keyboard focus.
        /// </summary>
        Brush HighlightBrush { get; set; }

        /// <summary>
        /// Get or set the background brush used for this control, while the mouse is clicking down on it.
        /// </summary>
        Brush ClickBrush { get; set; }

        /// <summary>
        /// Get or set the background brush used for this control, while it is selected (and isn't being clicked).
        /// </summary>
        Brush SelectedBrush { get; set; }
    }
}
