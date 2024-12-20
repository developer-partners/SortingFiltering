
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeveloperPartners.SortingFiltering.Tests.EntityFrameworkCore.QueryableExtensions
{
    [TestClass]
    public class NestedQueryTests
    {
        [TestMethod]
        public async Task MultiplOr()
        {
            using (var context = AppDbContext.CreateDbContext())
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

        [TestMethod]
        public async Task MultipleAnd()
        {
            using (var context = AppDbContext.CreateDbContext())
            {
                context.Products.AddRange(
                [
                    new()
                    {
                        Name = "Test 1",
                        DateCreated = DateTime.Now
                    },
                    new()
                    {
                        Name = "Test 2",
                        DateCreated = DateTime.Now
                    },
                    new()
                    {
                        Name = "Test 3",
                        DateCreated = DateTime.Now
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
                                    Value = "Test",
                                    ComparisonOperator = ComparisonOperator.StW
                                },
                                new()
                                {
                                    Value = "2",
                                    ComparisonOperator = ComparisonOperator.EndW
                                }
                            }
                        }
                    }
                };

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(1, products.Count);
            }
        }
    }
}