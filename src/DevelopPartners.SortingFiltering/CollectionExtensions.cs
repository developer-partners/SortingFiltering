
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeveloperPartners.SortingFiltering
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            if (list == null)
            {
                return true;
            }

            return list.Any() == false;
        }

        public static bool IsNullOrEmpty(this IEnumerable list)
        {
            if (list == null)
            {
                return true;
            }

            return list.GetEnumerator().MoveNext() == false;
        }

        public static IPagedData<TElement> ToPagedData<TElement>(this IEnumerable<TElement> elements, PageInfo pageInfo)
        {
            return new PagedData<TElement>(pageInfo, elements);
        }
    }
}