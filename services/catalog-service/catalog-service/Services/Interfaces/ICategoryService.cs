using catalog_service.DTOs.Requests;
using catalog_service.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace catalog_service.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<IEnumerable<CategoryResponse>> SearchByNameAsync(string name);
        Task<CategoryResponse?> GetByIdAsync(int id);
        Task<CategoryResponse> AddAsync(CreateCategoryRequest request);
        Task<bool> UpdateAsync(int id, UpdateCategoryRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
