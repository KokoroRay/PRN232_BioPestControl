using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;

namespace identity_service.Services.Interfaces
{
    public interface IPasswordResetService
    {
        Task<ServiceResult<object>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ServiceResult<object>> VerifyOtpAsync(VerifyOtpRequest request);
        Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
