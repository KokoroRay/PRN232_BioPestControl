using Microsoft.EntityFrameworkCore;
using agri_expert_service.Data;
using agri_expert_service.DTOs.Requests;
using agri_expert_service.Models;
using agri_expert_service.Repositories.Interfaces;

namespace agri_expert_service.Repositories.Implements
{
    public class ChemicalRepository : IChemicalRepository
    {
        private readonly AgriDbContext _context;

        public ChemicalRepository(AgriDbContext context)
        {
            _context = context;
        }

        public async Task<(List<ChemicalProfile> Items, int TotalCount)> GetPagedAsync(ChemicalSearchRequest request)
        {
            var query = _context.ChemicalProfiles.AsQueryable();

            // ── Lọc theo từ khóa (tên, CAS, nhóm, mô tả) ──
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var kw = request.Keyword.Trim().ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(kw) ||
                    (c.VietnameseName != null && c.VietnameseName.ToLower().Contains(kw)) ||
                    (c.CasNumber      != null && c.CasNumber.ToLower().Contains(kw)) ||
                    (c.ChemicalGroup  != null && c.ChemicalGroup.ToLower().Contains(kw)) ||
                    (c.Description    != null && c.Description.ToLower().Contains(kw)));
            }

            // ── Lọc theo nhóm hóa chất ──
            if (!string.IsNullOrWhiteSpace(request.ChemicalGroup))
            {
                var grp = request.ChemicalGroup.Trim().ToLower();
                query = query.Where(c => c.ChemicalGroup != null &&
                                         c.ChemicalGroup.ToLower() == grp);
            }

            // ── Lọc theo mức độ độc hại ──
            if (!string.IsNullOrWhiteSpace(request.ToxicityLevel))
            {
                var tox = request.ToxicityLevel.Trim().ToLower();
                query = query.Where(c => c.ToxicityLevel != null &&
                                         c.ToxicityLevel.ToLower() == tox);
            }

            // ── Lọc theo trạng thái ──
            if (request.IsActive.HasValue)
                query = query.Where(c => c.IsActive == request.IsActive.Value);

            // ── Đếm tổng ──
            var totalCount = await query.CountAsync();

            // ── Sắp xếp ──
            query = request.SortBy.ToLower() switch
            {
                "group"     => request.SortDesc ? query.OrderByDescending(c => c.ChemicalGroup) : query.OrderBy(c => c.ChemicalGroup),
                "toxicity"  => request.SortDesc ? query.OrderByDescending(c => c.ToxicityLevel) : query.OrderBy(c => c.ToxicityLevel),
                "createdat" => request.SortDesc ? query.OrderByDescending(c => c.CreatedAt)     : query.OrderBy(c => c.CreatedAt),
                _           => request.SortDesc ? query.OrderByDescending(c => c.Name)          : query.OrderBy(c => c.Name)
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

        public async Task<ChemicalProfile?> GetByIdAsync(int id)
            => await _context.ChemicalProfiles.FindAsync(id);

        public async Task<ChemicalProfile?> GetByCasNumberAsync(string casNumber)
            => await _context.ChemicalProfiles
                .FirstOrDefaultAsync(c => c.CasNumber == casNumber);

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.ChemicalProfiles
                .Where(c => c.Name.ToLower() == name.Trim().ToLower());
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByCasAsync(string casNumber, int? excludeId = null)
        {
            var query = _context.ChemicalProfiles
                .Where(c => c.CasNumber != null && c.CasNumber == casNumber.Trim());
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<List<string>> GetDistinctGroupsAsync()
            => await _context.ChemicalProfiles
                .Where(c => c.ChemicalGroup != null && c.IsActive)
                .Select(c => c.ChemicalGroup!)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

        public async Task AddAsync(ChemicalProfile chemical)
            => await _context.ChemicalProfiles.AddAsync(chemical);

        public Task UpdateAsync(ChemicalProfile chemical)
        {
            _context.ChemicalProfiles.Update(chemical);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ChemicalProfile chemical)
        {
            _context.ChemicalProfiles.Remove(chemical);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
