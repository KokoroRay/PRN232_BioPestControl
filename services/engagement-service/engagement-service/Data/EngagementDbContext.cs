using engagement_service.Models;
using Microsoft.EntityFrameworkCore;

namespace engagement_service.Data
{
    public class EngagementDbContext : DbContext
    {
        public EngagementDbContext(DbContextOptions<EngagementDbContext> options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Article>().HasData(
                new Article
                {
                    Id = 1,
                    Title = "Hướng dẫn sử dụng thuốc trừ sâu an toàn",
                    Content = "Thuốc trừ sâu sinh học là giải pháp bền vững cho nông nghiệp hiện đại. Bài viết này cung cấp hướng dẫn chi tiết về cách sử dụng thuốc trừ sâu một cách an toàn và hiệu quả, đảm bảo sức khỏe cho người dùng và bảo vệ môi trường.",
                    Summary = "Hướng dẫn chi tiết về cách dùng thuốc trừ sâu an toàn cho nông nghiệp sinh học.",
                    Status = "Published",
                    Tags = "thuốc trừ sâu,nông nghiệp,an toàn",
                    CreatedAt = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                    CreatedByStaffId = 1,
                    ManagedByAdminId = 1
                },
                new Article
                {
                    Id = 2,
                    Title = "Phân bón hữu cơ và lợi ích với cây trồng",
                    Content = "Phân bón hữu cơ không chỉ cung cấp dinh dưỡng cho cây trồng mà còn cải thiện cấu trúc đất, tăng khả năng giữ nước và thúc đẩy hệ sinh vật đất phát triển. Đây là nền tảng của nông nghiệp bền vững.",
                    Summary = "Tìm hiểu về các loại phân bón hữu cơ và cách chúng giúp cải thiện năng suất cây trồng.",
                    Status = "Published",
                    Tags = "phân bón,hữu cơ,cây trồng",
                    CreatedAt = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                    CreatedByStaffId = 2,
                    ManagedByAdminId = 1
                },
                new Article
                {
                    Id = 3,
                    Title = "Kiểm soát dịch hại tích hợp (IPM) trong nông nghiệp",
                    Content = "Quản lý dịch hại tích hợp (IPM) là phương pháp tiếp cận toàn diện kết hợp các biện pháp sinh học, văn hóa, vật lý và hóa học để giảm thiểu tác hại của sâu bệnh đến mức có thể chấp nhận được về mặt kinh tế.",
                    Summary = "Khám phá phương pháp IPM giúp kiểm soát dịch hại hiệu quả và thân thiện môi trường.",
                    Status = "Published",
                    Tags = "IPM,dịch hại,nông nghiệp sinh học",
                    CreatedAt = new DateTime(2024, 3, 5, 0, 0, 0, DateTimeKind.Utc),
                    CreatedByStaffId = 1,
                    ManagedByAdminId = 1
                }
            );
        }
    }
}
