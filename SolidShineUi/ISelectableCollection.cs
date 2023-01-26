using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SolidShineUi
{
    /// <summary>
    /// Defines a generic interface for a collection where items within it can be selected.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface ISelectableCollectionSource<T>
    {
        /// <summary>
        /// Get a list of currently selected items.
        /// </summary>
        ReadOnlyCollection<T> SelectedItems { get; }

        /// <summary>
        /// Add an item to the existing list of selected items.
        /// </summary>
        /// <param name="item">The item to add to the selection.</param>
        void AddToSelection(T item);

        /// <summary>
        /// Select an item, replacing the current selection.
        /// </summary>
        /// <param name="item">The item to select.</param>
        void Select(T item);

        /// <summary>
        /// Remove an item from the list of selected items.
        /// </summary>
        /// <param name="item">The item to remove from the selection.</param>
        void Deselect(T item);

        /// <summary>
        /// Select all items in the collection.
        /// </summary>
        void SelectAll();

        /// <summary>
        /// Clear the list of selected items. No items will be selected.
        /// </summary>
        void ClearSelection();

        /// <summary>
        /// Check if an item is currently selected. Only returns true if the item is in this
        /// collection, and is currently selected; otherwise, this always returns false.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is in this colelction and is selected; otherwise, false.</returns>
        bool IsSelected(T item);

        /// <summary>
        /// Select a collection of items, replacing the current selection.
        /// Only items currently in the collection can be selected.
        /// </summary>
        /// <param name="items">The items to select.</param>
        void SelectRange(IEnumerable<T> items);

        /// <summary>
        /// Get or set if multiple items can be selected in this collection.
        /// </summary>
        /// <remarks>
        /// Some implementers may not allow this value to be changed via a setter; if so, a <see cref="NotSupportedException"/> will be thrown.
        /// </remarks>
        bool CanSelectMultiple { get; set; }

        /// <summary>
        /// Raised when the selection is changed, including additions and removals.
        /// </summary>
#if NETCOREAPP
        event CollectionSelectionChangedEventArgs.SelectionChangedEventHandler? SelectionChanged;
#else
        event CollectionSelectionChangedEventArgs.SelectionChangedEventHandler SelectionChanged;
#endif

    }

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
        public CollectionSelectionChangedEventArgs(IEnumerable removedItems, IEnumerable addedItems)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        /// <summary>
        /// The list of items being added to the selection ("selected").
        /// </summary>
        public IEnumerable AddedItems { get; private set; }
        /// <summary>
        /// The list of items being removed from the selection ("deselected").
        /// </summary>
        public IEnumerable RemovedItems { get; private set; }

        /// <summary>
        /// Represents a handler for the SelectionChanged event.
        /// </summary>
        /// <param name="sender">The source object of the event.</param>
        /// <param name="e">The event arguments, containing the list of items being added or removed from the selection.</param>
        public delegate void SelectionChangedEventHandler(object sender, CollectionSelectionChangedEventArgs e);
    }
}
