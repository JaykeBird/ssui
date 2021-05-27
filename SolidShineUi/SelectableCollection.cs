using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi
{
    public class SelectableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Initializes a new SelectableCollection&lt;<typeparamref name="T"/>&gt;.
        /// </summary>
        public SelectableCollection() : base()
        {
            CollectionChanged += this_CollectionChanged;
        }

        /// <summary>
        /// Initializes a new SelectableCollection&lt;<typeparamref name="T"/>&gt; that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection to copy from.</param>
        /// <exception cref="ArgumentNullException">Thrown if the collection is null.</exception>
        public SelectableCollection(IEnumerable<T> collection) : base(collection)
        {
            CollectionChanged += this_CollectionChanged;
        }

        /// <summary>
        /// Initializes a new SelectableCollection&lt;<typeparamref name="T"/>&gt; that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list">The list to copy from.</param>
        /// <exception cref="ArgumentNullException">Thrown if the list is null.</exception>
        public SelectableCollection(IList<T> list) : base(list)
        {
            CollectionChanged += this_CollectionChanged;
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
        public void Select(T item)
        {
            if (Contains(item))
            {
                List<T> old = selectedItems;
                selectedItems = new List<T>() { item };
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(old, selectedItems));
            }
        }

        /// <summary>
        /// Add an item to the existing list of selected items. If <c>CanSelectMultiple</c> is false, this item is only selected if nothing else is selected. Otherwise, nothing happens.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddToSelection(T item)
        {
            if (CanSelectMultiple)
            {
                selectedItems.Add(item);
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(), new List<T> { item }));
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
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(old, selectedItems));
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

                    selectedItems = new List<T>(1) { item };

                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(old, new List<T>()));
                }
            }
        }

        /// <summary>
        /// Select a collection of items. If <c>CanSelectMultiple</c> is false, only the first item in the collection is selected. If the collection is empty, the selection is not changed.
        /// </summary>
        /// <param name="items">The items to select.</param>
        public void SelectRange(IEnumerable<T> items)
        {
            List<T> sel = new List<T>();

            if (!items.Any())
            {
                // select none
                //ClearSelection();
                return;
            }

            if (!CanSelectMultiple)
            {
                if (Contains(items.First()))
                {
                    List<T> old = selectedItems;
                    selectedItems = new List<T>() { items.First() };
                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(old, selectedItems));
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
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(old, selectedItems));
            }
        }

        public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs<T> e);

        /// <summary>
        /// Raised when the selection is changed in any way, including additions, removals, and the selection being cleared.
        /// </summary>
#if NETCOREAPP
        public event SelectionChangedEventHandler? SelectionChanged;
#else
        public event SelectionChangedEventHandler SelectionChanged;
#endif
    }

    public class SelectionChangedEventArgs<T>
    {
        public SelectionChangedEventArgs(IList<T> removedItems, IList<T> addedItems)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        public IList<T> AddedItems { get; private set; }
        public IList<T> RemovedItems { get; private set; }
    }
}
