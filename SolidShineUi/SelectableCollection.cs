using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SolidShineUi.Utils;
using static SolidShineUi.CollectionSelectionChangedEventArgs;

namespace SolidShineUi
{
    /// <summary>
    /// Represents a dynamic data collection, which provides notifications as items are added or removed, and which items can be marked as "selected".
    /// This is ideal for scenarios where you're working with a list or collection of objects, and want the ability to only affect any arbitrary subset of these objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class SelectableCollection<T> : ObservableCollection<T>, ISelectableCollection<T>, ISelectableCollection
    {
        /// <summary>
        /// Initializes a new SelectableCollection.
        /// </summary>
        public SelectableCollection() : base()
        {
            CollectionChanged += this_CollectionChanged;
        }

        /// <summary>
        /// Initializes a new SelectableCollection that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection to copy from.</param>
        /// <exception cref="ArgumentNullException">Thrown if the collection is null.</exception>
        public SelectableCollection(IEnumerable<T> collection) : base(collection)
        {
            CollectionChanged += this_CollectionChanged;
        }

        /// <summary>
        /// Initializes a new SelectableCollection that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list">The list to copy from.</param>
        /// <exception cref="ArgumentNullException">Thrown if the list is null.</exception>
        public SelectableCollection(IList<T> list) : base(list)
        {
            CollectionChanged += this_CollectionChanged;
        }

        /// <summary>
        /// Represents a handler for the ItemRemoving event.
        /// </summary>
        /// <param name="sender">The source object of the event.</param>
        /// <param name="e">The event arguments, containing the item being removed and the ability to cancel the removal.</param>
        public delegate void ItemRemovingEventHandler(object sender, ItemRemovingEventArgs<T> e);

        /// <summary>
        /// Raised before an item is removed, to give the ability to cancel removing this item.
        /// </summary>
#if NETCOREAPP
        public event ItemRemovingEventHandler? ItemRemoving;
#else
        public event ItemRemovingEventHandler ItemRemoving;
#endif

        /// <summary>
        /// Represents a handler for the ItemAdding event.
        /// </summary>
        /// <param name="sender">The source object of the event.</param>
        /// <param name="e">The event arguments, containing the item being added and the ability to cancel the addition.</param>
        public delegate void ItemAddingEventHandler(object sender, ItemAddingEventArgs<T> e);

        /// <summary>
        /// Raised before an item is added, to give the ability to cancel adding this item.
        /// </summary>
#if NETCOREAPP
        public event ItemAddingEventHandler? ItemAdding;
#else
        public event ItemAddingEventHandler ItemAdding;
#endif

        /// <summary>
        /// Add an item to the end of this collection.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        public new void Add(T item)
        {
            //if (Contains(item)) return;
            ItemAddingEventArgs<T> e = new ItemAddingEventArgs<T>(item);
            ItemAdding?.Invoke(this, e);
            if (e.Cancel) return;
            base.Add(item);
        }

        /// <summary>
        /// Insert an item into this collection at the specified index. 
        /// </summary>
        /// <param name="index">The zero-based index at which to add the item.</param>
        /// <param name="item">The item to add to the collection.</param>
        public new void Insert(int index, T item)
        {
            //if (Contains(item)) return;
            ItemAddingEventArgs<T> e = new ItemAddingEventArgs<T>(item);
            ItemAdding?.Invoke(this, e);
            if (e.Cancel) return;
            base.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object in the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><c>true</c> if the item is removed, otherwise <c>false</c>. It may be <c>false</c> if the item isn't actually in the collection or if the removal was cancelled via the ItemRemoving event.</returns>
        public new bool Remove(T item)
        {
            ItemRemovingEventArgs<T> te = new ItemRemovingEventArgs<T>(item);

            ItemRemoving?.Invoke(this, te);

            if (te.Cancel) return false;

            bool res = base.Remove(item);

            if (res && selectedItems.Contains(item)) selectedItems.Remove(item);

            return res;
        }

        /// <summary>
        /// Remove the item at the specified index of this collection.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns><c>true</c> if the item is removed, otherwise <c>false</c>. It will be <c>false</c> if the removal was cancelled via the ItemRemoving event.</returns>
        public new bool RemoveAt(int index)
        {
            T t = this[index];

            ItemRemovingEventArgs<T> te = new ItemRemovingEventArgs<T>(t);

            ItemRemoving?.Invoke(this, te);

            if (te.Cancel) return false;

            base.RemoveAt(index);

            return true;
        }

        /// <summary>
        /// Remove the item at the specified index of this collection.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns><c>true</c> if the item is removed, otherwise <c>false</c>. It will be <c>false</c> if the removal was cancelled via the ItemRemoving event.</returns>
        public new bool RemoveItem(int index)
        {
            T t = this[index];

            ItemRemovingEventArgs<T> te = new ItemRemovingEventArgs<T>(t);

            ItemRemoving?.Invoke(this, te);

            if (te.Cancel) return false;

            base.RemoveItem(index);

            return true;
        }

#if NETCOREAPP
        private void this_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        private void this_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#endif
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    return;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    List<T> removedItems = new List<T>();
                    if (e.OldItems != null)
                    {
                        foreach (T item in e.OldItems)
                        {
                            if (item != null)
                            {
                                if (selectedItems.Contains(item))
                                {
                                    selectedItems.Remove(item);
                                    removedItems.Add(item);
                                }
                            }
                        }
                    }
                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(removedItems, new List<T>()));
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    List<T> removedItemsR = new List<T>();
                    if (e.OldItems != null)
                    {
                        foreach (T item in e.OldItems)
                        {
                            if (item != null)
                            {
                                if (selectedItems.Contains(item))
                                {
                                    selectedItems.Remove(item);
                                    removedItemsR.Add(item);
                                }
                            }
                        }
                    }
                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(removedItemsR, new List<T>()));
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    return;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    ClearSelection();
                    break;
                default:
                    return;
            }
        }


        private List<T> selectedItems = new List<T>();

        /// <summary>
        /// A collection of items that are currently selected (if no items are selected, the collection is empty). If <c>CanSelectMultiple</c> is false, this collection will only have 0 or 1 items.
        /// </summary>
        /// <remarks>To interact with the collection to add or remove items, use the methods of the SelectableCollection itself, such as Select, AddToSelection, Deselect, and ClearSelection.</remarks>
        public ReadOnlyCollection<T> SelectedItems { get => selectedItems.AsReadOnly(); }

        /// <summary>
        /// Select an item, replacing the current selection.
        /// </summary>
        /// <param name="item">The item to select.</param>
        /// <remarks>If <c>CanSelectMultiple</c> is true, use <see cref="AddToSelection(T)"/> to add more items to the selected list,
        /// or <see cref="SelectRange(IEnumerable{T})"/> to select multiple items at once.
        /// This function will always replace whatever is currently selected.</remarks>
        public void Select(T item)
        {
            if (Contains(item))
            {
                List<T> old = selectedItems;
                selectedItems = new List<T>() { item };
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(old), new List<T>(selectedItems)));
            }
        }

        /// <summary>
        /// Add an item to the existing list of selected items. If <c>CanSelectMultiple</c> is false, this item is only selected if nothing else is selected. Otherwise, nothing happens.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <remarks>This function is a complement to <see cref="AddToOrReplaceSelection(T)"/>. 
        /// <c>AddToSelection</c> should be used if preserving the existing selection is more important while <c>CanSelectMultiple</c> is false.
        /// <c>AddToOrReplaceSelection</c> should be used if selecting the item in the parameter is more important while <c>CanSelectMultiple</c> is false.
        /// While <c>CanSelectMultiple</c> is true, the two functions are identical.</remarks>
        public void AddToSelection(T item)
        {
            if (CanSelectMultiple)
            {
                if (!selectedItems.Contains(item))
                {
                    selectedItems.Add(item);
                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(), new List<T> { item }));
                }
            }
            else
            {
                if (selectedItems.Count == 0)
                {
                    Select(item);
                }
            }
        }

        /// <summary>
        /// Add an item to the existing list of selected items. If <c>CanSelectMultiple</c> is false, this item is selected, replacing the previous selection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <remarks>This function is a complement to <see cref="AddToSelection(T)"/>. 
        /// <c>AddToSelection</c> should be used if preserving the existing selection is more important while <c>CanSelectMultiple</c> is false.
        /// <c>AddToOrReplaceSelection</c> should be used if selecting the item in the parameter is more important while <c>CanSelectMultiple</c> is false.
        /// While <c>CanSelectMultiple</c> is true, the two functions are identical.</remarks>
        public void AddToOrReplaceSelection(T item)
        {
            if (CanSelectMultiple)
            {
                if (!selectedItems.Contains(item))
                {
                    selectedItems.Add(item);
                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(), new List<T> { item }));
                }
            }
            else
            {
                Select(item);
            }
        }

        /// <summary>
        /// Remove an item from the list of selected items.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void Deselect(T item)
        {
            if (selectedItems.Contains(item))
            {
                selectedItems.Remove(item);
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T> { item }, new List<T>()));
            }
        }

        /// <summary>
        /// Clear the list of selected items. No items will be selected.
        /// </summary>
        public void ClearSelection()
        {
            List<T> old = selectedItems;
            selectedItems = new List<T>();
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(old), new List<T>(selectedItems)));
        }

        /// <summary>
        /// Check if an item is currently selected. Only returns true if the item is in this SelectableCollection and is selected; otherwise, this will return false.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is in this SelectableCollection and is selected; otherwise, false.</returns>
        public bool IsSelected(T item)
        {
            return selectedItems.Contains(item);
        }

        private bool multiSelect = false;

        /// <summary>
        /// Get or set if multiple items can be selected in this SelectableCollection.
        /// </summary>
        /// <remarks>If CanSelectMultiple is set to false while there's currently more than 1 item selected, all items except the first are removed from the selection.</remarks>
        public bool CanSelectMultiple
        {
            get => multiSelect;
            set
            {
                multiSelect = value;
                if (!multiSelect && selectedItems.Count > 1)
                {
                    List<T> old = selectedItems;
                    T item = selectedItems[0];
                    old.RemoveAt(0);

                    selectedItems = new List<T>() { item };

                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(old), new List<T>()));
                }
            }
        }

        /// <summary>
        /// Select a collection of items, replacing the current selection. If <c>CanSelectMultiple</c> is false, only the first item in the collection is selected. If the collection is empty, the selection is not changed.
        /// </summary>
        /// <param name="items">The items to select.</param>
        /// <remarks>This function will always replace whatever is currently selected EXCEPT in the situation that the <paramref name="items"/> parameter is an empty collection.
        /// If <c>CanSelectMultiple</c> is true, use <see cref="AddToSelection(T)"/> to add to the existing list of selected items, rather than replacing it.</remarks>
        public void SelectRange(IEnumerable<T> items)
        {
            List<T> sel = new List<T>();

            if (!items.Any())
            {
                // do nothing
                //ClearSelection();
                return;
            }

            if (!CanSelectMultiple)
            {
                if (Contains(items.First()))
                {
                    List<T> old = selectedItems;
                    selectedItems = new List<T>() { items.First() };
                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(old), new List<T>(selectedItems)));
                    return;
                }
            }

            foreach (T item in items)
            {
                if (Contains(item))
                {
                    sel.Add(item);
                }
            }

            if (sel.Count > 0)
            {
                List<T> old = selectedItems;
                selectedItems = sel;
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(old), new List<T>(selectedItems)));
            }
        }

        /// <summary>
        /// Select all items in the collection. If <c>CanSelectMultiple</c> is false, only the first item in the collection is selected.
        /// </summary>
        public void SelectAll()
        {
            SelectRange(Items);
        }

        #region Non-generic ISelectableCollectionSource handling
        ICollection ISelectableCollection.SelectedItems => SelectedItems;

        void ISelectableCollection.AddToSelection(object item)
        {
            if (item is T)
            {
                AddToSelection((T)item);
            }
        }

        void ISelectableCollection.Select(object item)
        {
            if (item is T)
            {
                Select((T)item);
            }
        }

        void ISelectableCollection.Deselect(object item)
        {
            if (item is T)
            {
                Deselect((T)item);
            }
        }

        bool ISelectableCollection.IsSelected(object item)
        {
            if (item is T)
            {
                return IsSelected((T)item);
            }
            else
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// Raised when the selection is changed in any way, including additions, removals, and the selection being cleared.
        /// </summary>
#if NETCOREAPP
        public event SelectionChangedEventHandler? SelectionChanged;
#else
        public event SelectionChangedEventHandler SelectionChanged;
#endif

    }

    /// <summary>
    /// Event arguments for an ItemRemoving event. This is used for when an item is about to be removed.
    /// </summary>
    /// <typeparam name="T">Represents the type of item being removed.</typeparam>
    public class ItemRemovingEventArgs<T>
    {
        /// <summary>
        /// Create an ItemRemovingEventArgs.
        /// </summary>
        /// <param name="item">The item being removed.</param>
        public ItemRemovingEventArgs(T item)
        {
            Item = item;
        }

        /// <summary>
        /// The item being removed.
        /// </summary>
        public T Item { get; private set; }

        /// <summary>
        /// Get or set if the removal of this item should be cancelled. If true, the item will not be removed.
        /// </summary>
        public bool Cancel { get; set; } = false;
    }

    /// <summary>
    /// Event arguments for an ItemAdding event. This is used for when an item is about to be added.
    /// </summary>
    /// <typeparam name="T">Represents the type of item being added.</typeparam>
    public class ItemAddingEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The item being added.
        /// </summary>
        public T Item { get; private set; }

        /// <summary>
        /// Get or set if the addition of this item should be cancelled. If true, the item will not be added.
        /// </summary>
        public bool Cancel { get; set; } = false;

        /// <summary>
        /// Create an ItemAddingEventArgs.
        /// </summary>
        /// <param name="item">The item being added.</param>
        public ItemAddingEventArgs(T item)
        {
            Item = item;
        }
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
}
