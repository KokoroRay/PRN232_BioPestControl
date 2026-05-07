using Microsoft.EntityFrameworkCore;
using identity_service.Data;
using identity_service.DTOs;
using identity_service.Models;
using identity_service.Repositories.Interfaces;

namespace identity_service.Repositories.Implements
{
    /// <summary>
    /// Triển khai IStaffRepository sử dụng Entity Framework Core.
    /// </summary>
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<(List<Staff> Items, int TotalCount)> GetPagedAsync(StaffSearchRequest request)
        {
            // Base query: load kèm User và Permissions
            var query = _context.Staffs
                .Include(s => s.User)
                .Include(s => s.StaffPermissions)
                    .ThenInclude(sp => sp.Permission)
                .AsQueryable();

            // ── Lọc theo từ khóa tìm kiếm (email, tên) ──
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var kw = request.Keyword.Trim().ToLower();
                query = query.Where(s =>
                    s.User.Email.ToLower().Contains(kw) ||
                    (s.User.FullName != null && s.User.FullName.ToLower().Contains(kw)));
            }

            // ── Lọc theo trạng thái tài khoản ──
            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.User.IsActive == request.IsActive.Value);
            }

            // ── Lọc theo IsFullAccess ──
            if (request.IsFullAccess.HasValue)
            {
                query = query.Where(s => s.IsFullAccess == request.IsFullAccess.Value);
            }

            // ── Đếm tổng số bản ghi trước khi phân trang ──
            var totalCount = await query.CountAsync();

            // ── Sắp xếp ──
            query = request.SortBy.ToLower() switch
            {
                "email"    => request.SortDesc ? query.OrderByDescending(s => s.User.Email)    : query.OrderBy(s => s.User.Email),
                "fullname" => request.SortDesc ? query.OrderByDescending(s => s.User.FullName) : query.OrderBy(s => s.User.FullName),
                _          => request.SortDesc ? query.OrderByDescending(s => s.CreatedAt)     : query.OrderBy(s => s.CreatedAt)
            };

            // ── Phân trang ──
            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var page     = Math.Max(request.Page, 1);
            var items    = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        /// <inheritdoc/>
        public async Task<Staff?> GetByIdAsync(Guid staffId)
        {
            return await _context.Staffs
                .Include(s => s.User)
                .Include(s => s.StaffPermissions)
                    .ThenInclude(sp => sp.Permission)
                .FirstOrDefaultAsync(s => s.Id == staffId);
        }

        /// <inheritdoc/>
        public async Task<Staff?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Staffs
                .Include(s => s.User)
                .Include(s => s.StaffPermissions)
                    .ThenInclude(sp => sp.Permission)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _context.Staffs.AnyAsync(s => s.UserId == userId);
        }

        /// <inheritdoc/>
        public async Task AddAsync(Staff staff)
        {
            await _context.Staffs.AddAsync(staff);
        }

        /// <inheritdoc/>
        public Task UpdateAsync(Staff staff)
        {
            _context.Staffs.Update(staff);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task DeleteAsync(Staff staff)
        {
            _context.Staffs.Remove(staff);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task ClearPermissionsAsync(Guid staffId)
        {
            var existingPermissions = await _context.StaffPermissions
                .Where(sp => sp.StaffId == staffId)
                .ToListAsync();

            if (existingPermissions.Any())
            {
                _context.StaffPermissions.RemoveRange(existingPermissions);
            }
        }

        /// <inheritdoc/>
        public async Task AddPermissionsAsync(IEnumerable<StaffPermission> permissions)
        {
            await _context.StaffPermissions.AddRangeAsync(permissions);
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
