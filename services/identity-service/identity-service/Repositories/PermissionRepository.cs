using Microsoft.EntityFrameworkCore;
using identity_service.Data;
using identity_service.Models;

namespace identity_service.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Permission>> GetAllActiveAsync()
        {
            return await _context.Permissions
                .Where(p => p.IsActive)
                .OrderBy(p => p.GroupCode)
                .ThenBy(p => p.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<Permission>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();
            return await _context.Permissions
                .Where(p => idList.Contains(p.Id) && p.IsActive)
                .ToListAsync();
        }

        public async Task<Permission?> GetByIdAsync(int id)
        {
            return await _context.Permissions.FindAsync(id);
        }
        public async Task<bool> AllExistAsync(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();
            if (!idList.Any()) return true;

            var count = await _context.Permissions
                .CountAsync(p => idList.Contains(p.Id) && p.IsActive);

            return count == idList.Count;
        }
    }
}
