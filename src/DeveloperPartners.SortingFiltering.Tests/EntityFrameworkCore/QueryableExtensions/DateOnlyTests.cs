
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeveloperPartners.SortingFiltering.Tests.EntityFrameworkCore.QueryableExtensions
{
    [TestClass]
    public class DateOnlyTests
    {
        [TestMethod]
        public async Task Equals()
        {
            using (var context = AppDbContext.CreateDbContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now)
                    },
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DueDate),
                            ParsedValue = DateTime.Now
                        }
                    }
                };

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(1, products.Count);
            }
        }

        [TestMethod]
        public async Task NotEqual()
        {
            using (var context = AppDbContext.CreateDbContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now)
                    },
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5))
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DueDate),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.NotEq
                        }
                    }
                };

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(1, products.Count);
            }
        }

        [TestMethod]
        public async Task GreaterThan()
        {
            using (var context = AppDbContext.CreateDbContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now)
                    },
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DueDate),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.Gt
                        }
                    }
                };

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(1, products.Count);
            }
        }

        [TestMethod]
        public async Task GreaterThanOrEqual()
        {
            using (var context = AppDbContext.CreateDbContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now)
                    },
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
                    },
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DueDate),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.Gte
                        }
                    }
                };

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(2, products.Count);
            }
        }

        [TestMethod]
        public async Task LessThan()
        {
            using (var context = AppDbContext.CreateDbContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now)
                    },
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DueDate),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.Lt
                        }
                    }
                };

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(1, products.Count);
            }
        }

        [TestMethod]
        public async Task LessThanOrEqual()
        {
            using (var context = AppDbContext.CreateDbContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now)
                    },
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
                    },
                    new Product
                    {
                        DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DueDate),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.Lte
                        }
                    }
                };

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(2, products.Count);
            }
        }
    }
}