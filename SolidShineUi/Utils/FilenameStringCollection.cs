using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SolidShineUi.Utils
{
    public class FilenameStringCollection : ObservableCollection<string>
    {
        public FilenameStringCollection()
        {
            Capacity = -1;
        }

        public FilenameStringCollection(int capacity)
        {
            Capacity = capacity;
        }

        public delegate void AddingItemStringEventHandler(object sender, AddingItemStringEventArgs e);

#if NETCOREAPP
        public event AddingItemStringEventHandler? AddingItem;
#else
        public event AddingItemStringEventHandler AddingItem;
#endif

        //private bool _internalAction = false;
        private int _capacity = -1;

        public int Capacity
        {
            get => _capacity;
            set
            {
                int oldCapacity = _capacity;
                _capacity = value;
                if (_capacity < 0) return;
                if (_capacity < oldCapacity)
                {
                    // capacity went down
                    if (Count > _capacity)
                    {
                        //_internalAction = true;
                        while (Count > _capacity)
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
            if (Capacity >= 0 && Count >= Capacity)
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
            if (Capacity >= 0 && Count >= Capacity)
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
