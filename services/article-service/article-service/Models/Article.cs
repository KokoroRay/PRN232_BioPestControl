using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace article_service.Models
{
    public class Article
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("content")]
        public string Content { get; set; } = null!;

        [BsonElement("summary")]
        public string? Summary { get; set; }

        [BsonElement("thumbnailUrl")]
        public string? ThumbnailUrl { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Published"; // Draft, Published, Archived

        [BsonElement("tags")]
        public string? Tags { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("createdByStaffId")]
        public int? CreatedByStaffId { get; set; }

        [BsonElement("managedByAdminId")]
        public int? ManagedByAdminId { get; set; }
    }
}
