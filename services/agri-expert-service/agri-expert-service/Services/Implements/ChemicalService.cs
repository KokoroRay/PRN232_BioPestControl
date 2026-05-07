using agri_expert_service.DTOs.Requests;
using agri_expert_service.DTOs.Responses;
using agri_expert_service.Models;
using agri_expert_service.Repositories.Interfaces;
using agri_expert_service.Services.Interfaces;

namespace agri_expert_service.Services.Implements
{
    public class ChemicalService : IChemicalService
    {
        private readonly IChemicalRepository _repo;

        public ChemicalService(IChemicalRepository repo)
        {
            _repo = repo;
        }

        // ─────────────────────────────────────────────────────────────
        // GET PAGED (View + Search + Filter)
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<PagedResult<ChemicalDto>>> GetPagedAsync(ChemicalSearchRequest request)
        {
            var (items, totalCount) = await _repo.GetPagedAsync(request);

            var dtoList = items.Select(MapToDto).ToList();

            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var page     = Math.Max(request.Page, 1);

            var result = new PagedResult<ChemicalDto>
            {
                Items      = dtoList,
                TotalCount = totalCount,
                Page       = page,
                PageSize   = pageSize
            };

            return ServiceResult<PagedResult<ChemicalDto>>.Ok(result,
                $"Lấy danh sách thành công ({totalCount} hóa chất).");
        }

        // ─────────────────────────────────────────────────────────────
        // GET BY ID
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<ChemicalDto>> GetByIdAsync(int id)
        {
            var chemical = await _repo.GetByIdAsync(id);

            if (chemical == null)
                return ServiceResult<ChemicalDto>.NotFound("Không tìm thấy hóa chất.");

            return ServiceResult<ChemicalDto>.Ok(MapToDto(chemical));
        }

        // ─────────────────────────────────────────────────────────────
        // GET DISTINCT GROUPS (cho dropdown filter)
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<List<string>>> GetGroupsAsync()
        {
            var groups = await _repo.GetDistinctGroupsAsync();
            return ServiceResult<List<string>>.Ok(groups,
                $"Lấy danh sách nhóm thành công ({groups.Count} nhóm).");
        }

        // ─────────────────────────────────────────────────────────────
        // CREATE (Admin only)
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<ChemicalDto>> CreateAsync(CreateChemicalRequest request)
        {
            if (await _repo.ExistsByNameAsync(request.Name))
                return ServiceResult<ChemicalDto>.Conflict($"Hóa chất tên '{request.Name}' đã tồn tại.");

            if (!string.IsNullOrWhiteSpace(request.CasNumber) &&
                await _repo.ExistsByCasAsync(request.CasNumber))
                return ServiceResult<ChemicalDto>.Conflict($"Số CAS '{request.CasNumber}' đã được sử dụng bởi hóa chất khác.");

            var chemical = new ChemicalProfile
            {
                Name            = request.Name.Trim(),
                VietnameseName  = request.VietnameseName?.Trim(),
                CasNumber       = string.IsNullOrWhiteSpace(request.CasNumber) ? null : request.CasNumber.Trim(),
                ChemicalGroup   = request.ChemicalGroup?.Trim(),
                ChemicalFormula = request.ChemicalFormula?.Trim(),
                Description     = request.Description?.Trim(),
                ToxicityLevel   = request.ToxicityLevel?.Trim(),
                UsageMethod     = request.UsageMethod?.Trim(),
                SafetyNotes     = request.SafetyNotes?.Trim(),
                TargetCrops     = request.TargetCrops?.Trim(),
                TargetPests     = request.TargetPests?.Trim(),
                IsActive        = request.IsActive,
                CreatedAt       = DateTime.UtcNow
            };

            await _repo.AddAsync(chemical);
            await _repo.SaveChangesAsync();

            var created = await _repo.GetByIdAsync(chemical.Id);
            return ServiceResult<ChemicalDto>.Created(MapToDto(created!), "Tạo hóa chất thành công.");
        }

        // ─────────────────────────────────────────────────────────────
        // UPDATE (Admin only)
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<ChemicalDto>> UpdateAsync(int id, UpdateChemicalRequest request)
        {
            var chemical = await _repo.GetByIdAsync(id);

            if (chemical == null)
                return ServiceResult<ChemicalDto>.NotFound("Không tìm thấy hóa chất.");

            if (await _repo.ExistsByNameAsync(request.Name, id))
                return ServiceResult<ChemicalDto>.Conflict($"Hóa chất tên '{request.Name}' đã tồn tại.");

            if (!string.IsNullOrWhiteSpace(request.CasNumber) &&
                await _repo.ExistsByCasAsync(request.CasNumber, id))
                return ServiceResult<ChemicalDto>.Conflict($"Số CAS '{request.CasNumber}' đã được sử dụng bởi hóa chất khác.");

            chemical.Name            = request.Name.Trim();
            chemical.VietnameseName  = request.VietnameseName?.Trim();
            chemical.CasNumber       = string.IsNullOrWhiteSpace(request.CasNumber) ? null : request.CasNumber.Trim();
            chemical.ChemicalGroup   = request.ChemicalGroup?.Trim();
            chemical.ChemicalFormula = request.ChemicalFormula?.Trim();
            chemical.Description     = request.Description?.Trim();
            chemical.ToxicityLevel   = request.ToxicityLevel?.Trim();
            chemical.UsageMethod     = request.UsageMethod?.Trim();
            chemical.SafetyNotes     = request.SafetyNotes?.Trim();
            chemical.TargetCrops     = request.TargetCrops?.Trim();
            chemical.TargetPests     = request.TargetPests?.Trim();
            chemical.IsActive        = request.IsActive;
            chemical.UpdatedAt       = DateTime.UtcNow;

            await _repo.UpdateAsync(chemical);
            await _repo.SaveChangesAsync();

            return ServiceResult<ChemicalDto>.Ok(MapToDto(chemical), "Cập nhật hóa chất thành công.");
        }

        // ─────────────────────────────────────────────────────────────
        // DELETE (Admin only)
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<object>> DeleteAsync(int id)
        {
            var chemical = await _repo.GetByIdAsync(id);

            if (chemical == null)
                return ServiceResult<object>.NotFound("Không tìm thấy hóa chất.");

            await _repo.DeleteAsync(chemical);
            await _repo.SaveChangesAsync();

            return ServiceResult<object>.Ok(null!, "Xóa hóa chất thành công.");
        }

        // ─────────────────────────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────────────────────────

        private static ChemicalDto MapToDto(ChemicalProfile c) => new()
        {
            Id              = c.Id,
            Name            = c.Name,
            VietnameseName  = c.VietnameseName,
            CasNumber       = c.CasNumber,
            ChemicalGroup   = c.ChemicalGroup,
            ChemicalFormula = c.ChemicalFormula,
            Description     = c.Description,
            ToxicityLevel   = c.ToxicityLevel,
            UsageMethod     = c.UsageMethod,
            SafetyNotes     = c.SafetyNotes,
            TargetCrops     = c.TargetCrops,
            TargetPests     = c.TargetPests,
            IsActive        = c.IsActive,
            CreatedAt       = c.CreatedAt,
            UpdatedAt       = c.UpdatedAt
        };
    }
}
