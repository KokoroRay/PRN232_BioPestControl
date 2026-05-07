namespace identity_service.DTOs.Requests
{
    public class UserSearchRequest
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
        public string? Role { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "createdAt";
        public bool SortDesc { get; set; } = true;
    }
}
