
using Microsoft.EntityFrameworkCore;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;

namespace DeveloperPartners.SortingFiltering.Tests.EntityFrameworkCore.QueryableExtensions
{
    [TestClass]
    public class WhereTests
    {
        [TestMethod]
        public async Task Where_EqualNull()
        {
            //create In Memory Database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "SortingFilteringDb")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Product 1",
                        Description = "Product 1 description"
                    },
                    new Product
                    {
                        Name = "Product 2"
                    });

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.Description),
                            IsValueNull = true,
                            ComparisonOperator = ComparisonOperator.Eq
                        }
                    }
                };

                var products1 = await context.Products.Where(query.Filter).ToListAsync();
                var products2 = await context.Products.Where(p => p.Description == null).ToListAsync();

                Assert.AreEqual(products1.Count, products2.Count, $"products1 count = {products1.Count}, products2 count = {products2.Count}");
            }
        }

        [TestMethod]
        public async Task Where_NotEqualNull()
        {
            //create In Memory Database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "SortingFilteringDb")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Product 1",
                        Description = "Product 1 description"
                    },
                    new Product
                    {
                        Name = "Product 2"
                    });

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.Description),
                            IsValueNull = true,
                            ComparisonOperator = ComparisonOperator.NotEq
                        }
                    }
                };

                var products1 = await context.Products.Where(query.Filter).ToListAsync();
                var products2 = await context.Products.Where(p => p.Description == null).ToListAsync();

                Assert.AreEqual(products1.Count, products2.Count, $"products1 count = {products1.Count}, products2 count = {products2.Count}");
            }
        }
    }
}