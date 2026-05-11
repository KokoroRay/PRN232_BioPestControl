using System.ComponentModel.DataAnnotations;

namespace engagement_service.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }

        public virtual ICollection<FeedbackImage> Images { get; set; } = new List<FeedbackImage>();
        public virtual ICollection<FeedbackReviewTag> FeedbackTags { get; set; } = new List<FeedbackReviewTag>();
        public virtual ICollection<FeedbackReply> Replies { get; set; } = new List<FeedbackReply>();
    }

    public class FeedbackImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public int FeedbackId { get; set; }
        public virtual Feedback Feedback { get; set; } = null!;
    }

    public class FeedbackReply
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Reply { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int FeedbackId { get; set; }
        public int StaffId { get; set; }

        public virtual Feedback Feedback { get; set; } = null!;
    }

    public class FeedbackReviewTag
    {
        public int FeedbackId { get; set; }
        public int ReviewTagId { get; set; }

        public virtual Feedback Feedback { get; set; } = null!;
    }
}
