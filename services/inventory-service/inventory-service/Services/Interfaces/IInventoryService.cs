using inventory_service.DTOs.Requests;
using inventory_service.DTOs.Responses;

namespace inventory_service.Services.Interfaces
{
    public interface IInventoryService
    {
        // View stock for Staff (no history)
        Task<PagedResult<ProductStockResponse>> GetProductStocksAsync(string? searchQuery, string? sortBy, bool ascending, int page = 1, int pageSize = 10);
        
        // View details for Admin (with history)
        Task<PagedResult<ProductDetailResponse>> GetProductDetailsAsync(string? searchQuery, string? sortBy, bool ascending, int page = 1, int pageSize = 10);
        Task<ProductDetailResponse?> GetProductDetailByIdAsync(int id);
        
        // Import multiple products (Admin)
        Task<ImportBatchSummaryResponse> ImportProductsAsync(ImportProductsRequest request, Guid adminId, string adminName);
    }
}
