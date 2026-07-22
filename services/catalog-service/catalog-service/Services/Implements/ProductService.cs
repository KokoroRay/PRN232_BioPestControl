using catalog_service.DTOs.Requests;
using catalog_service.DTOs.Responses;
using catalog_service.Models;
using catalog_service.Repositories.Interfaces;
using catalog_service.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace catalog_service.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IIdentityServiceClient _identityServiceClient;
        private readonly IAgriExpertServiceClient _agriExpertServiceClient;
        private readonly IDistributedCache _cache;

        public ProductService(
            IProductRepository repository,
            IIdentityServiceClient identityServiceClient,
            IAgriExpertServiceClient agriExpertServiceClient,
            IDistributedCache cache)
        {
            _repository = repository;
            _identityServiceClient = identityServiceClient;
            _agriExpertServiceClient = agriExpertServiceClient;
            _cache = cache;
        }

        public async Task<PagedResult<ProductResponse>> GetAllAsync(ProductFilterRequest? filter = null)
        {
            var cacheKey = $"Products_GetAll_{JsonSerializer.Serialize(filter)}";
            string? cachedData = null;

            try
            {
                cachedData = await _cache.GetStringAsync(cacheKey);
            }
            catch (Exception ex)
            {
                // Log exception if possible, or just ignore to fallback to DB
                Console.WriteLine($"Redis Cache Error (Get): {ex.Message}");
            }

            if (!string.IsNullOrEmpty(cachedData))
            {
                try
                {
                    var resultFromCache = JsonSerializer.Deserialize<PagedResult<ProductResponse>>(cachedData);
                    if (resultFromCache != null)
                    {
                        return resultFromCache;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Redis Cache Deserialize Error: {ex.Message}");
                }
            }

            var pagedProducts = await _repository.GetAllAsync(filter);

            var mapTasks = pagedProducts.Items.Select(product => MapToResponseAsync(product));
            var responses = (await Task.WhenAll(mapTasks)).ToList();

            var result = new PagedResult<ProductResponse>
            {
                Items = responses,
                TotalCount = pagedProducts.TotalCount,
                Page = pagedProducts.Page,
                PageSize = pagedProducts.PageSize
            };

            try
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis Cache Error (Set): {ex.Message}");
            }

            return result;
        }

        public async Task<IEnumerable<ProductResponse>> SearchByNameAsync(string name)
        {
            var products = await _repository.SearchByNameAsync(name);

            var mapTasks = products.Select(product => MapToResponseAsync(product));
            var responses = (await Task.WhenAll(mapTasks)).ToList();

            return responses;
        }

        public async Task<ProductResponse?> GetByIdAsync(int id)
        {
            var cacheKey = $"Product_GetById_{id}";
            string? cachedData = null;

            try
            {
                cachedData = await _cache.GetStringAsync(cacheKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis Cache Error (Get): {ex.Message}");
            }

            if (!string.IsNullOrEmpty(cachedData))
            {
                try
                {
                    var resultFromCache = JsonSerializer.Deserialize<ProductResponse>(cachedData);
                    if (resultFromCache != null)
                    {
                        return resultFromCache;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Redis Cache Deserialize Error: {ex.Message}");
                }
            }

            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }

            var response = await MapToResponseAsync(product);

            try
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), cacheOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis Cache Error (Set): {ex.Message}");
            }

            return response;
        }

        public async Task<ProductCommandResult> AddAsync(CreateProductRequest request)
        {
            var sku = request.SKU.Trim();
            var name = request.Name.Trim();

            if (await _repository.ExistsBySkuAsync(sku))
            {
                return ProductCommandResult.Fail(ProductCommandError.DuplicateSku);
            }

            if (!await _repository.ExistsByCategoryIdAsync(request.CategoryId))
            {
                return ProductCommandResult.Fail(ProductCommandError.CategoryNotFound);
            }

            if (request.ChemicalProfileId.HasValue &&
                !await _agriExpertServiceClient.ExistsChemicalProfileAsync(request.ChemicalProfileId.Value))
            {
                return ProductCommandResult.Fail(ProductCommandError.ChemicalProfileNotFound);
            }

            var product = new Product
            {
                SKU = sku,
                Name = name,
                Description = request.Description,
                Unit = request.Unit,
                UnitPrice = request.UnitPrice,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId,
                ChemicalProfileId = request.ChemicalProfileId,
                IsActive = request.IsActive,
                CreatedByAdminId = request.CreatedByAdminId,
                ProductCrops = request.CropIds?.Select(cId => new ProductCrop { CropId = cId }).ToList() ?? new List<ProductCrop>()
            };

            var added = await _repository.AddAsync(product);
            var withCategory = await _repository.GetByIdAsync(added.Id);

            if (withCategory == null)
            {
                return ProductCommandResult.Fail(ProductCommandError.ProductNotFound);
            }

            return ProductCommandResult.Ok(await MapToResponseAsync(withCategory));
        }

        public async Task<ProductCommandResult> UpdateAsync(int id, UpdateProductRequest request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return ProductCommandResult.Fail(ProductCommandError.ProductNotFound);
            }

            var sku = request.SKU.Trim();
            var name = request.Name.Trim();

            if (await _repository.ExistsBySkuExceptIdAsync(sku, id))
            {
                return ProductCommandResult.Fail(ProductCommandError.DuplicateSku);
            }

            if (!await _repository.ExistsByCategoryIdAsync(request.CategoryId))
            {
                return ProductCommandResult.Fail(ProductCommandError.CategoryNotFound);
            }

            if (request.ChemicalProfileId.HasValue &&
                !await _agriExpertServiceClient.ExistsChemicalProfileAsync(request.ChemicalProfileId.Value))
            {
                return ProductCommandResult.Fail(ProductCommandError.ChemicalProfileNotFound);
            }

            existing.SKU = sku;
            existing.Name = name;
            existing.Description = request.Description;
            existing.Unit = request.Unit;
            existing.UnitPrice = request.UnitPrice;
            existing.ImageUrl = request.ImageUrl;
            existing.CategoryId = request.CategoryId;
            existing.ChemicalProfileId = request.ChemicalProfileId;
            existing.IsActive = request.IsActive;
            existing.ManagedByStaffId = request.ManagedByStaffId;
            existing.UpdatedAt = DateTime.UtcNow;

            // Update ProductCrops
            existing.ProductCrops.Clear();
            if (request.CropIds != null && request.CropIds.Any())
            {
                foreach (var cropId in request.CropIds)
                {
                    existing.ProductCrops.Add(new ProductCrop { CropId = cropId, ProductId = id });
                }
            }

            await _repository.UpdateAsync(existing);
            return ProductCommandResult.Ok();
        }

        public async Task<ProductCommandResult> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return ProductCommandResult.Fail(ProductCommandError.ProductNotFound);
            }

            await _repository.DeleteAsync(existing);
            return ProductCommandResult.Ok();
        }

        private async Task<ProductResponse> MapToResponseAsync(Product product)
        {
            var adminNameTask = product.CreatedByAdminId.HasValue
                ? _identityServiceClient.GetUserNameAsync(product.CreatedByAdminId.Value)
                : Task.FromResult<string?>(null);

            var staffNameTask = product.ManagedByStaffId.HasValue
                ? _identityServiceClient.GetUserNameAsync(product.ManagedByStaffId.Value)
                : Task.FromResult<string?>(null);

            var chemicalNameTask = product.ChemicalProfileId.HasValue
                ? _agriExpertServiceClient.GetChemicalNameAsync(product.ChemicalProfileId.Value)
                : Task.FromResult<string?>(null);

            await Task.WhenAll(adminNameTask, staffNameTask, chemicalNameTask);

            var adminName = adminNameTask.Result;
            var staffName = staffNameTask.Result;
            var chemicalName = chemicalNameTask.Result;

            return new ProductResponse
            {
                Id = product.Id,
                SKU = product.SKU,
                Name = product.Name,
                Description = product.Description,
                Unit = product.Unit,
                UnitPrice = product.UnitPrice,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                ChemicalProfileId = product.ChemicalProfileId,
                ChemicalName = chemicalName,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                CreatedByAdminId = product.CreatedByAdminId,
                CreatedByAdminName = adminName,
                ManagedByStaffId = product.ManagedByStaffId,
                ManagedByStaffName = staffName,
                CropIds = product.ProductCrops?.Select(pc => pc.CropId).ToList() ?? new List<int>()
            };
        }
    }
}
