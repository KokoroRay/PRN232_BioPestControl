namespace identity_service.DTOs.Responses
{
    // Unified Staff response — dùng cho cả list và detail view
    public class StaffDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        // Thông tin User account
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        // IAM
        public bool IsFullAccess { get; set; }
        public int PermissionCount { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new();

        // Audit
        public Guid? CreatedByAdminId { get; set; }
        public Guid? UpdatedByAdminId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
