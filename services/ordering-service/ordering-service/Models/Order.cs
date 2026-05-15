using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ordering_service.Models
{
    /// <summary>
    /// Trạng thái đơn hàng — tịnh tuyến, không quay lại trạng thái trước.
    /// WaitingConfirmation → Confirmed → Processing → Shipping → Delivered
    ///                     ↘                                    ↗
    ///                       Cancelled (chỉ khi ở WaitingConfirmation hoặc bởi Staff/Admin)
    /// </summary>
    public enum OrderStatus
    {
        WaitingConfirmation = 0, // Chờ xác nhận (Customer vừa đặt)
        Confirmed           = 1, // Đã xác nhận bởi Staff/Admin
        Processing          = 2, // Đang xử lý / đóng gói
        Shipping            = 3, // Đang giao hàng
        Delivered           = 4, // Đã giao thành công
        Cancelled           = 5  // Đã hủy
    }

    /// <summary>Trạng thái thanh toán của đơn hàng</summary>
    public enum PaymentStatus
    {
        Unpaid    = 0, // Chưa thanh toán (COD hoặc chờ PayOS)
        Paid      = 1, // Đã thanh toán
        Refunded  = 2  // Đã hoàn tiền (sau khi hủy đơn đã thanh toán)
    }

    /// <summary>Phương thức thanh toán</summary>
    public enum PaymentMethod
    {
        COD   = 0, // Thanh toán khi nhận hàng
        PayOS = 1  // Thanh toán online qua PayOS
    }

    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CustomerId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // ── Trạng thái đơn hàng ──
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.WaitingConfirmation;

        // ── Trạng thái thanh toán ──
        [Required]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        [Required]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

        // Thời điểm thanh toán (nếu đã thanh toán)
        public DateTime? PaidAt { get; set; }

        // Thời điểm hoàn tiền (nếu đã hoàn tiền)
        public DateTime? RefundedAt { get; set; }

        [MaxLength(500)]
        public string? ShippingAddress { get; set; }

        // ── Thông tin hủy đơn ──
        // ID người hủy (Customer hoặc Staff/Admin)
        public Guid? CancelledByUserId { get; set; }

        // "Customer" | "Staff" | "Admin"
        [MaxLength(20)]
        public string? CancelledByRole { get; set; }

        // Lý do hủy (bắt buộc với Staff/Admin, tùy chọn với Customer)
        [MaxLength(1000)]
        public string? CancellationReason { get; set; }

        public DateTime? CancelledAt { get; set; }

        // Navigation
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ProductImageUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [NotMapped]
        public decimal SubTotal => UnitPrice * Quantity;

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}
