using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Interactivity;

namespace SolidShineUi
{

    /// <summary>
    /// Event arguments for when a property is changed and causes a routed event to be raised.
    /// </summary>
    public class RoutedPropertyChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Create a RoutedSelectionChangedEventArgs.
        /// </summary>
        /// <param name="eventName">the routed event that was raised</param>
        /// <param name="changeArgs">the arguments for the property change that caused the event to be changed</param>
        public RoutedPropertyChangedEventArgs(RoutedEvent eventName, AvaloniaPropertyChangedEventArgs changeArgs) : base(eventName)
        {
            PropertyChange = changeArgs;
        }

        /// <summary>
        /// Create a RoutedSelectionChangedEventArgs.
        /// </summary>
        /// <param name="eventName">the routed event that was raised</param>
        /// <param name="source">an alternate source for this event, rather than the object that raised it</param>
        /// <param name="changeArgs">the arguments for the property change that caused the event to be changed</param>
        public RoutedPropertyChangedEventArgs(RoutedEvent eventName, object source, AvaloniaPropertyChangedEventArgs changeArgs) : base(eventName, source)
        {
            PropertyChange = changeArgs;
        }

        /// <summary>
        /// Get event argument information about the property that has changed.
        /// </summary>
        public AvaloniaPropertyChangedEventArgs PropertyChange { get; private set; }
    }

    /// <summary>
    /// A delegate for handling an event with <see cref="RoutedPropertyChangedEventArgs"/> as the event arguments.
    /// </summary>
    /// <param name="sender">the sender object of the event</param>
    /// <param name="e">the event arguments associated with the event</param>
    public delegate void RoutedPropertyChangedEventHandler(object?  sender, RoutedPropertyChangedEventArgs e);
}
