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
        public LimitableStringCollection()
        {
            MaxCount = -1;
        }

        public LimitableStringCollection(int maxCount)
        {
            MaxCount = maxCount;
        }

        public delegate void AddingItemStringEventHandler(object sender, AddingItemStringEventArgs e);

#if NETCOREAPP
        public event AddingItemStringEventHandler? AddingItem;
#else
        public event AddingItemStringEventHandler AddingItem;
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

        public new void Add(string item)
        {
            if (Contains(item)) return;
            AddingItemStringEventArgs e = new AddingItemStringEventArgs(item);
            AddingItem?.Invoke(this, e);
            if (e.Cancel) return;
            if (MaxCount >= 0 && Count >= MaxCount)
            {
                RemoveAt(Count - 1);
            }
            base.Add(item);
        }

        public new void Insert(int index, string item)
        {
            if (Contains(item)) return;
            AddingItemStringEventArgs e = new AddingItemStringEventArgs(item);
            AddingItem?.Invoke(this, e);
            if (e.Cancel) return;
            if (MaxCount >= 0 && Count >= MaxCount)
            {
                RemoveAt(Count - 1);
            }
            base.Insert(index, item);
        }

    }

    public class AddingItemStringEventArgs : EventArgs
    {
        public string Item { get; private set; }

        public bool Cancel { get; set; } = false;

        public AddingItemStringEventArgs(string item)
        {
            Item = item;
        }
    }

    public class AddingItemEventArgs<T> : EventArgs
    {
        public T Item { get; private set; }

        public bool Cancel { get; set; } = false;

        public AddingItemEventArgs(T item)
        {
            Item = item;
        }
    }
}
