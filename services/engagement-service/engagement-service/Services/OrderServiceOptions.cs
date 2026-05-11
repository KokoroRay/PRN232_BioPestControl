namespace engagement_service.Services
{
    /// <summary>Cấu hình gọi order-service để xác minh đơn đã giao và có dòng sản phẩm trước khi cho tạo feedback.</summary>
    public class OrderServiceOptions
    {
        public const string SectionName = "OrderService";

        /// <summary>
        /// true: gọi HTTP qua HttpOrderPurchaseEligibilityService tới order-service.
        /// false: dùng NoOpPurchaseEligibilityService (luôn cho phép — dev).
        /// </summary>
        public bool UseHttpValidation { get; set; }

        /// <summary>Base URL order-service, ví dụ https://orders.internal (không dấu / cuối).</summary>
        public string? BaseUrl { get; set; }

        /// <summary>Đường dẫn tương đối sau BaseUrl. Query customerId, orderId, productId được gắn tự động.</summary>
        public string EligibilityRelativePath { get; set; } = "api/orders/review-eligibility";

        /// <summary>Timeout gọi order-service.</summary>
        public int TimeoutSeconds { get; set; } = 10;

        /// <summary>Tùy chọn: tên header (ví dụ X-Api-Key).</summary>
        public string? ApiKeyHeaderName { get; set; }

        public string? ApiKey { get; set; }
    }
}
