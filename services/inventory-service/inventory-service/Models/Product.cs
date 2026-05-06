using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inventory_service.Models
{
    /// <summary>
    /// Bảng sản phẩm — dùng để tham chiếu trong kho hàng.
    /// Mỗi sản phẩm có mã SKU và số lượng tồn kho hiện tại.
    /// </summary>
    public class Product
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Mã sản phẩm nội bộ (SKU)</summary>
        [Required]
        [MaxLength(100)]
        public string SKU { get; set; } = string.Empty;

        /// <summary>Tên thương mại của sản phẩm</summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Mô tả ngắn</summary>
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>Đơn vị tính (lít, kg, chai, ...)</summary>
        [MaxLength(50)]
        public string? Unit { get; set; }

        /// <summary>Số lượng tồn kho hiện tại</summary>
        public int StockQuantity { get; set; } = 0;

        /// <summary>Ngưỡng cảnh báo hàng tồn thấp</summary>
        public int LowStockThreshold { get; set; } = 10;

        /// <summary>Trạng thái sản phẩm</summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual ICollection<WarehouseImport> WarehouseImports { get; set; } = new List<WarehouseImport>();
    }
}
