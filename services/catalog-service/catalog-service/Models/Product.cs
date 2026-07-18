using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace catalog_service.Models
{
    public class Product
    {
        // Mã định danh duy nhất cho sản phẩm, tự tăng.
        [Key]
        public int Id { get; set; }

        // Stock Keeping Unit - mã định danh duy nhất cho sản phẩm, do admin tự tạo và quản lý.
        [Required]
        [MaxLength(100)]
        public string SKU { get; set; } = string.Empty;

        // Tên sản phẩm, do admin tự tạo và quản lý.
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        // Mô tả chi tiết sản phẩm, do admin tự tạo và quản lý.
        [MaxLength(2000)]
        public string? Description { get; set; }
        // Đơn vị tính (ví dụ: kg, lít, gói, chai...), do admin tự tạo và quản lý.
        [MaxLength(50)]
        public string? Unit { get; set; }

        // Giá bán lẻ đề xuất, do admin tự tạo và quản lý. 
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // URL hình ảnh sản phẩm, do admin tự tạo và quản lý.
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        // Mã định danh của danh mục sản phẩm, liên kết đến bảng Category. Do admin tự tạo và quản lý.
        [Required]
        public int CategoryId { get; set; }

        // Reference to agri-expert-service chemical profile (if applicable).
        public int? ChemicalProfileId { get; set; }

        // Trạng thái hoạt động của sản phẩm (true = hiển thị, false = ẩn), do admin tự quản lý.
        public bool IsActive { get; set; } = true;

        // Thông tin quản lý
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Mã định danh của admin đã tạo sản phẩm, do hệ thống tự quản lý.
        public int? CreatedByAdminId { get; set; }
        public int? ManagedByStaffId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<ProductCrop> ProductCrops { get; set; } = new List<ProductCrop>();
    }
}
