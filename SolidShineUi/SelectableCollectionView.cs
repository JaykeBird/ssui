using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Linq;
using System.Collections.Specialized;
using System.Collections;

namespace SolidShineUi
{
    /// <summary>
    /// A type of CollectionView that operates as a SelectableCollection. This can be used as a SelectPanel's ItemsSource if <typeparamref name="T"/> derives from SelectableUserControl.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public class SelectableCollectionView<T> : ListCollectionView, ISelectableCollection<T>, ICollection<T>, ISelectableCollection
    {
        /// <summary>
        /// Create a SelectableCollectionView, that represents a view of the specified list.
        /// </summary>
        /// <param name="collection">The collection that is represented in this view.</param>
        public SelectableCollectionView(List<T> collection) : base(collection)
        {
            //Source = collection;
            baseCollection = collection;
            baseEnumerator = this;

            if (baseCollection is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += baseCol_CollectionChanged;
            } // else... welp lol
        }

        /// <summary>
        /// Create a SelectableCollectionView, that represents a view of the specified list.
        /// </summary>
        /// <param name="collection">The collection that is represented in this view.</param>
        /// <exception cref="ArgumentException">Thrown if collection is not a generic IList of type <typeparamref name="T"/> (<c>IList&lt;T&gt;</c>).</exception>
        public SelectableCollectionView(IList collection) : base(collection)
        {
            if (collection is IList<T> tcol)
            {
                baseCollection = tcol;
                baseEnumerator = this;

                if (baseCollection is INotifyCollectionChanged incc)
                {
                    incc.CollectionChanged += baseCol_CollectionChanged;
                } // else... welp lol
            }
            else
            {
                throw new ArgumentException("The collection passed in is not a generic IList of type " + typeof(T).Name + ".", nameof(collection));
            }
        }

#if NETCOREAPP
        private void baseCol_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
#else
        private void baseCol_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
#endif
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
                                if (selectedItems.Remove(item))
                                {
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
                                if (selectedItems.Remove(item))
                                {
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

        private IList<T> baseCollection;

        private IEnumerable baseEnumerator;

        private List<T> selectedItems = new List<T>();

#if NETCOREAPP
        /// <summary>
        /// Raised when the list of selected items is changed.
        /// </summary>
        public event CollectionSelectionChangedEventArgs.SelectionChangedEventHandler? SelectionChanged;
#else
        /// <summary>
        /// Raised when the list of selected items is changed.
        /// </summary>
        public event CollectionSelectionChangedEventArgs.SelectionChangedEventHandler SelectionChanged;
#endif

        /// <summary>
        /// Get a list of currently selected items.
        /// </summary>
        public ReadOnlyCollection<T> SelectedItems => selectedItems.AsReadOnly();

        /// <summary>
        /// Get if multiple items can be selected at once. This property cannot be changed or set to; trying to call the setter will raise a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Raised if the setter for this property is called. This property is implemented as read-only here.</exception>
        public bool CanSelectMultiple { get => true; set { throw new NotSupportedException("The CanSelectMultiple value cannot be changed in a SelectableCollectionView."); } }

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

                selectedItems = new List<T>() { item };
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(oldSel), new List<T> { item }));
            }
        }

        /// <summary>
        /// Remove an item from the list of selected items.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void Deselect(T item)
        {
            if (selectedItems.Remove(item))
            {
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
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(old), new List<T>(selectedItems)));
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

            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(new List<T>(oldSel), new List<T>(newSel)));
        }

        #region ICollection implementations

        /// <summary>
        /// Get the item at the specified index in the underlying list.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        public T this[int index]
        {
            get => (T)base.GetItemAt(index);
            //set
            //{
            //    throw new NotSupportedException("Not supported in a ListCollectionView.");
            //}
        }

        /// <summary>
        /// Get the index of an item in this collection, or -1 if the index is unknown.
        /// </summary>
        /// <param name="item">The index of the item.</param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return base.IndexOf(item);
        }

        /// <summary>
        /// Returns if this collection currently contains the specified item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        public bool Contains(T item)
        {
            return base.Contains(item);
        }

        ///// <summary>
        ///// Copy this collection into an array.
        ///// </summary>
        ///// <param name="array">The array to copy into.</param>
        ///// <param name="arrayIndex">The index in the array to start copying in at.</param>
        //public void CopyTo(T[] array, int arrayIndex)
        //{
        //    baseEnumerator.OfType<T>().ToList().CopyTo(array, arrayIndex);
        //}

        /// <summary>
        /// Remove an item from this collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public bool Remove(T item)
        {
            if (!Contains(item)) return false;
            base.Remove(item);
            return true;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return baseEnumerator.OfType<T>().GetEnumerator();
        }
        #endregion

        #region non-generic ISelectableCollectionSource implementations

        ICollection ISelectableCollection.SelectedItems => SelectedItems;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        void ISelectableCollection.AddToSelection(object item)
        {
            if (item is T t)
            {
                AddToSelection(t);
            }
        }

        void ISelectableCollection.Select(object item)
        {
            if (item is T t)
            {
                Select(t);
            }
        }

        void ISelectableCollection.Deselect(object item)
        {
            if (item is T t)
            {
                Deselect(t);
            }
        }

        bool ISelectableCollection.IsSelected(object item)
        {
            if (item is T t)
            {
                return IsSelected(t);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Copy the elements of this Collection to an Array, starting at the specified index in the Array.
        /// </summary>
        /// <param name="array">The array to copy values into.</param>
        /// <param name="index">The starting index at which to start copying values.</param>
        /// <exception cref="ArgumentException">Raised if this Array is not an Array of type <typeparamref name="T"/>, 
        /// or if this Array isn't large enough to fit all the items in this Collection.</exception>
        public void CopyTo(T[] array, int index)
        {
            if (array.Length - index < Count)
            {
                throw new ArgumentException("The inputted array is not large enough to fit all of the elements in this collection.");
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    array[index + i] = this[i];
                }
            }
        }

        /// <summary>
        /// Copy the elements of this Collection to an Array, starting at the specified index in the Array.
        /// </summary>
        /// <param name="array">The array to copy values into.</param>
        /// <param name="index">The starting index at which to start copying values.</param>
        /// <exception cref="ArgumentException">Raised if this Array is not an Array of type <typeparamref name="T"/>, 
        /// or if this Array isn't large enough to fit all the items in this Collection.</exception>
        public void CopyTo(Array array, int index)
        {
            if (array is T[] a)
            {
                CopyTo(a, index);
            }
            else
            {
                throw new ArgumentException("The inputted Array is not an array of type \"" + typeof(T).Name + "\".");
            }
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            if (CanAddNewItem)
            {
                AddNewItem(item);
                CommitNew();
            }
            else
            {
                throw new NotSupportedException("Adding in new entries isn't directly supported. Please edit the base collection this wraps around instead.");
            }
            //throw new NotSupportedException("This object does not support directly adding in new entries. Please consider using AddNew, or edit the base collection this wraps around instead.");
        }

        /// <summary>
        /// This function is not supported in this context.
        /// </summary>
        /// <exception cref="NotSupportedException">This function is not supported in this context.</exception>
        public void Clear()
        {
            throw new NotSupportedException("This object does not support this function. Please edit the base collection this wraps around instead.");
        }
        #endregion
    }
}
