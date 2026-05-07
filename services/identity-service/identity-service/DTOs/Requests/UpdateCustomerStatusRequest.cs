using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs.Requests
{
    public class UpdateCustomerStatusRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
