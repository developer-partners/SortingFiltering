
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DeveloperPartners.SortingFiltering.EntityFrameworkCore.Helpers
{
    internal class PaginationHelper
    {
        public static IQueryable<T> Paginate<T>(IOrderedQueryable<T> itemList, PageInfo pageInfo, int? pageSize)
            where T : class
        {
            if (pageInfo.PageNumber <= 0)
            {
                pageInfo.PageNumber = PageInfo.DefaultPageNumber;
            }

            if (pageSize.HasValue)
            {
                pageInfo.PageSize = pageSize.Value;
            }
            else if (pageInfo.PageSize > PageInfo.MaxPageSize || pageInfo.PageSize <= 0)
            {
                pageInfo.PageSize = PageInfo.DefaultPageSize;
            }

            pageInfo.TotalItems = itemList.Count();
            pageInfo.TotalPages = (int)decimal.Ceiling((pageInfo.TotalItems / pageInfo.PageSize));

            if (pageInfo.TotalPages > 0 && pageInfo.PageNumber > pageInfo.TotalPages)
            {
                pageInfo.PageNumber = (int)pageInfo.TotalPages;
            }

            return itemList
                .Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize)
                .Take(pageInfo.PageSize);
        }

        internal static async Task<IQueryable<T>> PaginateAsync<T>(IOrderedQueryable<T> itemList, PageInfo pageInfo, int? pageSize)
            where T : class
        {
            if (pageInfo.PageNumber <= 0)
            {
                pageInfo.PageNumber = PageInfo.DefaultPageNumber;
            }

            if (pageSize.HasValue)
            {
                pageInfo.PageSize = pageSize.Value;
            }
            else if (pageInfo.PageSize > PageInfo.MaxPageSize || pageInfo.PageSize <= 0)
            {
                pageInfo.PageSize = PageInfo.DefaultPageSize;
            }

            pageInfo.TotalItems = await itemList.CountAsync();
            pageInfo.TotalPages = (int)decimal.Ceiling((pageInfo.TotalItems / pageInfo.PageSize));

            if (pageInfo.TotalPages > 0 && pageInfo.PageNumber > pageInfo.TotalPages)
            {
                pageInfo.PageNumber = (int)pageInfo.TotalPages;
            }

            return itemList
                .Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize)
                .Take(pageInfo.PageSize);
        }
    }
}