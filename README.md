# DeveloperPartners.SortingFiltering

The `DeveloperPartners.SortingFiltering` library allows dynamically building Entity Framework queries for sorting, filtering, and pagination.

Let's assume you have an Entity Framework model called `Product` with the following properties:

```
public class Product
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime DateCreated { get; set; }

    public decimal UnitPrice { get; set; }
}
```

You can build dynamic Entity Framework queries using our extension methods for sorting, filtering, and paginating the `Products` table rows with the your model properties.

The dynamic queries for sorting and filtering are built using the `Query` class. The `Query` class has `Filter` and `Sort` properties. The `Filter` property is used for dynamically filtering data. The `Sort` property is used for dynamically sorting data.

```
public class Query
{
    public QueryFilter Filter { get; set; }

    public IDictionary<string, SortOperator> Sort { get; set; }
}
```

## Sorting

The `Query.Sort` property is a dictionary where the key of the dictionary is the property name that you want to sort by and the value is an `enum` representing whether the sort direction should be in ascending or descending order.

```
using Microsoft.EntityFrameworkCore;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;

using (var context = new AppDbContext())
{
    var query = new Query();

    query.Sort.Add("DateCreated", SortOperator.Desc);
    query.Sort.Add("Name", SortOperator.Asc);

    var products = await context
        .Products
        // The OrderBy method creates dynamic sorting by DateCreated descending order, then by Name ascending order.
        .OrderBy(query.Sort)
        .ToListAsync();
}
```

## Pagination

Pagination is done by using the `PageInfo` class. You can use the `PageNumber` and `PageSize` properties of the `PageInfo` class to specify which page should be queries and how many records should each page contain.

```
using Microsoft.EntityFrameworkCore;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;

using (var context = new AppDbContext())
{
    var pageInfo = new PageInfo
    {
        PageNumber = 1,
        PageSize = 20
    };

    var firstPageProducts = await context
        .Products
        // Data must be sorted for pagination.
        .OrderBy(product => product.Id)
        // The Paginate method takes the first page data with 20 records per page.
        .Paginate(pageInfo)
        .ToListAsync();

    var firstPage = firstPageProducts.ToPagedData(pageInfo);

    Console.WriteLine(pageInfo.PageNumber);
    Console.WriteLine(pageInfo.PageSize);
    Console.WriteLine(pageInfo.TotalPages);

    foreach (var product in firstPage.Data)
    {
        // Use products here.
    }
}
```

For being able to paginate, the data must first be sorted. You can use the regular Entity Framework `OrderBy` method to paginate sort by the properties you know at design time or you can use `Query.Sort` for dynamically sorting and dynamically paginating the sorted data:

```
using Microsoft.EntityFrameworkCore;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;

using (var context = new AppDbContext())
{
    var pageInfo = new PageInfo
    {
        PageNumber = 1,
        PageSize = 20
    };

    var query = new Query();

    query.Sort.Add("DateCreated", SortOperator.Desc);
    query.Sort.Add("Name", SortOperator.Asc);

    var firstPageProducts = context
        .Products
        // Data must be sorted for pagination. Use Query.Sort for dynamically sorting data before pagination.
        .OrderBy(query.Sort)
        // The Paginate method takes the first page data with 20 records per page.
        .Paginate(pageInfo)
        .ToListAsync();

    var firstPage = firstPageProducts.ToPagedData(pageInfo);

    Console.WriteLine(pageInfo.PageNumber);
    Console.WriteLine(pageInfo.PageSize);
    Console.WriteLine(pageInfo.TotalPages);

    foreach (var product in firstPage.Data)
    {
        // Use products here.
    }
}
```