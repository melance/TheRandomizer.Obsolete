using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Utility;
using TheRandomizer.Utility.Collections;

namespace TheRandomizer.Generators.Assignment
{
    public class GroupItemList : LineItem
    {
        [XmlElement("item")]
        public ObservableList<GroupItem> GroupItems { get; set; } = new ObservableList<GroupItem>();
        private new string Expression { get; set; }
        private new int Weight { get; set; }
    }
}
