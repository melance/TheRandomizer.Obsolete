using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators
{
    public class TagComparer : IEqualityComparer<Tag>
    {
        private TagComparer() { }

        public static TagComparer Instance
        {
            get
            {
               return new TagComparer();
            }
        }

        public bool Equals(Tag x, Tag y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode(Tag obj)
        {
            return obj.Value == null ? obj.GetHashCode() : obj.Value.GetHashCode();
        }
    }
}
