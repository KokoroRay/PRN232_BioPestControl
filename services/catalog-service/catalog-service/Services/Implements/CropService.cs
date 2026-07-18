using catalog_service.DTOs;
using catalog_service.Models;
using catalog_service.Repositories.Interfaces;
using catalog_service.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace catalog_service.Services.Implements
{
    public class CropService : ICropService
    {
        private readonly ICropRepository _repository;

        public CropService(ICropRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CropResponse>> GetAllAsync(bool includeInactive = false)
        {
            var crops = await _repository.GetAllAsync(includeInactive);
            return crops.Select(c => new CropResponse
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive
            });
        }

        public async Task<CropProfileResponse?> GetByIdAsync(int id)
        {
            var crop = await _repository.GetByIdAsync(id);
            if (crop == null) return null;

            return MapToProfileResponse(crop);
        }

        public async Task<CropProfileResponse?> GetBySlugAsync(string slug)
        {
            var crop = await _repository.GetBySlugAsync(slug);
            if (crop == null) return null;

            return MapToProfileResponse(crop);
        }

        private CropProfileResponse MapToProfileResponse(Crop crop)
        {
            var response = new CropProfileResponse
            {
                Id = crop.Id,
                Name = crop.Name,
                Slug = crop.Slug,
                Description = crop.Description,
                ImageUrl = crop.ImageUrl,
                IsActive = crop.IsActive,
                Products = crop.ProductCrops
                    .Where(pc => pc.Product.IsActive)
                    .Select(pc => new CropProductDetail
                    {
                        ProductId = pc.Product.Id,
                        ProductName = pc.Product.Name,
                        ProductImageUrl = pc.Product.ImageUrl,
                        UsageInstruction = pc.UsageInstruction ?? "",
                        CategoryId = pc.Product.CategoryId,
                        CategoryName = pc.Product.Category?.Name ?? ""
                    }).ToList()
            };
            return response;
        }

        public async Task<CropResponse> CreateAsync(CropRequest request)
        {
            var crop = new Crop
            {
                Name = request.Name,
                Slug = GenerateSlug(request.Name),
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                IsActive = request.IsActive
            };
            
            await _repository.CreateAsync(crop);
            return new CropResponse
            {
                Id = crop.Id,
                Name = crop.Name,
                Slug = crop.Slug,
                Description = crop.Description,
                ImageUrl = crop.ImageUrl,
                IsActive = crop.IsActive
            };
        }

        public async Task<CropResponse> UpdateAsync(int id, CropRequest request)
        {
            var crop = await _repository.GetByIdAsync(id);
            if (crop == null)
            {
                throw new System.Exception("Crop not found");
            }

            crop.Name = request.Name;
            crop.Slug = GenerateSlug(request.Name);
            crop.Description = request.Description;
            crop.ImageUrl = request.ImageUrl;
            crop.IsActive = request.IsActive;

            await _repository.UpdateAsync(crop);
            return new CropResponse
            {
                Id = crop.Id,
                Name = crop.Name,
                Slug = crop.Slug,
                Description = crop.Description,
                ImageUrl = crop.ImageUrl,
                IsActive = crop.IsActive
            };
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        private string GenerateSlug(string phrase)
        {
            string str = phrase.ToLower().Replace(" ", "-");
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");
            return str;
        }
    }
}
