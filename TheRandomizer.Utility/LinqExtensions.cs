using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Utility
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Pagination<T>(this IEnumerable<T> extended, Int32 page, Int32 pageSize)
        {
            return extended.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
