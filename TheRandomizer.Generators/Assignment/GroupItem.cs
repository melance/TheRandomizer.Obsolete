using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Utility;

namespace TheRandomizer.Generators.Assignment
{
    public class GroupItem : LineItem
    {
        private new string Name { get; set; }
        private new string Variable { get; set; }
    }
}
