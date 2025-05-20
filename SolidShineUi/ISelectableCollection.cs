using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SolidShineUi
{

    /// <summary>
    /// Defines an interface for a collection where items within it can be selected. This is the non-generic version of <see cref="ISelectableCollection{T}"/>.
    /// </summary>
    public interface ISelectableCollection : IEnumerable
    {
        /// <summary>
        /// Get a list of currently selected items.
        /// </summary>
        ICollection SelectedItems { get; }

        /// <summary>
        /// Add an item to the existing list of selected items.
        /// </summary>
        /// <param name="item">The item to add to the selection.</param>
        /// <remarks>
        /// If <see cref="CanSelectMultiple"/> is false, this function will only succeed if there is currently nothing selected; otherwise, nothing happens.
        /// </remarks>
        void AddToSelection(object item);

        /// <summary>
        /// Select an item, replacing the current selection.
        /// </summary>
        /// <param name="item">The item to select.</param>
        void Select(object item);

        /// <summary>
        /// Remove an item from the list of selected items.
        /// </summary>
        /// <param name="item">The item to remove from the selection.</param>
        void Deselect(object item);

        /// <summary>
        /// Select all items in the collection.
        /// </summary>
        /// <remarks>
        /// If <see cref="CanSelectMultiple"/> is false, either the first item will be selected or nothing will happen.
        /// </remarks>
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
        /// <returns>True if the item is in this collection and is selected; otherwise, false.</returns>
        bool IsSelected(object item);

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
        event SelectionChangedEventHandler? SelectionChanged;
#else
        event SelectionChangedEventHandler SelectionChanged;
#endif
    }

    /// <summary>
    /// Defines a generic interface for a collection where items within it can be selected. This is the generic version of <see cref="ISelectableCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface ISelectableCollection<T> : ICollection<T>
    {
        /// <summary>
        /// Get a list of currently selected items.
        /// </summary>
        ReadOnlyCollection<T> SelectedItems { get; }

        /// <summary>
        /// Add an item to the existing list of selected items (select an item).
        /// </summary>
        /// <param name="item">The item to add to the selection.</param>
        /// <remarks>
        /// If <see cref="CanSelectMultiple"/> is false, this function will only succeed if there is currently nothing selected; otherwise, nothing happens.
        /// </remarks>
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
        /// <remarks>
        /// If <see cref="CanSelectMultiple"/> is false, either the first item will be selected or nothing will happen.
        /// </remarks>
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
        /// <returns>True if the item is in this collection and is selected; otherwise, false.</returns>
        bool IsSelected(T item);

        /// <summary>
        /// Select a collection of items, replacing the current selection.
        /// Only items currently in the collection can be selected.
        /// </summary>
        /// <param name="items">The items to select.</param>
        /// <remarks>
        /// If <see cref="CanSelectMultiple"/> is false, either the first item will be selected or nothing will happen.
        /// </remarks>
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
        event SelectionChangedEventHandler? SelectionChanged;
#else
        event SelectionChangedEventHandler SelectionChanged;
#endif

    }
}
