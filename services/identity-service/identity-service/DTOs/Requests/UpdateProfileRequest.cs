using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs.Requests
{
    public class UpdateProfileRequest
    {
        [MaxLength(256)]
        public string? FullName { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        [MaxLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string? PhoneNumber { get; set; }
    }
}
