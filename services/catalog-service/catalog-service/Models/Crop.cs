using System.ComponentModel.DataAnnotations;

namespace catalog_service.Models
{
    public class Crop
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<ProductCrop> ProductCrops { get; set; } = new List<ProductCrop>();
    }
}
