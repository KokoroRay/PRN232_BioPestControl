using inventory_service.DTOs.Requests;
using inventory_service.DTOs.Responses;

namespace inventory_service.Services.Interfaces
{
    public interface IInventoryService
    {
        // View stock for Staff (no history)
        Task<IEnumerable<ProductStockResponse>> GetProductStocksAsync(string? searchQuery, string? sortBy, bool ascending);
        
        // View details for Admin (with history)
        Task<IEnumerable<ProductDetailResponse>> GetProductDetailsAsync(string? searchQuery, string? sortBy, bool ascending);
        Task<ProductDetailResponse?> GetProductDetailByIdAsync(int id);
        
        // Import multiple products (Admin)
        Task<ImportBatchSummaryResponse> ImportProductsAsync(ImportProductsRequest request, Guid adminId, string adminName);
    }
}
