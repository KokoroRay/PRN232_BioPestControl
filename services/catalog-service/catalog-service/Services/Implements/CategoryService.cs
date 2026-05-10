using catalog_service.DTOs.Requests;
using catalog_service.DTOs.Responses;
using catalog_service.Models;
using catalog_service.Repositories.Interfaces;
using catalog_service.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace catalog_service.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IIdentityServiceClient _identityServiceClient;

        public CategoryService(ICategoryRepository repository, IIdentityServiceClient identityServiceClient)
        {
            _repository = repository;
            _identityServiceClient = identityServiceClient;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            var responses = new List<CategoryResponse>();
            foreach (var c in categories)
            {
                responses.Add(await MapToResponseAsync(c));
            }
            return responses;
        }

        public async Task<IEnumerable<CategoryResponse>> SearchByNameAsync(string name)
        {
            var categories = await _repository.SearchByNameAsync(name);
            var responses = new List<CategoryResponse>();
            foreach (var c in categories)
            {
                responses.Add(await MapToResponseAsync(c));
            }
            return responses;
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return null;
            return await MapToResponseAsync(category);
        }

        public async Task<CategoryResponse> AddAsync(CreateCategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                CreatedByAdminId = request.CreatedByAdminId
            };

            var addedCategory = await _repository.AddAsync(category);
            return await MapToResponseAsync(addedCategory);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            var existingCategory = await _repository.GetByIdAsync(id);
            if (existingCategory == null) return false;

            existingCategory.Name = request.Name;
            existingCategory.Description = request.Description;
            existingCategory.ManagedByStaffId = request.ManagedByStaffId;

            await _repository.UpdateAsync(existingCategory);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingCategory = await _repository.GetByIdAsync(id);
            if (existingCategory == null) return false;

            await _repository.DeleteAsync(existingCategory);
            return true;
        }

        private async Task<CategoryResponse> MapToResponseAsync(Category category)
        {
            string? adminName = category.CreatedByAdminId.HasValue 
                ? await _identityServiceClient.GetUserNameAsync(category.CreatedByAdminId.Value) 
                : null;
                
            string? staffName = category.ManagedByStaffId.HasValue 
                ? await _identityServiceClient.GetUserNameAsync(category.ManagedByStaffId.Value) 
                : null;

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedByAdminId = category.CreatedByAdminId,
                CreatedByAdminName = adminName,
                ManagedByStaffId = category.ManagedByStaffId,
                ManagedByStaffName = staffName
            };
        }
    }
}
