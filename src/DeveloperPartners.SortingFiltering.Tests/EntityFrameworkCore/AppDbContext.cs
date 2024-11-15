
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DeveloperPartners.SortingFiltering.Tests.EntityFrameworkCore
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public AppDbContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>()
                .HasKey(p => new { p.ProductId, p.CategoryId });
        }
    }

    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? TypeId { get; set; }
        public ProductType? Type { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DueDate { get; set; }
    }

    public class ProductType
    {
        [Key]
        public int Id { get; set; }
    }

    public class Category
    {
        [Key]
        public int Id { get; set; }
    }

    public class ProductCategory
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}