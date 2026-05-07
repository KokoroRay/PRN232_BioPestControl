using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs.Requests
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = string.Empty;
    }
}
