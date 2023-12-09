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
        /// Raised when the <see cref="IsSelected"/> value is changed.
        /// </summary>
        event ItemSelectionChangedEventHandler IsSelectedChanged;

        /// <summary>
        /// Set the <see cref="IsSelected"/> value for this control, while also providing additional information to pass along to the <see cref="IsSelectedChanged"/> event.
        /// </summary>
        /// <param name="value">the new value to set <see cref="IsSelected"/> to</param>
        /// <param name="trigger">the method by which the value has changed (if not specified, use <see cref="SelectionChangeTrigger.CodeUnknown"/>)</param>
        /// <param name="triggerSource">the object that triggered this value change, or <c>null</c></param>
#if NETCOREAPP
        void SetIsSelectedWithSource(bool value, SelectionChangeTrigger trigger, object? triggerSource);
#else 
        void SetIsSelectedWithSource(bool value, SelectionChangeTrigger trigger, object triggerSource);
#endif

        /// <summary>
        /// Perform a click programmatically. This control responds the same way as if it was clicked by the user.
        /// </summary>
        void DoClick();

        #region Appearance

        /// <summary>
        /// Update this control's brushes and appearance using a ColorScheme.
        /// </summary>
        /// <param name="cs">the ColorScheme to apply</param>
        void ApplyColorScheme(ColorScheme cs);

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
        #endregion
    }


    /// <summary>
    /// Represents a handler for the <see cref="IClickSelectableControl.IsSelectedChanged"/> event.
    /// </summary>
    /// <param name="sender">The source object of the event.</param>
    /// <param name="e">The event arguments, containing information on the new IsSelected value and how the selection changed.</param>
    public delegate void ItemSelectionChangedEventHandler(object sender, ItemSelectionChangedEventArgs e);

}
