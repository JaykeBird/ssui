using System;
using System.Windows;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A base interface defining the common clicking and selecting methods and events used in Solid Shine UI controls.
    /// </summary>
    /// <remarks>
    /// It is expected that this interface will be applied to a control/object that inherits from <see cref="FrameworkElement"/>.
    /// </remarks>
    public interface IClickSelectableControl : IFrameworkInputElement
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
        /// Get or set the SsuiTheme to apply for this control. Each control will apply the theme's colors appropriately via its own
        /// <c>OnApplySsuiTheme</c> method.
        /// </summary>
#if NETCOREAPP
        SsuiTheme? SsuiTheme { get; set; }
#else
        SsuiTheme SsuiTheme { get; set; }
#endif

        /// <summary>
        /// Get or set the background brush used for this control, when it isn't being clicked or selected.
        /// </summary>
        Brush Background { get; set; }

        /// <summary>
        /// Get or set the foreground brush used for this control.
        /// </summary>
        Brush Foreground { get; set; }

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

    /// <summary>
    /// The event arguments for the IsSelectedChanged event of the SelectableUserControl.
    /// </summary>
    public class ItemSelectionChangedEventArgs : RoutedEventArgs
    {

        /// <summary>
        /// Create a new ItemSelectionChangedRoutedEventArgs.
        /// </summary>
        /// <param name="ev">the routed event associated with these routed event args</param>
        /// <param name="oldValue">The old IsSelected value.</param>
        /// <param name="newValue">The new IsSelected value.</param>
        /// <param name="trigger">The trigger method that caused the value to be updated.</param>
        /// <param name="triggerSource">The source object that updated the value (if available).</param>
#if NETCOREAPP
        public ItemSelectionChangedEventArgs(RoutedEvent ev, bool oldValue, bool newValue, SelectionChangeTrigger trigger, object? triggerSource = null) : base(ev, triggerSource)
#else
        public ItemSelectionChangedEventArgs(RoutedEvent ev, bool oldValue, bool newValue, SelectionChangeTrigger trigger, object triggerSource = null) : base(ev, triggerSource)
#endif
        {
            OldValue = oldValue;
            NewValue = newValue;
            TriggerMethod = trigger;
            TriggerSource = triggerSource;
        }

        /// <summary>
        /// Create a new ItemSelectionChangedEventArgs, with a related DependencyProperty.
        /// </summary>
        /// <param name="ev">the routed event associated with these routed event args</param>
        /// <param name="oldValue">The old IsSelected value.</param>
        /// <param name="newValue">The new IsSelected value.</param>
        /// <param name="trigger">The trigger method that caused the value to be updated.</param>
        /// <param name="triggerSource">The source object that updated the value (if available).</param>
        /// <param name="sourceProperty">The dependency property related to this event change (if available).</param>
#if NETCOREAPP
        public ItemSelectionChangedEventArgs(RoutedEvent ev, bool oldValue, bool newValue, DependencyProperty? sourceProperty, SelectionChangeTrigger trigger, object? triggerSource = null) : base(ev, triggerSource)
#else
        public ItemSelectionChangedEventArgs(RoutedEvent ev, bool oldValue, bool newValue, DependencyProperty sourceProperty, SelectionChangeTrigger trigger, object triggerSource = null) : base(ev, triggerSource)
#endif
        {
            OldValue = oldValue;
            NewValue = newValue;
            TriggerMethod = trigger;
            TriggerSource = triggerSource;
            Property = sourceProperty;
        }


        /// <summary>
        /// The old value of the IsSelected property.
        /// </summary>
        public bool OldValue { get; private set; }
        /// <summary>
        /// The new value of the IsSelected property.
        /// </summary>
        public bool NewValue { get; private set; }
        /// <summary>
        /// The method that was used to update the value.
        /// </summary>
        public SelectionChangeTrigger TriggerMethod { get; private set; }

        /// <summary>
        /// The object that caused the update to occur, if available.
        /// </summary>
#if NETCOREAPP
        public object? TriggerSource { get; private set; }
#else
        public object TriggerSource { get; private set; }
#endif

        /// <summary>
        /// The dependency property related to this event, if applicable.
        /// </summary>
#if NETCOREAPP
        public DependencyProperty? Property { get; private set; } = null;
#else
        public DependencyProperty Property { get; private set; } = null;
#endif

    }

    /// <summary>
    /// Indicates which method or source triggered the change in selection.
    /// </summary>
    public enum SelectionChangeTrigger
    {
        /// <summary>
        /// The selection was changed due to the control itself being clicked.
        /// </summary>
        ControlClick = 0,
        /// <summary>
        /// The selection was changed due to a checkbox in the control being clicked.
        /// </summary>
        CheckBox = 1,
        /// <summary>
        /// The selection was changed due to an action by the parent object containing the control.
        /// </summary>
        Parent = 2,
        /// <summary>
        /// The selection was changed because the <see cref="SelectableUserControl.CanSelect"/> property (or similar property) was changed.
        /// </summary>
        DisableSelecting = 3,
        /// <summary>
        /// The selection was changed via directly setting the value in code, or via a different undefined method.
        /// </summary>
        CodeUnknown = 9,
    }

    /// <summary>
    /// An interface of common properties and methods for Solid Shine UI's button controls.
    /// </summary>
    public interface ISsuiButton : IClickSelectableControl
    {
        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the control when the control is disabled.
        /// </summary>
        Brush DisabledBrush { get; set; }

        /// <summary>
        /// Get or set the brush used for the border around the edges of the control.
        /// </summary>
        Brush BorderBrush { get; set; }

        /// <summary>
        /// Get or set the brush used for the border of the control when the control is disabled.
        /// </summary>
        Brush BorderDisabledBrush { get; set; }

        /// <summary>
        /// Get or set the brush used for the border while the control has the mouse over it, or it has keyboard focus.
        /// </summary>
        Brush BorderHighlightBrush { get; set; }

        /// <summary>
        /// Get or set the brush used for the foreground while the control has the mouse over it, or it has keyboard focus.
        /// </summary>
        Brush ForegroundHighlightBrush { get; set; }

        /// <summary>
        /// Get or set the brush used for the border while the control is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        Brush BorderSelectedBrush { get; set; }

        #endregion

        /// <summary>
        /// Get or set whether the button should have a transparent background when the button is not focused or selected.
        /// </summary>
        bool TransparentBack { get; set; }

        /// <summary>
        /// Get or set the thickness of the border around the button.
        /// </summary>
        Thickness BorderThickness { get; set; }

        /// <summary>
        /// Get or set the thickness of the border around the button, while the button is in a selected (<c>IsSelected</c>) state.
        /// </summary>
        Thickness BorderSelectionThickness { get; set; }

        /// <summary>
        /// Get or set the corner radius (or radii) to use for the button and its border. Can be used to create a rounded button.
        /// </summary>
        CornerRadius CornerRadius { get; set; }

        /// <summary>
        /// Get or set if the button should be highlighted (using the <see cref="IClickSelectableControl.HighlightBrush"/> and <see cref="BorderHighlightBrush"/>)
        /// when it has keyboard focus. If <c>false</c>, only the keyboard focus outline appears, and highlighting only occurs on mouse/stylus over.
        /// </summary>
        bool HighlightOnKeyboardFocus { get; set; }

        /// <summary>
        /// Update the value to a dependency property of this control.
        /// </summary>
        /// <param name="dp">the property to update</param>
        /// <param name="value">the value to update the property to</param>
        void SetValue(DependencyProperty dp, object value);
    }

}
