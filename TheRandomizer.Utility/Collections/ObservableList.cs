using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Utility.Collections
{
    /// <summary>
    /// An <see cref="ObservableCollection{T}"/> that also notifies when one of the items in the list raises the <see cref="ObservableList{T}.ItemPropertyChanged"/> event
    /// </summary>
    /// <typeparam name="T">The type the list will hold.  Must implement <see cref="INotifyPropertyChanged"/> </typeparam>
    public class ObservableList<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler ItemPropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}" /> class.
        /// </summary>
        public ObservableList() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        public ObservableList(IEnumerable<T> collection) : base(collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}"/> class that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list">The list from which the elements are copied.</param>
        public ObservableList(List<T> list) : base(list) { }

        /// <summary>
        /// Adds an object to the end of the <see cref="ObservableList{T}"/>. (Replaces <see cref="Collection{T}.Add(T)"/>)
        /// </summary>
        /// <param name="item">The object to add.</param>
        public new void Add(T item)
        {
            item.PropertyChanged += OnPropertyChanged;
            base.Add(item);
        }

        /// <summary>
        /// Adds a list of objects to the end of the <see cref="ObservableList{T}"/>. 
        /// </summary>
        /// <param name="items">The list of objects to add</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="ObservableList{T}"/>. (Replaces <see cref="Collection{T}.Clear"/>)
        /// </summary>
        public new void Clear()
        {
            foreach (T item in Items)
            {
                if (item != null) item.PropertyChanged -= OnPropertyChanged;
            }
            base.Clear();
        }

        /// <summary>
        /// Removes all items from the collection. (Overrides <see cref="ObservableCollection{T}.ClearItems"/>)
        /// </summary>
        protected override void ClearItems()
        {
            foreach (T item in Items)
            {
                if (item != null) item.PropertyChanged -= OnPropertyChanged;
            }
            base.ClearItems();
        }

        /// <summary>
        /// Inserts an item into the <see cref="ObservableList{T}"/> at the specified index. (Replaces <see cref="Collection{T}.Insert(int, T)"/>) 
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        public new void Insert(int index, T item)
        {
            InsertItem(index, item);
        }

        /// <summary>
        /// Inserts an item into the <see cref="ObservableList{T}"/> at the specified index. (Overrides <see cref="ObservableCollection{T}.InsertItem(int, T)"/> 
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            item.PropertyChanged += OnPropertyChanged;
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ObservableList{T}"/> (Replaces <see cref="Collection{T}.Remove(T)"/>)
        /// </summary>
        /// <param name="item">The object to remove.</param>
        public new void Remove(T item)
        {
            item.PropertyChanged -= OnPropertyChanged;
            base.Remove(item);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="ObservableList{T}"/> (Replaces <see cref="Collection{T}.RemoveAt(int)"/>)
        /// </summary>
        /// <param name="index"></param>
        public new void RemoveAt(int index)
        {
            if (Items.Count > index && Items[index] != null) Items[index].PropertyChanged -= OnPropertyChanged;
            base.RemoveAt(index);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ObservableList{T}"/> (Overrides <see cref="Collection{T}.RemoveItem(int)"/>)
        /// </summary>
        /// <param name="index">The zero-based index of the item to be removed.</param>
        protected override void RemoveItem(int index)
        {
            if (Items.Count > index && Items[index] != null) Items[index].PropertyChanged -= OnPropertyChanged;
            base.RemoveItem(index);
        }

        /// <summary>
        /// Raises the <see cref="ObservableList{T}.ItemPropertyChanged"/> event 
        /// </summary>
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemPropertyChanged?.Invoke(sender, e);
        }
    }
}
