using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Utility
{
    public class ObservableCollection<T> : IList<T>
    {

        #region Events
        public event EventHandler<ListItemChangedEventArgs<T>> ItemAdded;
        public event EventHandler<ListItemChangedEventArgs<T>> ItemRemoved;
        public event EventHandler<ListItemChangedEventArgs<T>> ItemUpdated;
        public event EventHandler Cleared;
        #endregion

        #region Constructors
        public ObservableCollection() { }

        public ObservableCollection(IEnumerable<T> collection)
        {
            Items.AddRange(collection);
        }
        #endregion  

        #region Members
        private List<T> Items { get; set; } = new List<T>();
        #endregion

        #region Public Properties
        public T this[int index]
        {
            get
            {
                return Items[index];
            }

            set
            {
                Items[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return Items.Count();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Public Methods
        public void Add(T item)
        {
            Items.Add(item);
            OnItemAdded(item);
        }

        public void Clear()
        {
            Items.Clear();
            OnCleared();
        }

        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return Items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Items.Insert(index, item);
            OnItemAdded(item);
        }

        public bool Remove(T item)
        {
            var result = Items.Remove(item);
            if (result) OnItemRemoved(item);
            return result;
        }

        public void RemoveAt(int index)
        {
            var item = this[index];
            Items.RemoveAt(index);
            OnItemRemoved(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Protected Methods
        protected virtual void OnItemAdded(T item)
        {
            ItemAdded(this, new ListItemChangedEventArgs<T>(item, Items.IndexOf(item)));
        }

        protected virtual void OnItemRemoved(T item)
        {
            ItemRemoved(this, new ListItemChangedEventArgs<T>(item, -1));
        }

        protected virtual void OnItemUpdated(T item)
        {
            ItemUpdated(this, new ListItemChangedEventArgs<T>(item, Items.IndexOf(item)));
        }
        
        protected virtual void OnCleared()
        {
            Cleared(this, EventArgs.Empty);
        }
        #endregion
    }
}
