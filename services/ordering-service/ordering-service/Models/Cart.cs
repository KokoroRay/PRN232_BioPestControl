using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ordering_service.Models
{
    // Bảng Cart: mỗi Customer có DUY NHẤT một giỏ hàng
    public class Cart
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // ID của Customer (lấy từ JWT token của identity-service)
        [Required]
        public Guid CustomerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation: Một Cart có nhiều CartItem
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }

    // Bảng CartItem: mỗi dòng = 1 sản phẩm trong giỏ hàng
    public class CartItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Liên kết về Cart cha
        [Required]
        public Guid CartId { get; set; }

        // ID sản phẩm tham chiếu sang catalog-service (không dùng FK vì microservice)
        [Required]
        public int ProductId { get; set; }

        // Lưu snapshot tên sản phẩm tại thời điểm thêm (tránh gọi lại catalog-service mỗi lần)
        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        // Snapshot giá tại thời điểm thêm vào giỏ
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // Ảnh đại diện sản phẩm (tùy chọn)
        [MaxLength(500)]
        public string? ProductImageUrl { get; set; }

        // Số lượng sản phẩm trong giỏ (tối thiểu 1)
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }

        // Thành tiền = UnitPrice * Quantity (computed, không lưu DB)
        [NotMapped]
        public decimal SubTotal => UnitPrice * Quantity;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation về Cart cha
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; } = null!;
    }
}
