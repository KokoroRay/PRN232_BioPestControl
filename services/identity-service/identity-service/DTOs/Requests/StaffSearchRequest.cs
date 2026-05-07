namespace identity_service.DTOs.Requests
{
    public class StaffSearchRequest
    {
        // Tìm theo email hoặc họ tên
        public string? Keyword { get; set; }

        // Lọc theo trạng thái tài khoản (null = tất cả)
        public bool? IsActive { get; set; }

        // Lọc theo loại quyền (null = tất cả)
        public bool? IsFullAccess { get; set; }

        // Phân trang
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sắp xếp: "createdAt" | "email" | "fullName"
        public string SortBy { get; set; } = "createdAt";
        public bool SortDesc { get; set; } = true;
    }
}
