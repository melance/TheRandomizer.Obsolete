using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Utility
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/>  delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the <see cref="ObservableCollection{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public static int RemoveAll<T>(this ObservableCollection<T> extended, Func<T, bool> match)
        {
            if (match == null) throw new ArgumentNullException("match");
            if (extended == null) return 0;
            var toRemove = extended.Where(match).ToList();
            foreach (var item in toRemove)
            {
                extended.Remove(item);
            }

            return toRemove.Count();
        }
    }
}
