
using Microsoft.EntityFrameworkCore;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;

namespace DeveloperPartners.SortingFiltering.Tests.EntityFrameworkCore.QueryableExtensions
{
    [TestClass]
    public class NullTests
    {
        [TestMethod]
        public async Task Equal()
        {
            //create In Memory Database
            using (var context = AppDbContext.CreateDbContext())
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
        public async Task NotEqual()
        {
            //create In Memory Database
            using (var context = AppDbContext.CreateDbContext())
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