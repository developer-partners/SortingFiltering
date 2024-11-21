
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeveloperPartners.SortingFiltering.Tests.EntityFrameworkCore.QueryableExtensions
{
    [TestClass]
    public class NestedQueryTests
    {
        private AppDbContext CreateDContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString("N"))
                .Options;

            return new AppDbContext(options);
        }

        [TestMethod]
        public async Task MultiplOr()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                [
                    new()
                    {
                        Name = "Test 1"
                    },
                    new()
                    {
                        Name = "Test 2"
                    },
                    new()
                    {
                        Name = "Test 3"
                    }
                ]);

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new()
                    {
                        new()
                        {
                            ColumnName = nameof(Product.Name),
                            Children = new()
                            {
                                new()
                                {
                                    Value = "Test 1",
                                    LogicalOperator = LogicalOperator.Or
                                },
                                new()
                                {
                                    Value = "Test 2",
                                    LogicalOperator = LogicalOperator.Or
                                }
                            }
                        }
                    }
                };

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(2, products.Count);
            }
        }

    }
}