using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A string collection where a limit can be applied to the number of items allowed. This collection can be observed via the CollectionChanged event.
    /// </summary>
    [DefaultEvent("CollectionChanged")]
    public class LimitableStringCollection : ObservableCollection<string>
    {
        /// <summary>
        /// Create a LimitableStringCollection.
        /// </summary>
        public LimitableStringCollection()
        {
            MaxCount = -1;
        }

        /// <summary>
        /// Create a LimitableStringCollection, with a max limit of items preset.
        /// </summary>
        /// <param name="maxCount">The maximum number of items allowed in this collection.</param>
        public LimitableStringCollection(int maxCount)
        {
            MaxCount = maxCount;
        }

        /// <summary>
        /// The event handler for <see cref="ItemAdding"/> event, with the ability to cancel adding in the item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ItemAddingStringEventHandler(object sender, ItemAddingStringEventArgs e);

        /// <summary>
        /// Raised prior to an item being added to this collection, with the ability to cancel adding the item.
        /// </summary>
#if NETCOREAPP
        public event ItemAddingStringEventHandler? ItemAdding;
#else
        public event ItemAddingStringEventHandler ItemAdding;
#endif

        //private bool _internalAction = false;
        private int _maxCount = -1;

        /// <summary>
        /// Get or set the maximum number of items allowed for this collection. If the maximum count is less than 0, then the collection is allowed to grow to any size without a limit.
        /// </summary>
        public int MaxCount
        {
            get => _maxCount;
            set
            {
                int oldCapacity = _maxCount;
                _maxCount = value;
                if (_maxCount < 0) return;
                else if (_maxCount < oldCapacity)
                {
                    // capacity went down
                    if (Count > _maxCount)
                    {
                        //_internalAction = true;
                        while (Count > _maxCount)
                        {
                            RemoveAt(Count - 1);
                        }
                        //_internalAction = false;
                    }
                }
            }
        }

        /// <summary>
        /// Adds an object to the end of this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        /// If the LimitableStringCollection's ItemAdding event is handled, it is possible that the addition of this item could be cancelled.
        /// If the item being added would bring the count over the maximum count of items allowed (via <see cref="MaxCount"/>), then this item
        /// replaces the last item already in the collection (with the last item being removed before this item is added in).
        /// </remarks>
        public new void Add(string item)
        {
            if (Contains(item)) return;
            ItemAddingStringEventArgs e = new ItemAddingStringEventArgs(item);
            ItemAdding?.Invoke(this, e);
            if (e.Cancel) return;
            if (MaxCount >= 0 && Count >= MaxCount)
            {
                RemoveAt(Count - 1);
            }
            base.Add(item);
        }

        /// <summary>
        /// Insert an object into the collection at the specified index.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="index">The zero-based index at which the item should be inserted.</param>
        /// <remarks>
        /// If the LimitableStringCollection's ItemAdding event is handled, it is possible that the insertion of this item could be cancelled.
        /// If the item being inserted would bring the count over the maximum count of items allowed (via <see cref="MaxCount"/>), then this item
        /// is inserted after the current last item in the collection is removed.
        /// </remarks>
        public new void Insert(int index, string item)
        {
            if (Contains(item)) return;
            ItemAddingStringEventArgs e = new ItemAddingStringEventArgs(item);
            ItemAdding?.Invoke(this, e);
            if (e.Cancel) return;
            if (MaxCount >= 0 && Count >= MaxCount)
            {
                RemoveAt(Count - 1);
            }
            base.Insert(index, item);
        }

    }

    /// <summary>
    /// The event arguments for the ItemAdding event of the <see cref="LimitableStringCollection"/>.
    /// </summary>
    public class ItemAddingStringEventArgs : EventArgs
    {
        /// <summary>
        /// Get the item about to be added.
        /// </summary>
        public string Item { get; private set; }

        /// <summary>
        /// Get or set whether the addition of this item should be cancelled. By default, this is set to <c>false</c>, as in "do not cancel".
        /// </summary>
        public bool Cancel { get; set; } = false;

        /// <summary>
        /// Create an ItemAddingStringEventArgs.
        /// </summary>
        /// <param name="item">The item about to be added.</param>
        public ItemAddingStringEventArgs(string item)
        {
            Item = item;
        }
    }

}
