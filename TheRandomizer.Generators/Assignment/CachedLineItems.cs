using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Assignment
{
    internal class CachedLineItems
    {
        /// <summary>The total weight of all items in the <see cref="Rules" /></summary>
        public Int32 TotalWeight { get; set; }
        /// <summary>The last index selected in this cache of line items, 
        /// used to prevent the same item from being selected continuously</summary>
        public Int32 LastSelect { get; set; }
        /// <summary>The list of rules contained in this cache</summary>
        public List<LineItem> Rules { get; set; }
    }
}
