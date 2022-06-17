using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Linq;
using System.Collections.Specialized;

namespace SolidShineUi
{
    /// <summary>
    /// A type of CollectionView that operates as a SelectableCollection. This can be used in SelectPanel.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public class SelectableCollectionViewSource<T> : CollectionViewSource, ISelectableCollectionSource<T>
    {
        /// <summary>
        /// Create a SelectableCollectionView, that represents a view of the specified collection.
        /// </summary>
        /// <param name="collection">The collection that is represented in this view.</param>
        public SelectableCollectionViewSource(IEnumerable<T> collection) : base()
        {
            Source = collection;
            baseCollection = collection;

            //if (collection is IEnumerable<T>)
            //{
            //}
            //else
            //{
            //    throw new ArgumentException("The collection inputted must be of the same type as the generic type defined for this SelectableCollectionView.");
            //}

            if (baseCollection is INotifyCollectionChanged inn)
            {
                inn.CollectionChanged += baseCol_CollectionChanged;
            } // else... welp lol
        }

        private void baseCol_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    return;
                case NotifyCollectionChangedAction.Remove:
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
                case NotifyCollectionChangedAction.Replace:
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
                case NotifyCollectionChangedAction.Move:
                    return;
                case NotifyCollectionChangedAction.Reset:
                    ClearSelection();
                    break;
                default:
                    return;
            }
        }

        private IEnumerable<T> baseCollection;

        private List<T> selectedItems = new List<T>();

        public event CollectionSelectionChangedEventArgs.SelectionChangedEventHandler? SelectionChanged;

        public ReadOnlyCollection<T> SelectedItems => selectedItems.AsReadOnly();

        /// <summary>
        /// Add an item to the existing list of selected items.
        /// </summary>
        /// <param name="item">The item to select.</param>
        public void AddToSelection(T item)
        {
            if (baseCollection.Contains(item))
            {
                selectedItems.Add(item);
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(), new List<T> { item }));
            }
        }

        /// <summary>
        /// Select an item, replacing the current list of selected items.
        /// </summary>
        /// <param name="item">The item to select.</param>
        public void Select(T item)
        {
            if (baseCollection.Contains(item))
            {
                List<T> oldSel = selectedItems;

                selectedItems = new List<T>(){ item };
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(oldSel, new List<T> { item }));
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
        /// Select all items in the base collection.
        /// </summary>
        public void SelectAll()
        {
            SelectRange(baseCollection);
        }

        /// <summary>
        /// Clear the list of selected items. Nothing will be selected.
        /// </summary>
        public void ClearSelection()
        {
            List<T> old = selectedItems;
            selectedItems = new List<T>();
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(old, selectedItems));
        }

        /// <summary>
        /// Check if an item is currently in the list of selected items.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is currently in the list of selected items; otherwise, false.</returns>
        public bool IsSelected(T item)
        {
            return selectedItems.Contains(item);
        }

        /// <summary>
        /// Select a range of items, replacing the current selection list.
        /// </summary>
        /// <param name="items">The items to select.</param>
        public void SelectRange(IEnumerable<T> items)
        {
            List<T> newSel = new List<T>();
            List<T> oldSel = selectedItems;

            foreach (T item in items)
            {
                if (baseCollection.Contains(item))
                {
                    newSel.Add(item);
                }
            }

            selectedItems = newSel;

            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(oldSel, newSel));
        }
    }
}
