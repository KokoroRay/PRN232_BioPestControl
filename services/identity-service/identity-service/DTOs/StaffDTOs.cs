using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs
{
    // ─────────────────────────────────────────────────────────────
    // RESPONSE DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO trả về đầy đủ thông tin Staff (kèm User info + permissions)
    /// </summary>
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
        public List<PermissionDto> Permissions { get; set; } = new();

        // Audit
        public Guid? CreatedByAdminId { get; set; }
        public Guid? UpdatedByAdminId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO tóm tắt cho danh sách Staff (không load chi tiết permissions)
    /// </summary>
    public class StaffSummaryDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsFullAccess { get; set; }
        public int PermissionCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    // REQUEST DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Request tạo Staff mới — thông tin User cơ bản + phân quyền IAM
    /// </summary>
    public class CreateStaffRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ hoa, 1 chữ thường và 1 số.")]
        public string Password { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        // ── IAM ──────────────────────────────────────────────────
        /// <summary>
        /// true = cấp toàn bộ quyền Manager (PermissionIds bị bỏ qua).
        /// false = chỉ cấp các quyền trong PermissionIds.
        /// </summary>
        public bool IsFullAccess { get; set; } = false;

        /// <summary>
        /// Danh sách Permission ID muốn cấp (chỉ dùng khi IsFullAccess = false).
        /// </summary>
        public List<int> PermissionIds { get; set; } = new();
    }

    /// <summary>
    /// Request cập nhật thông tin Staff
    /// </summary>
    public class UpdateStaffRequest
    {
        [MaxLength(256)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        /// <summary>null = không đổi mật khẩu</summary>
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ hoa, 1 chữ thường và 1 số.")]
        public string? NewPassword { get; set; }

        // ── IAM ──────────────────────────────────────────────────
        public bool IsFullAccess { get; set; } = false;

        /// <summary>Danh sách Permission mới (thay thế toàn bộ quyền cũ).</summary>
        public List<int> PermissionIds { get; set; } = new();
    }

    /// <summary>
    /// Request tìm kiếm + lọc Staff
    /// </summary>
    public class StaffSearchRequest
    {
        /// <summary>Tìm theo email hoặc họ tên</summary>
        public string? Keyword { get; set; }

        /// <summary>Lọc theo trạng thái tài khoản (null = tất cả)</summary>
        public bool? IsActive { get; set; }

        /// <summary>Lọc theo loại quyền (null = tất cả)</summary>
        public bool? IsFullAccess { get; set; }

        // Phân trang
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sắp xếp: "createdAt" | "email" | "fullName"
        public string SortBy { get; set; } = "createdAt";
        public bool SortDesc { get; set; } = true;
    }

    /// <summary>Kết quả phân trang generic</summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }

    /// <summary>Request cập nhật quyền riêng</summary>
    public class UpdateStaffPermissionsRequest
    {
        public bool IsFullAccess { get; set; } = false;
        public List<int> PermissionIds { get; set; } = new();
    }

    /// <summary>Request kích hoạt / khóa Staff</summary>
    public class UpdateStaffStatusRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
