using catalog_service.DTOs.Requests;
using catalog_service.DTOs.Responses;

namespace catalog_service.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllAsync(ProductFilterRequest filter = null);
        Task<IEnumerable<ProductResponse>> SearchByNameAsync(string name);
        Task<ProductResponse?> GetByIdAsync(int id);
        Task<ProductCommandResult> AddAsync(CreateProductRequest request);
        Task<ProductCommandResult> UpdateAsync(int id, UpdateProductRequest request);
        Task<ProductCommandResult> DeleteAsync(int id);
    }
}
