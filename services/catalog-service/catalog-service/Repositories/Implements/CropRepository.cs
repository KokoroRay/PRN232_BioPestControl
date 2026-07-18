using catalog_service.Data;
using catalog_service.Models;
using catalog_service.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace catalog_service.Repositories.Implements
{
    public class CropRepository : ICropRepository
    {
        private readonly CatalogDbContext _context;

        public CropRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Crop>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Crops.AsQueryable();
            if (!includeInactive)
            {
                query = query.Where(c => c.IsActive);
            }
            return await query.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Crop?> GetByIdAsync(int id)
        {
            return await _context.Crops
                .Include(c => c.ProductCrops)
                    .ThenInclude(pc => pc.Product)
                    .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Crop?> GetBySlugAsync(string slug)
        {
            return await _context.Crops
                .Include(c => c.ProductCrops)
                    .ThenInclude(pc => pc.Product)
                    .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.Slug == slug);
        }

        public async Task<Crop> CreateAsync(Crop crop)
        {
            _context.Crops.Add(crop);
            await _context.SaveChangesAsync();
            return crop;
        }

        public async Task<Crop> UpdateAsync(Crop crop)
        {
            _context.Crops.Update(crop);
            await _context.SaveChangesAsync();
            return crop;
        }

        public async Task DeleteAsync(int id)
        {
            var crop = await _context.Crops.FindAsync(id);
            if (crop != null)
            {
                _context.Crops.Remove(crop);
                await _context.SaveChangesAsync();
            }
        }
    }
}
