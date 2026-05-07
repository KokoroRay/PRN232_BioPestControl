using System.ComponentModel.DataAnnotations;

namespace identity_service.DTOs.Requests
{
    public class UpdateUserRoleRequest
    {
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;
    }
}
