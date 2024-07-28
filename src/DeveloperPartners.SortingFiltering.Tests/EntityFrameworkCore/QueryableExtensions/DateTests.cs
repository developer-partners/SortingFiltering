
using Microsoft.EntityFrameworkCore;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;

namespace DeveloperPartners.SortingFiltering.Tests.EntityFrameworkCore.QueryableExtensions
{
    [TestClass]
    public class DateTests
    {
        private AppDbContext CreateDContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "SortingFilteringDb")
                .Options;

            return new AppDbContext(options);
        }

        [TestMethod]
        public async Task Equals()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.Now
                    },
                    new Product
                    {
                        DateCreated = DateTime.Now.AddDays(-1)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
                            ParsedValue = DateTime.Now
                        }
                    }
                };

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(1, products.Count);
            }
        }

        [TestMethod]
        public async Task EqualsUtc()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.UtcNow
                    },
                    new Product
                    {
                        DateCreated = DateTime.UtcNow.AddDays(-1)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
                            ParsedValue = DateTime.Now
                        }
                    }
                };

                query.Filter.ClientTimeZone = TimeZoneInfo.Local;

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(1, products.Count);
            }
        }

        [TestMethod]
        public async Task NotEqual()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.Now
                    },
                    new Product
                    {
                        DateCreated = DateTime.Now.AddDays(5)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
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
        public async Task NotEqualUtc()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.UtcNow
                    },
                    new Product
                    {
                        DateCreated = DateTime.UtcNow.AddDays(5)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.NotEq
                        }
                    }
                };

                query.Filter.ClientTimeZone = TimeZoneInfo.Local;

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(1, products.Count);
            }
        }

        [TestMethod]
        public async Task GreaterThan()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.Now
                    },
                    new Product
                    {
                        DateCreated = DateTime.Now.AddDays(1)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
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
        public async Task GreaterThanUtc()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.UtcNow
                    },
                    new Product
                    {
                        DateCreated = DateTime.UtcNow.AddDays(1)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.Gt
                        }
                    }
                };

                query.Filter.ClientTimeZone = TimeZoneInfo.Local;

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(1, products.Count);
            }
        }

        [TestMethod]
        public async Task GreaterThanOrEqual()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.Now
                    },
                    new Product
                    {
                        DateCreated = DateTime.Now.AddDays(-1)
                    },
                    new Product
                    {
                        DateCreated = DateTime.Now.AddDays(1)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
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
        public async Task GreaterThanOrEqualUtc()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.UtcNow
                    },
                    new Product
                    {
                        DateCreated = DateTime.UtcNow.AddDays(-1)
                    },
                    new Product
                    {
                        DateCreated = DateTime.UtcNow.AddDays(1)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.Gte
                        }
                    }
                };

                query.Filter.ClientTimeZone = TimeZoneInfo.Local;

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(2, products.Count);
            }
        }


        [TestMethod]
        public async Task LessThan()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.Now
                    },
                    new Product
                    {
                        DateCreated = DateTime.Now.AddDays(-1)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
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
        public async Task LessThanUtc()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.UtcNow
                    },
                    new Product
                    {
                        DateCreated = DateTime.UtcNow.AddDays(-1)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.Lt
                        }
                    }
                };

                query.Filter.ClientTimeZone = TimeZoneInfo.Local;

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(1, products.Count);
            }
        }

        [TestMethod]
        public async Task LessThanOrEqual()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.Now
                    },
                    new Product
                    {
                        DateCreated = DateTime.Now.AddDays(-1)
                    },
                    new Product
                    {
                        DateCreated = DateTime.Now.AddDays(1)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.Lte
                        }
                    }
                };

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(2, products.Count);
            }
        }

        [TestMethod]
        public async Task LessThanOrEqualUtc()
        {
            using (var context = CreateDContext())
            {
                context.Products.AddRange(
                    new Product
                    {
                        DateCreated = DateTime.UtcNow
                    },
                    new Product
                    {
                        DateCreated = DateTime.UtcNow.AddDays(-1)
                    },
                    new Product
                    {
                        DateCreated = DateTime.UtcNow.AddDays(1)
                    }
                );

                await context.SaveChangesAsync();

                var query = new Query
                {
                    Filter = new QueryFilter
                    {
                        new QueryProperty
                        {
                            ColumnName = nameof(Product.DateCreated),
                            ParsedValue = DateTime.Now,
                            ComparisonOperator = ComparisonOperator.Lte
                        }
                    }
                };

                query.Filter.ClientTimeZone = TimeZoneInfo.Local;

                var products = await context.Products.Where(query.Filter).ToListAsync();

                Assert.AreEqual(2, products.Count);
            }
        }
    }
}