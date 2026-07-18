using System.ComponentModel.DataAnnotations;

namespace catalog_service.DTOs
{
    public class CropRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
