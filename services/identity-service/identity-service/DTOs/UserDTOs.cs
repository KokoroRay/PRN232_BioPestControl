using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs
{
    // Dữ liệu trả về khi lấy thông tin User (không bao gồm PasswordHash, GoogleId để bảo mật)
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // Dữ liệu Admin/User gửi lên để cập nhật thông tin User
    public class UpdateUserRequest
    {
        [MaxLength(256)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(256)]
        public string? AvatarUrl { get; set; }
    }

    // Dữ liệu Admin gửi lên để thay đổi Role của User
    public class UpdateUserRoleRequest
    {
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;
    }

    // Dữ liệu Admin gửi lên để thay đổi trạng thái kích hoạt / khóa tài khoản
    public class UpdateUserStatusRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
