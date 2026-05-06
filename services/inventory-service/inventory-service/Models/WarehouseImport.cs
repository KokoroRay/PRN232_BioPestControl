using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inventory_service.Models
{
    /// <summary>
    /// Bảng lịch sử nhập kho.
    /// Mỗi bản ghi đại diện cho một lần nhập một sản phẩm vào kho.
    /// Một phiếu nhập (ImportBatch) có thể chứa nhiều dòng sản phẩm.
    /// </summary>
    public class WarehouseImport
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Mã phiếu nhập kho — nhóm nhiều sản phẩm nhập cùng lúc</summary>
        [Required]
        [MaxLength(50)]
        public string BatchCode { get; set; } = string.Empty;

        /// <summary>ID sản phẩm được nhập</summary>
        public int ProductId { get; set; }

        /// <summary>Số lượng nhập</summary>
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng nhập phải lớn hơn 0")]
        public int QuantityImported { get; set; }

        /// <summary>Giá nhập (giá gốc từ nhà cung cấp)</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportPrice { get; set; }

        /// <summary>Nhà cung cấp</summary>
        [MaxLength(200)]
        public string? SupplierName { get; set; }

        /// <summary>Ghi chú thêm (lô hàng, ngày sản xuất, ...)</summary>
        [MaxLength(500)]
        public string? Note { get; set; }

        /// <summary>Ngày hết hạn của lô hàng (nếu có)</summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>ID người thực hiện nhập kho (Admin)</summary>
        public Guid ImportedByUserId { get; set; }

        /// <summary>Tên người nhập kho (lưu snapshot để tra cứu nhanh)</summary>
        [MaxLength(256)]
        public string? ImportedByUserName { get; set; }

        public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
