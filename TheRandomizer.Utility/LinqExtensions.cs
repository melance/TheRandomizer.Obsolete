using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Utility
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Returns <paramref name="pageSize"/> items from the <see cref="IEnumerable{T}"/> starting at the <paramref name="page"/> provided.
        /// </summary>
        /// <typeparam name="T">The type of the collection</typeparam>
        /// <param name="extended">The <see cref="IEnumerable{T}"/> to retrieve the items from</param>
        /// <param name="page">The page to begin pulling items from</param>
        /// <param name="pageSize">The size of each page</param>
        /// <returns><paramref name="pageSize"/> items from <paramref name="page"/> in the <paramref name="extended"/> collection</returns>
        public static IEnumerable<T> Pagination<T>(this IEnumerable<T> extended, Int32 page, Int32 pageSize)
        {
            return extended.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
