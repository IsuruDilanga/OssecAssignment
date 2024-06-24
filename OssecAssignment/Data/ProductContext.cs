// Data/ProductContext.cs
using Microsoft.EntityFrameworkCore;
using OssecAssignment.Models;

namespace OssecAssignment.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
        public DbSet<OssecAssignment.Models.User> User { get; set; } = default!;
    }
}
