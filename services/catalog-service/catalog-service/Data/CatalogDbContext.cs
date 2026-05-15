using catalog_service.Models;
using Microsoft.EntityFrameworkCore;

namespace catalog_service.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Phân bón và hóa chất" },
                new Category { Id = 2, Name = "Thuốc trừ bệnh" },
                new Category { Id = 3, Name = "Thuốc trừ cỏ" },
                new Category { Id = 4, Name = "Thuốc trừ sâu" }
            );

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.SKU).IsUnique();
                entity.HasOne(p => p.Category)
                      .WithMany()
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
