using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.WinApp.ViewModels
{
    public class Tag : ObservableBase
    {
        public Tag()
        {
            Selected = true;
        }

        public Tag(string name)
        {
            Name = name;
            Selected = true;
        }

        public Tag(string name, bool selected)
        {
            Name = name;
            Selected = selected;
        }

        public string Name
        {
            get
            {
                return GetProperty<string>();
            }
            set
            {
                SetProperty(value);
            }
        }

        public bool Selected
        {
            get
            {
                return GetProperty<bool>();
            }
            set
            {
                SetProperty(value);
            }
        }
    }
}
