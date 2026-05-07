using identity_service.DTOs;
using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;
using identity_service.Models;
using identity_service.Repositories.Interfaces;
using identity_service.Services.Interfaces;

namespace identity_service.Services.Implements
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IPermissionRepository _permissionRepo;

        public StaffService(IStaffRepository staffRepo, IPermissionRepository permissionRepo)
        {
            _staffRepo      = staffRepo;
            _permissionRepo = permissionRepo;
        }

        // ─────────────────────────────────────────────────────────────
        // GET PAGED
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<PagedResult<StaffDto>>> GetPagedAsync(StaffSearchRequest request)
        {
            var (items, totalCount) = await _staffRepo.GetPagedAsync(request);

            var dtoList = items.Select(MapToDto).ToList();

            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var page     = Math.Max(request.Page, 1);

            var result = new PagedResult<StaffDto>
            {
                Items      = dtoList,
                TotalCount = totalCount,
                Page       = page,
                PageSize   = pageSize
            };

            return ServiceResult<PagedResult<StaffDto>>.Ok(result,
                $"Lấy danh sách thành công ({totalCount} nhân viên).");
        }

        // ─────────────────────────────────────────────────────────────
        // GET BY ID
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<StaffDto>> GetByIdAsync(Guid staffId)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);

            if (staff == null)
                return ServiceResult<StaffDto>.NotFound("Không tìm thấy nhân viên.");

            return ServiceResult<StaffDto>.Ok(MapToDto(staff));
        }

        // ─────────────────────────────────────────────────────────────
        // CREATE
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<StaffDto>> CreateAsync(CreateStaffRequest request, Guid adminId)
        {
            if (!request.IsFullAccess && request.PermissionIds.Any())
            {
                var allExist = await _permissionRepo.AllExistAsync(request.PermissionIds);
                if (!allExist)
                    return ServiceResult<StaffDto>.Fail(
                        "Một hoặc nhiều Permission ID không hợp lệ hoặc không tồn tại.", 400);
            }

            var user = new User
            {
                Email        = request.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName     = request.FullName?.Trim(),
                PhoneNumber  = request.PhoneNumber?.Trim(),
                AvatarUrl    = request.AvatarUrl?.Trim(),
                Role         = "Staff",
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow
            };

            var staff = new Staff
            {
                User             = user,
                IsFullAccess     = request.IsFullAccess,
                CreatedByAdminId = adminId,
                CreatedAt        = DateTime.UtcNow
            };

            await _staffRepo.AddAsync(staff);
            await AssignPermissionsInternalAsync(staff, request.IsFullAccess, request.PermissionIds, adminId);

            try
            {
                await _staffRepo.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                when (ex.InnerException?.Message.Contains("IX_Users_Email") == true ||
                      ex.InnerException?.Message.Contains("unique") == true)
            {
                return ServiceResult<StaffDto>.Conflict("Email đã được sử dụng bởi tài khoản khác.");
            }

            var created = await _staffRepo.GetByIdAsync(staff.Id);
            return ServiceResult<StaffDto>.Created(MapToDto(created!), "Tạo nhân viên thành công.");
        }

        // ─────────────────────────────────────────────────────────────
        // UPDATE
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<StaffDto>> UpdateAsync(Guid staffId, UpdateStaffRequest request, Guid adminId)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);

            if (staff == null)
                return ServiceResult<StaffDto>.NotFound("Không tìm thấy nhân viên.");

            if (!request.IsFullAccess && request.PermissionIds.Any())
            {
                var allExist = await _permissionRepo.AllExistAsync(request.PermissionIds);
                if (!allExist)
                    return ServiceResult<StaffDto>.Fail(
                        "Một hoặc nhiều Permission ID không hợp lệ hoặc không tồn tại.", 400);
            }

            var user = staff.User;
            if (request.FullName    != null) user.FullName    = request.FullName.Trim();
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber.Trim();
            if (request.AvatarUrl   != null) user.AvatarUrl   = request.AvatarUrl.Trim();

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            user.UpdatedAt = DateTime.UtcNow;

            staff.IsFullAccess     = request.IsFullAccess;
            staff.UpdatedByAdminId = adminId;
            staff.UpdatedAt        = DateTime.UtcNow;

            await _staffRepo.ClearPermissionsAsync(staffId);
            await AssignPermissionsInternalAsync(staff, request.IsFullAccess, request.PermissionIds, adminId);

            await _staffRepo.UpdateAsync(staff);
            await _staffRepo.SaveChangesAsync();

            var updated = await _staffRepo.GetByIdAsync(staffId);
            return ServiceResult<StaffDto>.Ok(MapToDto(updated!), "Cập nhật nhân viên thành công.");
        }

        // ─────────────────────────────────────────────────────────────
        // DELETE
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<object>> DeleteAsync(Guid staffId)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);

            if (staff == null)
                return ServiceResult<object>.NotFound("Không tìm thấy nhân viên.");

            await _staffRepo.DeleteAsync(staff);
            await _staffRepo.SaveChangesAsync();

            return ServiceResult<object>.Ok(null!, "Xóa nhân viên thành công.");
        }

        // ─────────────────────────────────────────────────────────────
        // UPDATE STATUS
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<object>> UpdateStatusAsync(
            Guid staffId, UpdateStaffStatusRequest request, Guid adminId)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);

            if (staff == null)
                return ServiceResult<object>.NotFound("Không tìm thấy nhân viên.");

            staff.User.IsActive    = request.IsActive;
            staff.User.UpdatedAt   = DateTime.UtcNow;
            staff.UpdatedByAdminId = adminId;
            staff.UpdatedAt        = DateTime.UtcNow;

            await _staffRepo.UpdateAsync(staff);
            await _staffRepo.SaveChangesAsync();

            var statusText = request.IsActive ? "kích hoạt" : "khóa";
            return ServiceResult<object>.Ok(null!, $"Tài khoản nhân viên đã được {statusText} thành công.");
        }

        // ─────────────────────────────────────────────────────────────
        // UPDATE PERMISSIONS ONLY
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<StaffDto>> UpdatePermissionsAsync(
            Guid staffId, UpdateStaffPermissionsRequest request, Guid adminId)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);

            if (staff == null)
                return ServiceResult<StaffDto>.NotFound("Không tìm thấy nhân viên.");

            if (!request.IsFullAccess && request.PermissionIds.Any())
            {
                var allExist = await _permissionRepo.AllExistAsync(request.PermissionIds);
                if (!allExist)
                    return ServiceResult<StaffDto>.Fail(
                        "Một hoặc nhiều Permission ID không hợp lệ hoặc không tồn tại.", 400);
            }

            await _staffRepo.ClearPermissionsAsync(staffId);

            staff.IsFullAccess     = request.IsFullAccess;
            staff.UpdatedByAdminId = adminId;
            staff.UpdatedAt        = DateTime.UtcNow;

            await AssignPermissionsInternalAsync(staff, request.IsFullAccess, request.PermissionIds, adminId);
            await _staffRepo.UpdateAsync(staff);
            await _staffRepo.SaveChangesAsync();

            var updated = await _staffRepo.GetByIdAsync(staffId);
            return ServiceResult<StaffDto>.Ok(MapToDto(updated!), "Cập nhật quyền nhân viên thành công.");
        }

        // ─────────────────────────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────────────────────────

        private async Task AssignPermissionsInternalAsync(
            Staff staff, bool isFullAccess, List<int> permissionIds, Guid adminId)
        {
            List<int> idsToAssign;

            if (isFullAccess)
            {
                var allPermissions = await _permissionRepo.GetAllActiveAsync();
                idsToAssign = allPermissions.Select(p => p.Id).ToList();
            }
            else
            {
                idsToAssign = permissionIds.Distinct().ToList();
            }

            if (!idsToAssign.Any()) return;

            var staffPermissions = idsToAssign.Select(permId => new StaffPermission
            {
                StaffId          = staff.Id,
                PermissionId     = permId,
                GrantedAt        = DateTime.UtcNow,
                GrantedByAdminId = adminId
            }).ToList();

            await _staffRepo.AddPermissionsAsync(staffPermissions);
        }

        private static StaffDto MapToDto(Staff s) => new()
        {
            Id               = s.Id,
            UserId           = s.UserId,
            Email            = s.User.Email,
            FullName         = s.User.FullName,
            AvatarUrl        = s.User.AvatarUrl,
            PhoneNumber      = s.User.PhoneNumber,
            IsActive         = s.User.IsActive,
            IsFullAccess     = s.IsFullAccess,
            PermissionCount  = s.StaffPermissions.Count,
            CreatedByAdminId = s.CreatedByAdminId,
            UpdatedByAdminId = s.UpdatedByAdminId,
            CreatedAt        = s.CreatedAt,
            UpdatedAt        = s.UpdatedAt,
            Permissions      = s.StaffPermissions
                .Where(sp => sp.Permission != null)
                .OrderBy(sp => sp.Permission.GroupCode)
                .ThenBy(sp => sp.Permission.DisplayOrder)
                .Select(sp => new PermissionDto
                {
                    Id           = sp.Permission.Id,
                    Code         = sp.Permission.Code,
                    DisplayName  = sp.Permission.DisplayName,
                    Description  = sp.Permission.Description,
                    GroupCode    = sp.Permission.GroupCode,
                    GroupName    = sp.Permission.GroupName,
                    DisplayOrder = sp.Permission.DisplayOrder,
                    IsActive     = sp.Permission.IsActive
                }).ToList()
        };
    }
}
