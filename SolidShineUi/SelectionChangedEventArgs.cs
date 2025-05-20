using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace SolidShineUi
{
    /// <summary>
    /// Represents a handler for the SelectionChanged event.
    /// </summary>
    /// <param name="sender">The source object of the event.</param>
    /// <param name="e">The event arguments, containing the list of items being added or removed from the selection.</param>
    public delegate void SelectionChangedEventHandler(object sender, CollectionSelectionChangedEventArgs e);

    /// <summary>
    /// Represents a handler for the SelectionChanged event, with a routed event.
    /// </summary>
    /// <param name="sender">The source object of the event.</param>
    /// <param name="e">The event arguments, containing the list of items being added or removed from the selection, and the routed event in question.</param>
    public delegate void RoutedSelectionChangedEventHandler<T>(object sender, RoutedSelectionChangedEventArgs<T> e);

    /// <summary>
    /// A non-generic version of SelectionChangedEventArgs. This can be more generally used in other situations, and implemented by ISelectableCollection.
    /// </summary>
    public class CollectionSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Create a SelectionChangedEventArgs.
        /// </summary>
        /// <param name="removedItems">The list of items to be removed.</param>
        /// <param name="addedItems">The list of items to be added.</param>
        public CollectionSelectionChangedEventArgs(ICollection removedItems, ICollection addedItems)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        /// <summary>
        /// The list of items being added to the selection ("selected").
        /// </summary>
        public ICollection AddedItems { get; private set; }
        /// <summary>
        /// The list of items being removed from the selection ("deselected").
        /// </summary>
        public ICollection RemovedItems { get; private set; }

    }

    /// <summary>
    /// Event arguments for when the current selection of a SelectableCollection is changed.
    /// </summary>
    /// <typeparam name="T">Represents the type of item in the collection.</typeparam>
    public class SelectionChangedEventArgs<T> : CollectionSelectionChangedEventArgs
    {
        /// <summary>
        /// Create a SelectionChangedEventArgs.
        /// </summary>
        /// <param name="removedItems">A list of items being removed.</param>
        /// <param name="addedItems">A list of item being added.</param>
        public SelectionChangedEventArgs(List<T> removedItems, List<T> addedItems) : base(removedItems, addedItems)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        /// <summary>
        /// The list of items being added to the selection ("selected").
        /// </summary>
        public new List<T> AddedItems { get; private set; }
        /// <summary>
        /// The list of items being removed from the selection ("deselected").
        /// </summary>
        public new List<T> RemovedItems { get; private set; }
    }

    /// <summary>
    /// Event arguments for when the current selection of a SelectableCollection is changed via a routed event.
    /// </summary>
    /// <typeparam name="T">Represents the type of item in the collection.</typeparam>
    public class RoutedSelectionChangedEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Create a RoutedSelectionChangedEventArgs.
        /// </summary>
        /// <param name="removedItems">A list of items being removed.</param>
        /// <param name="addedItems">A list of item being added.</param>
        /// <param name="eventName">The routed event that was raised.</param>
        public RoutedSelectionChangedEventArgs(RoutedEvent eventName, List<T> removedItems, List<T> addedItems) : base(eventName)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        /// <summary>
        /// Create a RoutedSelectionChangedEventArgs.
        /// </summary>
        /// <param name="removedItems">A list of items being removed.</param>
        /// <param name="addedItems">A list of item being added.</param>
        /// <param name="eventName">The routed event that was raised.</param>
        /// <param name="source">An alternate source for this event, rather than the object that raised it</param>
        public RoutedSelectionChangedEventArgs(RoutedEvent eventName, object source, List<T> removedItems, List<T> addedItems) : base(eventName, source)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        /// <summary>
        /// The list of items being added to the selection ("selected").
        /// </summary>
        public List<T> AddedItems { get; private set; }
        /// <summary>
        /// The list of items being removed from the selection ("deselected").
        /// </summary>
        public List<T> RemovedItems { get; private set; }
    }
}
