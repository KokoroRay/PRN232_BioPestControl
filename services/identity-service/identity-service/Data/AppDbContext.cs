using Microsoft.EntityFrameworkCore;
using identity_service.Models;

namespace identity_service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ── DbSets ────────────────────────────────────────────────
        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<StaffPermission> StaffPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── User: Email duy nhất ──────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // ── Staff ─────────────────────────────────────────────
            modelBuilder.Entity<Staff>(entity =>
            {
                // Staff 1-1 với User
                entity.HasOne(s => s.User)
                      .WithMany()
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Index trên UserId để đảm bảo 1 User chỉ là 1 Staff
                entity.HasIndex(s => s.UserId).IsUnique();
            });

            // ── Permission: Code duy nhất ─────────────────────────
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(p => p.Code).IsUnique();
            });

            // ── StaffPermission (join table) ──────────────────────
            modelBuilder.Entity<StaffPermission>(entity =>
            {
                // Unique constraint: mỗi Staff chỉ có 1 bản ghi cho mỗi Permission
                entity.HasIndex(sp => new { sp.StaffId, sp.PermissionId }).IsUnique();

                entity.HasOne(sp => sp.Staff)
                      .WithMany(s => s.StaffPermissions)
                      .HasForeignKey(sp => sp.StaffId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sp => sp.Permission)
                      .WithMany(p => p.StaffPermissions)
                      .HasForeignKey(sp => sp.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Seed Permissions (UC14–UC20) ──────────────────────
            SeedPermissions(modelBuilder);
        }

        private static void SeedPermissions(ModelBuilder modelBuilder)
        {
            var permissions = new List<Permission>
            {
                // ── UC14: Manage Customers ────────────────────────
                new() { Id = 1,  Code = "customers.view_list",    DisplayName = "Xem danh sách khách hàng",   GroupCode = "UC14", GroupName = "UC14 - Quản lý khách hàng",   DisplayOrder = 1 },
                new() { Id = 2,  Code = "customers.view_detail",  DisplayName = "Xem chi tiết khách hàng",    GroupCode = "UC14", GroupName = "UC14 - Quản lý khách hàng",   DisplayOrder = 2 },
                new() { Id = 3,  Code = "customers.search",       DisplayName = "Tìm kiếm khách hàng",        GroupCode = "UC14", GroupName = "UC14 - Quản lý khách hàng",   DisplayOrder = 3 },
                new() { Id = 4,  Code = "customers.edit",         DisplayName = "Chỉnh sửa khách hàng",       GroupCode = "UC14", GroupName = "UC14 - Quản lý khách hàng",   DisplayOrder = 4 },
                new() { Id = 5,  Code = "customers.lock",         DisplayName = "Khóa tài khoản khách hàng",  GroupCode = "UC14", GroupName = "UC14 - Quản lý khách hàng",   DisplayOrder = 5 },

                // ── UC15: Manage Orders ───────────────────────────
                new() { Id = 6,  Code = "orders.view_list",       DisplayName = "Xem danh sách đơn hàng",     GroupCode = "UC15", GroupName = "UC15 - Quản lý đơn hàng",     DisplayOrder = 1 },
                new() { Id = 7,  Code = "orders.view_detail",     DisplayName = "Xem chi tiết đơn hàng",      GroupCode = "UC15", GroupName = "UC15 - Quản lý đơn hàng",     DisplayOrder = 2 },
                new() { Id = 8,  Code = "orders.search",          DisplayName = "Tìm kiếm đơn hàng",          GroupCode = "UC15", GroupName = "UC15 - Quản lý đơn hàng",     DisplayOrder = 3 },
                new() { Id = 9,  Code = "orders.update_delivery", DisplayName = "Cập nhật giao hàng",         GroupCode = "UC15", GroupName = "UC15 - Quản lý đơn hàng",     DisplayOrder = 4 },

                // ── UC16: Manage News/Articles ────────────────────
                new() { Id = 10, Code = "articles.filter",        DisplayName = "Lọc bài viết / tin tức",     GroupCode = "UC16", GroupName = "UC16 - Quản lý bài viết",     DisplayOrder = 1 },
                new() { Id = 11, Code = "articles.view",          DisplayName = "Xem bài viết / tin tức",     GroupCode = "UC16", GroupName = "UC16 - Quản lý bài viết",     DisplayOrder = 2 },
                new() { Id = 12, Code = "articles.create",        DisplayName = "Tạo bài viết / tin tức",     GroupCode = "UC16", GroupName = "UC16 - Quản lý bài viết",     DisplayOrder = 3 },
                new() { Id = 13, Code = "articles.edit",          DisplayName = "Chỉnh sửa bài viết",         GroupCode = "UC16", GroupName = "UC16 - Quản lý bài viết",     DisplayOrder = 4 },
                new() { Id = 14, Code = "articles.delete",        DisplayName = "Xóa bài viết / tin tức",     GroupCode = "UC16", GroupName = "UC16 - Quản lý bài viết",     DisplayOrder = 5 },

                // ── UC17: Manage Products ─────────────────────────
                new() { Id = 15, Code = "products.filter",        DisplayName = "Lọc sản phẩm",               GroupCode = "UC17", GroupName = "UC17 - Quản lý sản phẩm",    DisplayOrder = 1 },
                new() { Id = 16, Code = "products.search",        DisplayName = "Tìm kiếm sản phẩm",          GroupCode = "UC17", GroupName = "UC17 - Quản lý sản phẩm",    DisplayOrder = 2 },
                new() { Id = 17, Code = "products.view_detail",   DisplayName = "Xem chi tiết sản phẩm",      GroupCode = "UC17", GroupName = "UC17 - Quản lý sản phẩm",    DisplayOrder = 3 },
                new() { Id = 18, Code = "products.view_list",     DisplayName = "Xem danh sách sản phẩm",     GroupCode = "UC17", GroupName = "UC17 - Quản lý sản phẩm",    DisplayOrder = 4 },
                new() { Id = 19, Code = "products.view_feedback", DisplayName = "Xem phản hồi sản phẩm",      GroupCode = "UC17", GroupName = "UC17 - Quản lý sản phẩm",    DisplayOrder = 5 },
                new() { Id = 20, Code = "products.reply_feedback",DisplayName = "Trả lời phản hồi sản phẩm", GroupCode = "UC17", GroupName = "UC17 - Quản lý sản phẩm",    DisplayOrder = 6 },

                // ── UC18: Manage Categories ───────────────────────
                new() { Id = 21, Code = "categories.view",        DisplayName = "Xem danh mục sản phẩm",      GroupCode = "UC18", GroupName = "UC18 - Quản lý danh mục",    DisplayOrder = 1 },
                new() { Id = 22, Code = "categories.search",      DisplayName = "Tìm kiếm danh mục",          GroupCode = "UC18", GroupName = "UC18 - Quản lý danh mục",    DisplayOrder = 2 },

                // ── UC19: Manage Warehouse ────────────────────────
                new() { Id = 23, Code = "warehouse.view",         DisplayName = "Xem kho hàng",                GroupCode = "UC19", GroupName = "UC19 - Quản lý kho",          DisplayOrder = 1 },
                new() { Id = 24, Code = "warehouse.filter",       DisplayName = "Lọc kho hàng",                GroupCode = "UC19", GroupName = "UC19 - Quản lý kho",          DisplayOrder = 2 },

                // ── UC20: Manage Chemical Safety ──────────────────
                new() { Id = 25, Code = "chemical_safety.view",   DisplayName = "Xem an toàn hóa chất",       GroupCode = "UC20", GroupName = "UC20 - An toàn hóa chất",     DisplayOrder = 1 },
                new() { Id = 26, Code = "chemical_safety.search", DisplayName = "Tìm kiếm an toàn hóa chất",  GroupCode = "UC20", GroupName = "UC20 - An toàn hóa chất",     DisplayOrder = 2 },
                new() { Id = 27, Code = "chemical_safety.filter", DisplayName = "Lọc an toàn hóa chất",       GroupCode = "UC20", GroupName = "UC20 - An toàn hóa chất",     DisplayOrder = 3 },
            };

            modelBuilder.Entity<Permission>().HasData(permissions);
        }
    }
}
