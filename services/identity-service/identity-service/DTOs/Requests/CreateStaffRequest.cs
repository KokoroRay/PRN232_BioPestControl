using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs.Requests
{
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

        // true = cấp toàn bộ quyền Manager, false = chỉ cấp quyền trong PermissionIds
        public bool IsFullAccess { get; set; } = false;

        // Danh sách Permission ID muốn cấp (chỉ dùng khi IsFullAccess = false)
        public List<int> PermissionIds { get; set; } = new();
    }
}
