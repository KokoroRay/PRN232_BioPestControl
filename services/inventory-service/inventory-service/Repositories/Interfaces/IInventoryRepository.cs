using inventory_service.Models;

namespace inventory_service.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        // View / Search / Filter Stock
        Task<inventory_service.DTOs.Responses.PagedResult<Product>> GetAllProductsAsync(string? searchQuery, string? sortBy, bool ascending = true, int page = 1, int pageSize = 10);
        Task<Product?> GetProductBySkuAsync(string sku);
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> GetProductWithImportHistoryAsync(int id);
        
        // Import
        Task AddWarehouseImportsAsync(IEnumerable<WarehouseImport> imports);
        Task UpdateProductStocksAsync(IEnumerable<Product> products);
        
        // Transaction support
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
