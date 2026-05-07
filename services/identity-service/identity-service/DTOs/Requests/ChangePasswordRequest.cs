using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs.Requests
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu cũ là bắt buộc.")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
        [MinLength(8, ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu mới phải có ít nhất 1 chữ hoa, 1 chữ thường và 1 số.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
