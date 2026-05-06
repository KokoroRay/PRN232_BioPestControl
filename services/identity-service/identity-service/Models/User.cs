using System.ComponentModel.DataAnnotations;

namespace identity_service.Models
{
    public class User
    {
        // Khóa chính của bảng User, tự động sinh ra một GUID mới
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Email bắt buộc phải có, đúng định dạng và độ dài tối đa 256 ký tự
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        // Lưu mật khẩu đã được mã hóa (hash), cho phép null vì có thể đăng nhập bằng Google mà không cần mật khẩu
        public string? PasswordHash { get; set; }

        // Họ và tên người dùng
        [MaxLength(256)]
        public string? FullName { get; set; }

        // ID của tài khoản Google nếu người dùng đăng nhập bằng Google
        public string? GoogleId { get; set; }

        // Hình đại diện (Có thể lấy từ Google)
        public string? AvatarUrl { get; set; }

        // Số điện thoại liên hệ
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        // Vai trò của người dùng trong hệ thống (VD: Admin, Customer, Technician)
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Customer";

        // Trạng thái tài khoản (True = Đang hoạt động, False = Bị khóa)
        public bool IsActive { get; set; } = true;

        // Thời gian tạo tài khoản
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Thời gian cập nhật thông tin lần cuối
        public DateTime? UpdatedAt { get; set; }
    }
}
