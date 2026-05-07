using identity_service.DTOs;

namespace identity_service.Services
{
    public interface IPermissionService
    {
        Task<ServiceResult<List<PermissionDto>>> GetAllActiveAsync();
        Task<ServiceResult<List<PermissionGroupDto>>> GetGroupedAsync();
    }
}
