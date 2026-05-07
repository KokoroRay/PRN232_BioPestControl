using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs.Requests
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP là bắt buộc.")]
        [MaxLength(6)]
        public string Otp { get; set; } = string.Empty;
    }
}
