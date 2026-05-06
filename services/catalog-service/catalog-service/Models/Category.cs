using System.ComponentModel.DataAnnotations;

namespace catalog_service.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        
        [MaxLength(500)]
        public string? Description { get; set; }

        public int? CreatedByAdminId { get; set; }
        public int? ManagedByStaffId { get; set; }
    }
}
