using inventory_service.Models;

namespace inventory_service.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        // View / Search / Filter Stock
        Task<IEnumerable<Product>> GetAllProductsAsync(string? searchQuery, string? sortBy, bool ascending = true);
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
