
namespace DeveloperPartners.SortingFiltering
{
    public class PageInfo
    {
        public const int MaxPageSize = 500;
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 20;

        public decimal TotalItems { get; set; }

        public decimal TotalPages { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}