using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs.Requests
{
    public class UpdateUserStatusRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
