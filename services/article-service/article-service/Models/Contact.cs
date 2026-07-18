using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace article_service.Models
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public bool IsResolved { get; set; } = false;
        
        public string? ResolutionNotes { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
