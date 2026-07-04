using catalog_service.Data;
using catalog_service.Models;
using catalog_service.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using catalog_service.DTOs.Requests;

namespace catalog_service.Repositories.Implements
{
    public class ProductRepository : IProductRepository
    {
        private readonly CatalogDbContext _context;

        public ProductRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(ProductFilterRequest filter = null)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    query = query.Where(p => p.Name.Contains(filter.Name));
                }
                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
                }
                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(p => p.UnitPrice >= filter.MinPrice.Value);
                }
                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.UnitPrice <= filter.MaxPrice.Value);
                }

                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    switch (filter.SortBy.ToLower())
                    {
                        case "price":
                            query = filter.Ascending ? query.OrderBy(p => p.UnitPrice) : query.OrderByDescending(p => p.UnitPrice);
                            break;
                        case "name":
                            query = filter.Ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                            break;
                        default:
                            query = filter.Ascending ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id);
                            break;
                    }
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsByCategoryIdAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId);
        }

        public async Task<bool> ExistsBySkuAsync(string sku)
        {
            return await _context.Products.AnyAsync(p => p.SKU == sku);
        }

        public async Task<bool> ExistsBySkuExceptIdAsync(string sku, int id)
        {
            return await _context.Products.AnyAsync(p => p.SKU == sku && p.Id != id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
