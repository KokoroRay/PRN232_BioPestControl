namespace identity_service.DTOs.Requests
{
    public class CustomerSearchRequest
    {
        public string? Keyword { get; set; } // Tìm theo email hoặc tên
        public bool? IsActive { get; set; }  // Lọc theo trạng thái
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "createdAt";
        public bool SortDesc { get; set; } = true;
    }
}
