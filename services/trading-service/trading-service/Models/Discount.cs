using System.ComponentModel.DataAnnotations;

namespace trading_service.Models
{
    /// <summary>
    /// Khuyến mãi theo sản phẩm (ProductId tham chiếu logic tới catalog — không FK cross-service).
    /// </summary>
    public class Discount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Phần trăm giảm (0–100).</summary>
        [Range(0, 100)]
        public double DiscountPercent { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int ProductId { get; set; }

        public int? CreatedByAdminId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
