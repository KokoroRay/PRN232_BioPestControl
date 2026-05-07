using Microsoft.EntityFrameworkCore;
using identity_service.Data;
using identity_service.DTOs.Requests;
using identity_service.Models;
using identity_service.Repositories.Interfaces;

namespace identity_service.Repositories.Implements
{
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Staff> Items, int TotalCount)> GetPagedAsync(StaffSearchRequest request)
        {
            var query = _context.Staffs
                .Include(s => s.User)
                .Include(s => s.StaffPermissions)
                    .ThenInclude(sp => sp.Permission)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var kw = request.Keyword.Trim().ToLower();
                query = query.Where(s =>
                    s.User.Email.ToLower().Contains(kw) ||
                    (s.User.FullName != null && s.User.FullName.ToLower().Contains(kw)));
            }

            if (request.IsActive.HasValue)
                query = query.Where(s => s.User.IsActive == request.IsActive.Value);

            if (request.IsFullAccess.HasValue)
                query = query.Where(s => s.IsFullAccess == request.IsFullAccess.Value);

            var totalCount = await query.CountAsync();

            query = request.SortBy.ToLower() switch
            {
                "email"    => request.SortDesc ? query.OrderByDescending(s => s.User.Email)    : query.OrderBy(s => s.User.Email),
                "fullname" => request.SortDesc ? query.OrderByDescending(s => s.User.FullName) : query.OrderBy(s => s.User.FullName),
                _          => request.SortDesc ? query.OrderByDescending(s => s.CreatedAt)     : query.OrderBy(s => s.CreatedAt)
            };

            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var page     = Math.Max(request.Page, 1);
            var items    = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Staff?> GetByIdAsync(Guid staffId)
            => await _context.Staffs
                .Include(s => s.User)
                .Include(s => s.StaffPermissions)
                    .ThenInclude(sp => sp.Permission)
                .FirstOrDefaultAsync(s => s.Id == staffId);

        public async Task<Staff?> GetByUserIdAsync(Guid userId)
            => await _context.Staffs
                .Include(s => s.User)
                .Include(s => s.StaffPermissions)
                    .ThenInclude(sp => sp.Permission)
                .FirstOrDefaultAsync(s => s.UserId == userId);

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
            => await _context.Staffs.AnyAsync(s => s.UserId == userId);

        public async Task AddAsync(Staff staff)
            => await _context.Staffs.AddAsync(staff);

        public Task UpdateAsync(Staff staff)
        {
            _context.Staffs.Update(staff);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Staff staff)
        {
            _context.Staffs.Remove(staff);
            return Task.CompletedTask;
        }

        public async Task ClearPermissionsAsync(Guid staffId)
        {
            var existing = await _context.StaffPermissions
                .Where(sp => sp.StaffId == staffId)
                .ToListAsync();

            if (existing.Any())
                _context.StaffPermissions.RemoveRange(existing);
        }

        public async Task AddPermissionsAsync(IEnumerable<StaffPermission> permissions)
            => await _context.StaffPermissions.AddRangeAsync(permissions);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
