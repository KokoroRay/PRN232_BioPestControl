using System.ComponentModel.DataAnnotations;

namespace catalog_service.DTOs.Requests
{
    public class UpdateCategoryRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        
        [MaxLength(500)]
        public string? Description { get; set; }

        public int? ManagedByStaffId { get; set; }
    }
}
