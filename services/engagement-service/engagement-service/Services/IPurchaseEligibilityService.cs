namespace engagement_service.Services
{
    /// <summary>
    /// Kiểm tra khách đã mua và giao thành công sản phẩm trong đơn (tương đương monolith HasPurchasedAndDeliveredAsync).
    /// Bật gọi order-service bằng cấu hình OrderService:UseHttpValidation (HttpOrderPurchaseEligibilityService); tắt = NoOp.
    /// Chi tiết: INTEGRATION.md.
    /// </summary>
    public interface IPurchaseEligibilityService
    {
        Task<bool> HasDeliveredProductInOrderAsync(int customerId, int orderId, int productId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Dùng khi OrderService:UseHttpValidation = false — luôn cho phép (dev).
    /// </summary>
    public class NoOpPurchaseEligibilityService : IPurchaseEligibilityService
    {
        public Task<bool> HasDeliveredProductInOrderAsync(int customerId, int orderId, int productId, CancellationToken cancellationToken = default)
            => Task.FromResult(true);
    }
}
