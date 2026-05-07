using System.ComponentModel.DataAnnotations;

namespace trading_service.DTOs
{
    // DTO trả về thông tin toàn bộ giỏ hàng
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();

        // Tổng số lượng sản phẩm trong giỏ
        public int TotalQuantity => Items.Sum(i => i.Quantity);

        // Tổng tiền toàn bộ giỏ hàng
        public decimal TotalPrice => Items.Sum(i => i.SubTotal);

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // DTO trả về thông tin một dòng sản phẩm trong giỏ
    public class CartItemDto
    {
        public Guid Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        // Thành tiền = UnitPrice * Quantity
        public decimal SubTotal => UnitPrice * Quantity;

        public DateTime AddedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // Dữ liệu gửi lên khi thêm sản phẩm vào giỏ
    public class AddToCartRequest
    {
        [Required(ErrorMessage = "ProductId là bắt buộc.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal UnitPrice { get; set; }

        [MaxLength(500)]
        public string? ProductImageUrl { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; } = 1;
    }

    // Dữ liệu gửi lên khi cập nhật số lượng sản phẩm trong giỏ
    public class UpdateCartItemRequest
    {
        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }
    }
}
