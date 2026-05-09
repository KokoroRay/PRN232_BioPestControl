using System.ComponentModel.DataAnnotations;

namespace engagement_service.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        [MaxLength(500)]
        public string? Summary { get; set; }

        [MaxLength(200)]
        public string? ThumbnailUrl { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Published"; // Draft, Published, Archived

        [MaxLength(100)]
        public string? Tags { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int? CreatedByStaffId { get; set; }
        public int? ManagedByAdminId { get; set; }
    }
}
