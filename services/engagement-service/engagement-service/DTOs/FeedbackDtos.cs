using System.ComponentModel.DataAnnotations;

namespace engagement_service.DTOs
{
    public class FeedbackImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class FeedbackReplyDto
    {
        public int Id { get; set; }
        public string Reply { get; set; } = string.Empty;
        public int StaffId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class FeedbackTagDto
    {
        public int ReviewTagId { get; set; }
    }

    public class FeedbackDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public List<FeedbackImageDto> Images { get; set; } = new();
        public List<FeedbackReplyDto> Replies { get; set; } = new();
        public List<FeedbackTagDto> Tags { get; set; } = new();
    }

    public class CreateFeedbackRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int OrderId { get; set; }

        /// <summary>Bị ghi đè từ JWT khi role Customer; Admin có thể chỉ định.</summary>
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public List<string> ImageUrls { get; set; } = new();
        public List<int> TagsIds { get; set; } = new();
    }

    public class UpdateFeedbackRequest
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public List<string> ImageUrls { get; set; } = new();
        public List<int> TagsIds { get; set; } = new();
    }

    public class ReplyFeedbackRequest
    {
        [Required]
        [MaxLength(1000)]
        public string Reply { get; set; } = string.Empty;
    }
}
