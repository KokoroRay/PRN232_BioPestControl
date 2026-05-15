using System.ComponentModel.DataAnnotations;

namespace catalog_service.DTOs.Requests
{
    public class UpdateProductRequest
    {
        [Required]
        [MaxLength(100)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal UnitPrice { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int? ChemicalProfileId { get; set; }
        public bool IsActive { get; set; } = true;
        public int? ManagedByStaffId { get; set; }
    }
}
