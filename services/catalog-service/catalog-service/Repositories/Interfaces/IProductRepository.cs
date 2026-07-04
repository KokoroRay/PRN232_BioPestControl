using catalog_service.Models;
using catalog_service.DTOs.Requests;

namespace catalog_service.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(ProductFilterRequest filter = null);
        Task<IEnumerable<Product>> SearchByNameAsync(string name);
        Task<Product?> GetByIdAsync(int id);
        Task<bool> ExistsByCategoryIdAsync(int categoryId);
        Task<bool> ExistsBySkuAsync(string sku);
        Task<bool> ExistsBySkuExceptIdAsync(string sku, int id);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
