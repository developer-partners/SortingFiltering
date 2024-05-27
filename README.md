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

## Filtering

The `Query.Filter` property is a lit of `QueryProperty` objects. You can have multiple filtering conditions by adding multiple `QueryProperty` objects to the `Query.Filter` list.

```
using Microsoft.EntityFrameworkCore;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;

using (var context = new AppDbContext())
{
    var query = new Query();

    query.Filter.AddRange(new []
    {
        new QueryProperty
        {
            ColumnName = "Name",
            Value = "iPhone",
            ComparisonOperator = ComparisonOperator.Ct
        },
        new QueryProperty
        {
            ColumnName = "UnitPrice",
            Value = "1000",
            ComparisonOperator = ComparisonOperator.Lte
        }
    });

    var products = await context
        .Products
        .Where(query.Filter)
        .ToListAsync();
}
```

The query above returns products where Name contains the word "iPhone" and UnitPrice is less than or equal to 1,000.

## Using as Query String Parameters

The real power of `DeveloperPartners.SortingFiltering` comes when using `PageInfo` and `Query` as query string parameters of API endpoints. Let's take a look at the following endpoint:

```
public async Task<IActionResult> GetAllProductsAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query)
{
    var products = await _context
        .Products
        .Where(query.Filter)
        .OrderBy(query.Sort)
        .Pageinate(pageInfo)
        .ToListAsync();

    var pagedData = products.ToPagedData(pageInfo);

    return Ok(pagedData);
}
```

You can call the endpoint above with the following URL and query string parameters:
```
/api/products?pageNumber=2&pageSize=25&s[DateCreated]=Desc&s[Name]=Asc&q[0].col=Name&q[0].op=Ct&q[0].val=iPhone
```

The query string above translates to "Get products where Name contains "iPhone". Sort by DateCreated in descending order, then by Name in ascending order. Paginate the data with page size of 25 and get the data of the second page."


## Credits
Developer Partners, Inc.

[https://developerpartners.com](https://developerpartners.com)

