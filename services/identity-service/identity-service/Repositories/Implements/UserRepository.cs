using Microsoft.EntityFrameworkCore;
using identity_service.Data;
using identity_service.DTOs.Requests;
using identity_service.Models;
using identity_service.Repositories.Interfaces;

namespace identity_service.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<User> Items, int TotalCount)> GetPagedAsync(UserSearchRequest request)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var kw = request.Keyword.Trim().ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(kw) || 
                                        (u.FullName != null && u.FullName.ToLower().Contains(kw)) ||
                                        (u.PhoneNumber != null && u.PhoneNumber.Contains(kw)));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var role = request.Role.Trim().ToLower();
                query = query.Where(u => u.Role.ToLower() == role);
            }

            var totalCount = await query.CountAsync();

            query = request.SortBy.ToLower() switch
            {
                "email" => request.SortDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "fullname" => request.SortDesc ? query.OrderByDescending(u => u.FullName) : query.OrderBy(u => u.FullName),
                "role" => request.SortDesc ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
                _ => request.SortDesc ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt)
            };

            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var page = Math.Max(request.Page, 1);
            
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
