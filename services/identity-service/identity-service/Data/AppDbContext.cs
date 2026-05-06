using Microsoft.EntityFrameworkCore;
using identity_service.Models;

namespace identity_service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Đại diện cho bảng Users trong Database
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Thiết lập ràng buộc (constraints) cho các cột trong bảng
            modelBuilder.Entity<User>(entity =>
            {
                // Đảm bảo Email là duy nhất, không được trùng lặp trong cơ sở dữ liệu
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}
