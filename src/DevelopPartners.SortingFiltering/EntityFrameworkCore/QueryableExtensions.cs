
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore.Helpers;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore.Helpers.QueryHelper;

namespace DeveloperPartners.SortingFiltering.EntityFrameworkCore
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                return query.Where(expression);
            }

            return query;
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> itemList, QueryFilter filter)
        {
            return QueryHelper.Where(itemList, filter);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> itemList, IDictionary<string, SortOperator> sortColumns)
        {
            return QueryHelper.OrderBy(itemList, sortColumns);
        }

        public static IQueryable<T> Paginate<T>(this IOrderedQueryable<T> itemList, PageInfo pageInfo)
            where T : class
        {
            return PaginationHelper.Paginate(itemList, pageInfo, default);
        }

        public static IQueryable<T> Paginate<T>(this IOrderedQueryable<T> itemList, PageInfo pageInfo, int pageSize)
            where T : class
        {
            return PaginationHelper.Paginate(itemList, pageInfo, pageSize);
        }

        public static Task<IQueryable<T>> PaginateAsync<T>(this IOrderedQueryable<T> itemList, PageInfo pageInfo)
            where T : class
        {
            return PaginationHelper.PaginateAsync(itemList, pageInfo, default);
        }

        public static Task<IQueryable<T>> PaginateAsync<T>(this IOrderedQueryable<T> itemList, PageInfo pageInfo, int pageSize)
           where T : class
        {
            return PaginationHelper.PaginateAsync(itemList, pageInfo, pageSize);
        }
    }
}