using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs.Requests
{
    public class UpdateStaffStatusRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
