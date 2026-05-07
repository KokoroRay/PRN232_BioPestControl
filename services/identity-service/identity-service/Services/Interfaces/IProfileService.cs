using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;

namespace identity_service.Services.Interfaces
{
    public interface IProfileService
    {
        Task<ServiceResult<ProfileDto>> GetProfileAsync(Guid userId);
        Task<ServiceResult<ProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
        Task<ServiceResult<object>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    }
}
