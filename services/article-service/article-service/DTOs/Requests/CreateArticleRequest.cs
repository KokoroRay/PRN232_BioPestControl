using System.ComponentModel.DataAnnotations;

namespace article_service.DTOs.Requests
{
    public class CreateArticleRequest
    {
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
        public string Status { get; set; } = "Published";

        [MaxLength(100)]
        public string? Tags { get; set; }

        public int? CreatedByStaffId { get; set; }
        public int? ManagedByAdminId { get; set; }
    }
}
