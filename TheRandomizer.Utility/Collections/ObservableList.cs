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
    public class ObservableList<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler ItemPropertyChanged;

        protected override void InsertItem(int index, T item)
        {
            item.PropertyChanged += OnPropertyChanged;
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            Items[index].PropertyChanged -= OnPropertyChanged;
            base.RemoveItem(index);
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemPropertyChanged(sender, e);           
        }
    }
}
