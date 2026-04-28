using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs
{
    // Dữ liệu client gửi lên khi Đăng Ký
    public class RegisterRequest
    {
        // Yêu cầu bắt buộc phải có email và đúng định dạng
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Yêu cầu mật khẩu tối thiểu 6 ký tự
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        // Họ và tên (không bắt buộc)
        public string? FullName { get; set; }
    }

    // Dữ liệu client gửi lên khi Đăng Nhập
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Dữ liệu client gửi lên khi đăng nhập bằng Google
    public class GoogleLoginRequest
    {
        // Google ID Token nhận được từ client (Web, Android, iOS...)
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }

    // Dữ liệu trả về cho client sau khi đăng nhập/đăng ký thành công
    public class AuthResponse
    {
        // Chuỗi JWT Token dùng để đính kèm vào Header khi gọi các API bị bảo mật
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}
