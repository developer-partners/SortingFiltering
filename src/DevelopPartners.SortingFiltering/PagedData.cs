
using System.Collections.Generic;

namespace DeveloperPartners.SortingFiltering
{
    public class PagedData<TElement> : IPagedData<TElement>
    {
        public PageInfo PageInfo { get; private set; }
        public IEnumerable<TElement> Data { get; private set; }

        public PagedData(PageInfo pageInfo, IEnumerable<TElement> data)
        {
            this.PageInfo = pageInfo;
            this.Data = data;
        }
    }

    public interface IPagedData<TElement>
    {
        PageInfo PageInfo { get; }
        IEnumerable<TElement> Data { get; }
    }
}