using catalog_service.DTOs.Requests;
using catalog_service.DTOs.Responses;
using catalog_service.Models;
using catalog_service.Repositories.Interfaces;
using catalog_service.Services.Interfaces;

namespace catalog_service.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IIdentityServiceClient _identityServiceClient;
        private readonly IAgriExpertServiceClient _agriExpertServiceClient;

        public ProductService(
            IProductRepository repository,
            IIdentityServiceClient identityServiceClient,
            IAgriExpertServiceClient agriExpertServiceClient)
        {
            _repository = repository;
            _identityServiceClient = identityServiceClient;
            _agriExpertServiceClient = agriExpertServiceClient;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            var responses = new List<ProductResponse>();

            foreach (var product in products)
            {
                responses.Add(await MapToResponseAsync(product));
            }

            return responses;
        }

        public async Task<IEnumerable<ProductResponse>> SearchByNameAsync(string name)
        {
            var products = await _repository.SearchByNameAsync(name);
            var responses = new List<ProductResponse>();

            foreach (var product in products)
            {
                responses.Add(await MapToResponseAsync(product));
            }

            return responses;
        }

        public async Task<ProductResponse?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }

            return await MapToResponseAsync(product);
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
                CreatedByAdminId = request.CreatedByAdminId
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
            var adminName = product.CreatedByAdminId.HasValue
                ? await _identityServiceClient.GetUserNameAsync(product.CreatedByAdminId.Value)
                : null;

            var staffName = product.ManagedByStaffId.HasValue
                ? await _identityServiceClient.GetUserNameAsync(product.ManagedByStaffId.Value)
                : null;

            var chemicalName = product.ChemicalProfileId.HasValue
                ? await _agriExpertServiceClient.GetChemicalNameAsync(product.ChemicalProfileId.Value)
                : null;

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
                ManagedByStaffName = staffName
            };
        }
    }
}
