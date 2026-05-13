namespace article_service.DTOs.Responses
{
    public class ArticleResponse
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Summary { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string Status { get; set; } = null!;
        public string? Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedByStaffId { get; set; }
        public int? ManagedByAdminId { get; set; }
    }
}
