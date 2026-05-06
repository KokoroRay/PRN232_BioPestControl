using inventory_service.DTOs.Requests;
using inventory_service.DTOs.Responses;
using inventory_service.Models;
using inventory_service.Repositories.Interfaces;
using inventory_service.Services.Interfaces;

namespace inventory_service.Services.Implements
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;

        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductStockResponse>> GetProductStocksAsync(string? searchQuery, string? sortBy, bool ascending)
        {
            var products = await _repository.GetAllProductsAsync(searchQuery, sortBy, ascending);
            return products.Select(p => new ProductStockResponse
            {
                Id = p.Id,
                SKU = p.SKU,
                Name = p.Name,
                Description = p.Description,
                Unit = p.Unit,
                StockQuantity = p.StockQuantity,
                LowStockThreshold = p.LowStockThreshold,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        public async Task<IEnumerable<ProductDetailResponse>> GetProductDetailsAsync(string? searchQuery, string? sortBy, bool ascending)
        {
            var products = await _repository.GetAllProductsAsync(searchQuery, sortBy, ascending);
            // Cần query thêm lịch sử nếu trả về list. Nhưng thường list chỉ cần tồn kho.
            // Để đơn giản, map từ Entity sang DTO, có thể chưa bao gồm lịch sử chi tiết ở dạng List.
            return products.Select(p => new ProductDetailResponse
            {
                Id = p.Id,
                SKU = p.SKU,
                Name = p.Name,
                Description = p.Description,
                Unit = p.Unit,
                StockQuantity = p.StockQuantity,
                LowStockThreshold = p.LowStockThreshold,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                ImportHistory = p.WarehouseImports.Select(wi => new WarehouseImportResponse
                {
                    Id = wi.Id,
                    BatchCode = wi.BatchCode,
                    ProductId = wi.ProductId,
                    QuantityImported = wi.QuantityImported,
                    ImportPrice = wi.ImportPrice,
                    SupplierName = wi.SupplierName,
                    Note = wi.Note,
                    ExpirationDate = wi.ExpirationDate,
                    ImportedByUserId = wi.ImportedByUserId,
                    ImportedByUserName = wi.ImportedByUserName,
                    ImportedAt = wi.ImportedAt
                }).ToList()
            });
        }

        public async Task<ProductDetailResponse?> GetProductDetailByIdAsync(int id)
        {
            var p = await _repository.GetProductWithImportHistoryAsync(id);
            if (p == null) return null;

            return new ProductDetailResponse
            {
                Id = p.Id,
                SKU = p.SKU,
                Name = p.Name,
                Description = p.Description,
                Unit = p.Unit,
                StockQuantity = p.StockQuantity,
                LowStockThreshold = p.LowStockThreshold,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                ImportHistory = p.WarehouseImports.Select(wi => new WarehouseImportResponse
                {
                    Id = wi.Id,
                    BatchCode = wi.BatchCode,
                    ProductId = wi.ProductId,
                    QuantityImported = wi.QuantityImported,
                    ImportPrice = wi.ImportPrice,
                    SupplierName = wi.SupplierName,
                    Note = wi.Note,
                    ExpirationDate = wi.ExpirationDate,
                    ImportedByUserId = wi.ImportedByUserId,
                    ImportedByUserName = wi.ImportedByUserName,
                    ImportedAt = wi.ImportedAt
                }).ToList()
            };
        }

        public async Task<ImportBatchSummaryResponse> ImportProductsAsync(ImportProductsRequest request, Guid adminId, string adminName)
        {
            if (request.Items == null || !request.Items.Any())
                throw new ArgumentException("Danh sách sản phẩm nhập trống.");

            var batchCode = $"IMP-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
            var imports = new List<WarehouseImport>();
            var productsToUpdate = new List<Product>();

            await _repository.BeginTransactionAsync();
            try
            {
                foreach (var item in request.Items)
                {
                    var product = await _repository.GetProductBySkuAsync(item.SKU);
                    if (product == null)
                    {
                        throw new ArgumentException($"Sản phẩm với mã SKU {item.SKU} không tồn tại trong hệ thống.");
                    }

                    var importRecord = new WarehouseImport
                    {
                        BatchCode = batchCode,
                        ProductId = product.Id,
                        QuantityImported = item.Quantity,
                        ImportPrice = item.ImportPrice,
                        SupplierName = request.SupplierName,
                        Note = request.Note,
                        ExpirationDate = item.ExpirationDate,
                        ImportedByUserId = adminId,
                        ImportedByUserName = adminName,
                        ImportedAt = DateTime.UtcNow
                    };
                    imports.Add(importRecord);

                    product.StockQuantity += item.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                    if (!productsToUpdate.Any(p => p.Id == product.Id))
                    {
                        productsToUpdate.Add(product);
                    }
                }

                await _repository.AddWarehouseImportsAsync(imports);
                await _repository.UpdateProductStocksAsync(productsToUpdate);
                await _repository.CommitTransactionAsync();

                return new ImportBatchSummaryResponse
                {
                    BatchCode = batchCode,
                    ImportedAt = imports.First().ImportedAt,
                    ImportedByUserName = adminName,
                    SupplierName = request.SupplierName,
                    TotalProducts = request.Items.Count,
                    TotalQuantity = imports.Sum(i => i.QuantityImported),
                    TotalImportValue = imports.Sum(i => i.QuantityImported * i.ImportPrice),
                    Items = imports.Select(wi => new WarehouseImportResponse
                    {
                        Id = wi.Id,
                        BatchCode = wi.BatchCode,
                        ProductId = wi.ProductId,
                        ProductSKU = productsToUpdate.First(p => p.Id == wi.ProductId).SKU,
                        ProductName = productsToUpdate.First(p => p.Id == wi.ProductId).Name,
                        QuantityImported = wi.QuantityImported,
                        ImportPrice = wi.ImportPrice,
                        SupplierName = wi.SupplierName,
                        Note = wi.Note,
                        ExpirationDate = wi.ExpirationDate,
                        ImportedByUserId = wi.ImportedByUserId,
                        ImportedByUserName = wi.ImportedByUserName,
                        ImportedAt = wi.ImportedAt
                    }).ToList()
                };
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
