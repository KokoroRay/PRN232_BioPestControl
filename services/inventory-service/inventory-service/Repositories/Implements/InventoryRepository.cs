using inventory_service.Data;
using inventory_service.Models;
using inventory_service.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace inventory_service.Repositories.Implements
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _context;
        private IDbContextTransaction? _transaction;

        public InventoryRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<inventory_service.DTOs.Responses.PagedResult<Product>> GetAllProductsAsync(string? searchQuery, string? sortBy, bool ascending = true, int page = 1, int pageSize = 10)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var search = searchQuery.ToLower();
                query = query.Where(p => p.SKU.ToLower().Contains(search) || p.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                    "stock" => ascending ? query.OrderBy(p => p.StockQuantity) : query.OrderByDescending(p => p.StockQuantity),
                    "createdat" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.Id)
                };
            }
            else
            {
                query = query.OrderBy(p => p.Id); // Default sort
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new inventory_service.DTOs.Responses.PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Product?> GetProductBySkuAsync(string sku)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.SKU == sku);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetProductWithImportHistoryAsync(int id)
        {
            return await _context.Products
                .Include(p => p.WarehouseImports.OrderByDescending(wi => wi.ImportedAt))
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddWarehouseImportsAsync(IEnumerable<WarehouseImport> imports)
        {
            await _context.WarehouseImports.AddRangeAsync(imports);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductStocksAsync(IEnumerable<Product> products)
        {
            _context.Products.UpdateRange(products);
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
