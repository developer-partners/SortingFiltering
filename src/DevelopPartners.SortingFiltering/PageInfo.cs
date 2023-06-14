
namespace DeveloperPartners.SortingFiltering
{
    public class PageInfo
    {
        public static int MaxPageSize { get; set; } = 500;
        public static int DefaultPageNumber { get; set; } = 1;
        public static int DefaultPageSize { get; set; } = 20;

        public decimal TotalItems { get; set; }

        public decimal TotalPages { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}