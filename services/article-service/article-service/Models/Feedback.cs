using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace article_service.Models
{
    public class Feedback
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public int ProductId { get; set; }
        
        public Guid UserId { get; set; }
        
        public string UserName { get; set; } = string.Empty;

        public int Rating { get; set; } // 1 to 5

        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // For Staff/Admin replies
        public string? ReplyMessage { get; set; }
        public DateTime? RepliedAt { get; set; }
        public Guid? RepliedByStaffId { get; set; }
    }
}
