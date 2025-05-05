using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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

        #region Extra Control Properties

        //// these are properties I'm expecting all implementers to already have, since they're probably implementing this on a
        //// class inheriting from FrameworkElement - however, I don't want to introduce too many properties, thouugh, because
        //// I'd rather not bog down this interface too much, and instead point users to instead explicitly cast to the element
        //// that they're wanting to use. still, I can implement a couple common/useful things here

        ///// <summary>
        ///// Gets or sets the user interface (UI) visibility of this element.
        ///// </summary>
        //Visibility Visibility { get; set; }

        ///// <summary>
        ///// Gets or sets the opacity factor applied to the entire UIElement when it is rendered in the user interface (UI).
        ///// 0.0 is completely transparent, 1.0 is completely opaque.
        ///// </summary>
        //double Opacity { get; set; }

        ///// <summary>
        ///// Get or set if this element is visible for hit testing.
        ///// </summary>
        //bool IsHitTestVisible { get; set; }

        ///// <summary>
        ///// Gets or sets a value that determines whether rendering for this element should use device-specific pixel settings during rendering.
        ///// </summary>
        //bool SnapsToDevicePixels { get; set; }

        ///// <summary>
        ///// Get or set the margins of this element, indicating how distant it is from the bounds of the parent control.
        ///// </summary>
        //Thickness Margin { get; set; }

        ///// <summary>
        ///// Gets or sets the suggested height of the element.
        ///// </summary>
        //double Height { get; set; }

        ///// <summary>
        ///// Gets or sets the suggested width of the element.
        ///// </summary>
        //double Width { get; set; }

        ///// <summary>
        ///// Get or set how this element is aligned along the horizontal axis (X-axis).
        ///// </summary>
        //HorizontalAlignment HorizontalAlignment { get; set; }

        ///// <summary>
        ///// Get or set how this element is aligned along the vertical axis (Y-axis).
        ///// </summary>
        //VerticalAlignment VerticalAlignment { get; set; }

        ///// <summary>
        ///// Gets or sets the minimum height constraint of the element.
        ///// </summary>
        //double MinHeight { get; set; }

        ///// <summary>
        ///// Gets or sets the minimum width constraint of the element.
        ///// </summary>
        //double MinWidth { get; set; }

        ///// <summary>
        ///// Gets or sets the maximum height constraint of the element.
        ///// </summary>
        //double MaxHeight { get; set; }

        ///// <summary>
        ///// Gets or sets the maximum width constraint of the element.
        ///// </summary>
        //double MaxWidth { get; set; }

        ///// <summary>
        ///// Gets or sets an arbitrary object value that can be used to store custom information about this element. This may return null if no object is set.
        ///// </summary>
        //object Tag { get; set; }

        ///// <summary>
        ///// Gets or sets a value indicating whether this element is enabled in the user interface (UI).
        ///// </summary>
        //new bool IsEnabled { get; set; }

        ///// <summary>
        ///// Get if this element currently has logical focus. Use <see cref="IInputElement.IsKeyboardFocused"/> to check if this has keyboard focus.
        ///// </summary>
        //bool IsFocused { get; }

        ///// <summary>
        ///// Gets or sets transform information that affects the rendering position of this element.
        ///// </summary>
        //Transform RenderTransform { get; set; }

        ///// <summary>
        ///// Gets or sets a graphics transformation that should apply to this element when layout is performed.
        ///// </summary>
        //Transform LayoutTransform { get; set; }

        ///// <summary>
        ///// Gets or sets the tool-tip object that is displayed for this element in the user interface (UI).
        ///// </summary>
        //object ToolTip { get; set; }

        //// DependencyObject methods/properties

        ///// <summary>
        ///// Gets the Dispatcher this DispatcherObject is associated with.
        ///// </summary>
        //Dispatcher Dispatcher { get; }

        ///// <summary>
        ///// Gets a value that indicates whether this instance is currently sealed (read-only).
        ///// </summary>
        //bool IsSealed { get; }

        ///// <summary>
        ///// Clears the local value of a property. The property to be cleared is specified by a DependencyProperty identifier.
        ///// </summary>
        //void ClearValue(DependencyProperty dp);

        ///// <summary>
        ///// Coerces the value of the specified dependency property. This is accomplished by invoking any CoerceValueCallback 
        ///// function specified in property metadata for the dependency property as it exists on the calling DependencyObject.
        ///// </summary>
        //void CoerceValue(DependencyProperty dp);

        ///// <summary>
        ///// Creates a specialized enumerator for determining which dependency properties have locally set values on this DependencyObject.
        ///// </summary>
        //LocalValueEnumerator GetLocalValueEnumerator();

        ///// <summary>
        ///// Returns the current effective value of a dependency property on this instance of a DependencyObject.
        ///// </summary>
        //object GetValue(DependencyProperty dp);

        ///// <summary>
        ///// Re-evaluates the effective value for the specified dependency property.
        ///// </summary>
        //void InvalidateProperty(DependencyProperty dp);

        ///// <summary>
        ///// Sets the local value of a dependency property, specified by its dependency property identifier.
        ///// </summary>
        //void SetValue(DependencyProperty dp, object value);

        #endregion

    }


    /// <summary>
    /// Represents a handler for the <see cref="IClickSelectableControl.IsSelectedChanged"/> event.
    /// </summary>
    /// <param name="sender">The source object of the event.</param>
    /// <param name="e">The event arguments, containing information on the new IsSelected value and how the selection changed.</param>
    public delegate void ItemSelectionChangedEventHandler(object sender, ItemSelectionChangedEventArgs e);

//    /// <summary>
//    /// The event arguments for the IsSelectedChanged event of the SelectableUserControl.
//    /// </summary>
//    public class ItemSelectionChangedEventArgs : RoutedEventArgs
//    {

//        /// <summary>
//        /// Create a new ItemSelectionChangedRoutedEventArgs.
//        /// </summary>
//        /// <param name="ev">the routed event associated with these routed event args</param>
//        /// <param name="oldValue">The old IsSelected value.</param>
//        /// <param name="newValue">The new IsSelected value.</param>
//        /// <param name="trigger">The trigger method that caused the value to be updated.</param>
//        /// <param name="triggerSource">The source object that updated the value (if available).</param>
//#if NETCOREAPP
//        public ItemSelectionChangedEventArgs(RoutedEvent ev, bool oldValue, bool newValue, SelectionChangeTrigger trigger, object? triggerSource = null) : base(ev, triggerSource)
//#else
//        public ItemSelectionChangedEventArgs(RoutedEvent ev, bool oldValue, bool newValue, SelectionChangeTrigger trigger, object triggerSource = null) : base(ev, triggerSource)
//#endif
//        {
//            OldValue = oldValue;
//            NewValue = newValue;
//            TriggerMethod = trigger;
//            TriggerSource = triggerSource;
//        }


//        /// <summary>
//        /// The old value of the IsSelected property.
//        /// </summary>
//        public bool OldValue { get; private set; }
//        /// <summary>
//        /// The new value of the IsSelected property.
//        /// </summary>
//        public bool NewValue { get; private set; }
//        /// <summary>
//        /// The method that was used to update the value.
//        /// </summary>
//        public SelectionChangeTrigger TriggerMethod { get; private set; }

//        /// <summary>
//        /// The object that caused the update to occur, if available.
//        /// </summary>
//#if NETCOREAPP
//        public object? TriggerSource { get; private set; }
//#else
//        public object TriggerSource { get; private set; }
//#endif
//    }

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

}