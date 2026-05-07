using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs.Requests
{
    public class UpdateStaffRequest
    {
        [MaxLength(256)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        // null = không đổi mật khẩu
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ hoa, 1 chữ thường và 1 số.")]
        public string? NewPassword { get; set; }

        public bool IsFullAccess { get; set; } = false;

        // Danh sách Permission mới (thay thế toàn bộ quyền cũ)
        public List<int> PermissionIds { get; set; } = new();
    }
}
