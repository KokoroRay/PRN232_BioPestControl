using article_service.Models;
using MongoDB.Driver;

namespace article_service.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"]
                ?? throw new InvalidOperationException("MongoDB:ConnectionString is not configured.");
            var databaseName = configuration["MongoDB:DatabaseName"]
                ?? throw new InvalidOperationException("MongoDB:DatabaseName is not configured.");

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);

            SeedData();
        }

        public IMongoCollection<Article> Articles =>
            _database.GetCollection<Article>("Articles");

        public IMongoCollection<Feedback> Feedbacks =>
            _database.GetCollection<Feedback>("Feedbacks");

        public IMongoCollection<Contact> Contacts =>
            _database.GetCollection<Contact>("Contacts");

        /// <summary>
        /// Insert seed articles if the collection is empty.
        /// </summary>
        private void SeedData()
        {
            if (Articles.CountDocuments(FilterDefinition<Article>.Empty) > 0)
                return;

            var seedArticles = new List<Article>
            {
                new Article
                {
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
                    Title = "Kiểm soát dịch hại tích hợp (IPM) trong nông nghiệp",
                    Content = "Quản lý dịch hại tích hợp (IPM) là phương pháp tiếp cận toàn diện kết hợp các biện pháp sinh học, văn hóa, vật lý và hóa học để giảm thiểu tác hại của sâu bệnh đến mức có thể chấp nhận được về mặt kinh tế.",
                    Summary = "Khám phá phương pháp IPM giúp kiểm soát dịch hại hiệu quả và thân thiện môi trường.",
                    Status = "Published",
                    Tags = "IPM,dịch hại,nông nghiệp sinh học",
                    CreatedAt = new DateTime(2024, 3, 5, 0, 0, 0, DateTimeKind.Utc),
                    CreatedByStaffId = 1,
                    ManagedByAdminId = 1
                }
            };

            Articles.InsertMany(seedArticles);
        }
    }
}
