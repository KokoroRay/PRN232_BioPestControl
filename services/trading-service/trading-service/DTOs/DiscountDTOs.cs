using System.ComponentModel.DataAnnotations;

namespace trading_service.DTOs
{
    public class DiscountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int ProductId { get; set; }
        public int? CreatedByAdminId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>True khi IsActive và thời điểm hiện tại nằm trong [StartDate, EndDate] (UTC).</summary>
        public bool IsCurrentlyRunning { get; set; }
    }

    public class CreateDiscountRequest
    {
        [Required(ErrorMessage = "Tên khuyến mãi là bắt buộc.")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Phần trăm giảm phải từ 0 đến 100.")]
        public double DiscountPercent { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "ProductId phải lớn hơn 0.")]
        public int ProductId { get; set; }

        public int? CreatedByAdminId { get; set; }
    }

    public class UpdateDiscountRequest
    {
        [Required(ErrorMessage = "Tên khuyến mãi là bắt buộc.")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Phần trăm giảm phải từ 0 đến 100.")]
        public double DiscountPercent { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ProductId phải lớn hơn 0.")]
        public int ProductId { get; set; }
    }
}
