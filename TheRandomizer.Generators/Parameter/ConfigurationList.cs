using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRandomizer.Utility.Collections;

namespace TheRandomizer.Generators.Parameter
{
    public class ConfigurationList : ObservableList<Configuration>
    {
        public ConfigurationList() : base() { }

        public ConfigurationList(IEnumerable<Configuration> collection) : base(collection) { }
        
        /// <summary>
        /// Determines whether an element with the given name is in the ConfigurationList
        /// </summary>
        public bool Contains(string name)
        {
            return this.Any(p => p.Name == name);
        }

        /// <summary>
        /// Gets or sets the element at the specified name
        /// </summary>
        public Configuration this[string name]
        {
            get
            {
                if (Contains(name))
                {
                    return this.Where(p => p.Name == name).First();
                }
                return null;
            }
            set
            {
                if (!Contains(name))
                {
                    Add(value);                    
                }
                else
                {
                    this[name] = value;
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            base.OnCollectionChanged(e);
        }

    }
}
