using identity_service.DTOs;
using identity_service.Models;
using identity_service.Repositories;

namespace identity_service.Services
{
    /// <summary>
    /// Triển khai IStaffService — xử lý toàn bộ nghiệp vụ Staff Management + IAM.
    /// </summary>
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
        public async Task<ServiceResult<PagedResult<StaffSummaryDto>>> GetPagedAsync(StaffSearchRequest request)
        {
            var (items, totalCount) = await _staffRepo.GetPagedAsync(request);

            var dtoList = items.Select(s => MapToSummaryDto(s)).ToList();

            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var page     = Math.Max(request.Page, 1);

            var result = new PagedResult<StaffSummaryDto>
            {
                Items      = dtoList,
                TotalCount = totalCount,
                Page       = page,
                PageSize   = pageSize
            };

            return ServiceResult<PagedResult<StaffSummaryDto>>.Ok(result,
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
            // 1. Kiểm tra email đã tồn tại chưa (check ở DB thông qua repo)
            //    Thực tế check này nằm ở User table (unique index trên Email)
            //    Chúng ta sẽ để DB throw + service catch nếu duplicate
            //    Nhưng tốt hơn là kiểm tra trước.

            // 2. Validate PermissionIds nếu không phải full access
            if (!request.IsFullAccess && request.PermissionIds.Any())
            {
                var allExist = await _permissionRepo.AllExistAsync(request.PermissionIds);
                if (!allExist)
                    return ServiceResult<StaffDto>.Fail(
                        "Một hoặc nhiều Permission ID không hợp lệ hoặc không tồn tại.", 400);
            }

            // 3. Tạo User account mới với Role = "Staff"
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

            // 4. Tạo Staff entity liên kết với User
            var staff = new Staff
            {
                User             = user,
                IsFullAccess     = request.IsFullAccess,
                CreatedByAdminId = adminId,
                CreatedAt        = DateTime.UtcNow
            };

            await _staffRepo.AddAsync(staff);

            // 5. Gán quyền IAM
            await AssignPermissionsInternalAsync(staff, request.IsFullAccess, request.PermissionIds, adminId);

            // 6. Lưu vào DB (transaction ngầm của EF Core)
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

            // 7. Load lại để trả về đầy đủ thông tin
            var created = await _staffRepo.GetByIdAsync(staff.Id);
            return ServiceResult<StaffDto>.Created(MapToDto(created!),
                "Tạo nhân viên thành công.");
        }

        // ─────────────────────────────────────────────────────────────
        // UPDATE
        // ─────────────────────────────────────────────────────────────
        public async Task<ServiceResult<StaffDto>> UpdateAsync(Guid staffId, UpdateStaffRequest request, Guid adminId)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);

            if (staff == null)
                return ServiceResult<StaffDto>.NotFound("Không tìm thấy nhân viên.");

            // Validate PermissionIds
            if (!request.IsFullAccess && request.PermissionIds.Any())
            {
                var allExist = await _permissionRepo.AllExistAsync(request.PermissionIds);
                if (!allExist)
                    return ServiceResult<StaffDto>.Fail(
                        "Một hoặc nhiều Permission ID không hợp lệ hoặc không tồn tại.", 400);
            }

            // ── Cập nhật thông tin User ──
            var user = staff.User;
            if (request.FullName    != null) user.FullName    = request.FullName.Trim();
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber.Trim();
            if (request.AvatarUrl   != null) user.AvatarUrl   = request.AvatarUrl.Trim();

            // Đặt lại mật khẩu nếu Admin cung cấp
            if (!string.IsNullOrWhiteSpace(request.NewPassword))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            user.UpdatedAt = DateTime.UtcNow;

            // ── Cập nhật Staff ──
            staff.IsFullAccess     = request.IsFullAccess;
            staff.UpdatedByAdminId = adminId;
            staff.UpdatedAt        = DateTime.UtcNow;

            // ── Cập nhật quyền IAM (xóa cũ + gán mới) ──
            await _staffRepo.ClearPermissionsAsync(staffId);
            await AssignPermissionsInternalAsync(staff, request.IsFullAccess, request.PermissionIds, adminId);

            await _staffRepo.UpdateAsync(staff);
            await _staffRepo.SaveChangesAsync();

            // Load lại
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

            // Cascade: StaffPermissions sẽ bị xóa theo (cấu hình ở DbContext)
            // User account cũng bị xóa theo (Cascade)
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

            staff.User.IsActive     = request.IsActive;
            staff.User.UpdatedAt    = DateTime.UtcNow;
            staff.UpdatedByAdminId  = adminId;
            staff.UpdatedAt         = DateTime.UtcNow;

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

            // Validate PermissionIds
            if (!request.IsFullAccess && request.PermissionIds.Any())
            {
                var allExist = await _permissionRepo.AllExistAsync(request.PermissionIds);
                if (!allExist)
                    return ServiceResult<StaffDto>.Fail(
                        "Một hoặc nhiều Permission ID không hợp lệ hoặc không tồn tại.", 400);
            }

            // Xóa quyền cũ
            await _staffRepo.ClearPermissionsAsync(staffId);

            // Gán quyền mới
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

        /// <summary>
        /// Gán quyền cho Staff.
        /// Nếu IsFullAccess = true → lấy toàn bộ permissions từ DB rồi gán.
        /// Nếu IsFullAccess = false → chỉ gán các permissionIds được chỉ định.
        /// </summary>
        private async Task AssignPermissionsInternalAsync(
            Staff staff, bool isFullAccess, List<int> permissionIds, Guid adminId)
        {
            List<int> idsToAssign;

            if (isFullAccess)
            {
                // Lấy toàn bộ permissions đang active
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

        /// <summary>Map Staff entity → StaffDto (đầy đủ thông tin)</summary>
        private static StaffDto MapToDto(Staff s)
        {
            return new StaffDto
            {
                Id               = s.Id,
                UserId           = s.UserId,
                Email            = s.User.Email,
                FullName         = s.User.FullName,
                AvatarUrl        = s.User.AvatarUrl,
                PhoneNumber      = s.User.PhoneNumber,
                IsActive         = s.User.IsActive,
                IsFullAccess     = s.IsFullAccess,
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

        /// <summary>Map Staff entity → StaffSummaryDto (tóm tắt cho danh sách)</summary>
        private static StaffSummaryDto MapToSummaryDto(Staff s)
        {
            return new StaffSummaryDto
            {
                Id              = s.Id,
                UserId          = s.UserId,
                Email           = s.User.Email,
                FullName        = s.User.FullName,
                AvatarUrl       = s.User.AvatarUrl,
                PhoneNumber     = s.User.PhoneNumber,
                IsActive        = s.User.IsActive,
                IsFullAccess    = s.IsFullAccess,
                PermissionCount = s.StaffPermissions.Count,
                CreatedAt       = s.CreatedAt
            };
        }
    }
}
