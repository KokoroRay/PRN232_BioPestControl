using System.ComponentModel.DataAnnotations;

namespace ordering_service.DTOs
{
    // ─────────────────────────────────────────────
    // RESPONSE DTOs
    // ─────────────────────────────────────────────

    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Order status
        public string Status { get; set; } = string.Empty;
        public int StatusCode { get; set; }

        // Payment
        public string PaymentStatus { get; set; } = string.Empty;
        public int PaymentStatusCode { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public DateTime? RefundedAt { get; set; }

        public decimal TotalAmount { get; set; }
        public string? ShippingAddress { get; set; }

        // Cancellation info
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledByRole { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }

        public List<OrderItemResponse> Items { get; set; } = new();
    }

    public class OrderItemResponse
    {
        public Guid Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;
    }

    // ─────────────────────────────────────────────
    // REQUEST DTOs
    // ─────────────────────────────────────────────

    /// <summary>Customer đặt hàng từ cart</summary>
    public class PlaceOrderRequest
    {
        [MaxLength(500)]
        public string? ShippingAddress { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "COD"; // "COD" | "PayOS"
    }

    /// <summary>Staff/Admin cập nhật trạng thái đơn hàng (tịnh tuyến)</summary>
    public class UpdateOrderStatusRequest
    {
        [Required]
        public string NewStatus { get; set; } = string.Empty;
    }

    /// <summary>
    /// Staff/Admin hủy đơn hàng — bắt buộc có lý do.
    /// Customer hủy không cần request body.
    /// </summary>
    public class CancelOrderRequest
    {
        [Required(ErrorMessage = "Lý do hủy là bắt buộc.")]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>Bộ lọc danh sách đơn hàng (dùng chung cho Customer / Staff / Admin)</summary>
    public class OrderFilterRequest
    {
        public string? Status { get; set; }     // "WaitingConfirmation" | "Confirmed" | ...
        public string? Search { get; set; }     // Tìm theo OrderId hoặc tên sản phẩm
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
