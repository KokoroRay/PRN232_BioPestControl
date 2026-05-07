using identity_service.DTOs;
using identity_service.Repositories.Interfaces;
using identity_service.Services.Interfaces;

namespace identity_service.Services.Implements
{
    /// <summary>
    /// Triển khai IPermissionService.
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepo;

        public PermissionService(IPermissionRepository permissionRepo)
        {
            _permissionRepo = permissionRepo;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<List<PermissionDto>>> GetAllActiveAsync()
        {
            var permissions = await _permissionRepo.GetAllActiveAsync();

            var dtos = permissions.Select(p => new PermissionDto
            {
                Id          = p.Id,
                Code        = p.Code,
                DisplayName = p.DisplayName,
                Description = p.Description,
                GroupCode   = p.GroupCode,
                GroupName   = p.GroupName,
                DisplayOrder = p.DisplayOrder,
                IsActive    = p.IsActive
            }).ToList();

            return ServiceResult<List<PermissionDto>>.Ok(dtos,
                $"Lấy danh sách quyền thành công ({dtos.Count} quyền).");
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<List<PermissionGroupDto>>> GetGroupedAsync()
        {
            var permissions = await _permissionRepo.GetAllActiveAsync();

            // Nhóm theo GroupCode rồi sắp xếp
            var groups = permissions
                .GroupBy(p => new { p.GroupCode, p.GroupName })
                .OrderBy(g => g.Key.GroupCode)
                .Select(g => new PermissionGroupDto
                {
                    GroupCode   = g.Key.GroupCode,
                    GroupName   = g.Key.GroupName,
                    Permissions = g.OrderBy(p => p.DisplayOrder).Select(p => new PermissionDto
                    {
                        Id          = p.Id,
                        Code        = p.Code,
                        DisplayName = p.DisplayName,
                        Description = p.Description,
                        GroupCode   = p.GroupCode,
                        GroupName   = p.GroupName,
                        DisplayOrder = p.DisplayOrder,
                        IsActive    = p.IsActive
                    }).ToList()
                }).ToList();

            return ServiceResult<List<PermissionGroupDto>>.Ok(groups,
                $"Lấy danh sách nhóm quyền thành công ({groups.Count} nhóm).");
        }
    }
}
