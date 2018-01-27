using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRandomizer.Utility;

namespace TheRandomizer.WinApp.Models
{
    public class MRU : ObservableCollection<MRUItem>
    {
        public static MRU LoadMRU()
        {
            var xml = Utility.Settings.EditorMRU;
            if (!string.IsNullOrWhiteSpace(xml) && xml.TryDeserialize(out MRU value))
            {
                return value;
            }
            return new MRU();
        }

        private int _maxRecords = 10;

        public int MaxRecords { get { return _maxRecords; } set { _maxRecords = value; CheckLimit(); } }

        public void Add(string filePath)
        {
            var existingItem = Find(filePath);
            if (existingItem == null)
            {
                Add(new MRUItem(filePath));
            }
            else
            {
                Remove(existingItem);
                Add(existingItem);
            }
        }

        public void Remove(string filePath)
        {
            var item = Find(filePath);
            if (item != null) Remove(item);
        }

        public void Remove()
        {
            RemoveAt(0);
        }

        public bool Exists(string filePath)
        {
            return this.ToList().Find(m => m.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        public MRUItem Find(string filePath)
        {
            return this.ToList().Find(m => m.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase));
        }

        public int GetIndex(string filePath)
        {
            var item = Find(filePath);
            if (item != null)
            {
                return IndexOf(item);
            }
            return -1;
        }

        public void Save()
        {
            if (this.TrySerialize(out string xml))
            {
                Utility.Settings.EditorMRU = xml;
                Utility.Settings.Save();
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            CheckLimit();
            Save();
        }

        private void CheckLimit()
        {
            while (Items.Count > MaxRecords)
            {
                Remove();
            }
        }
    }
}
