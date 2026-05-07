using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;
using identity_service.Repositories.Interfaces;
using identity_service.Services.Interfaces;

namespace identity_service.Services.Implements
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public PasswordResetService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<ServiceResult<object>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                // To prevent email enumeration, we still return a success message
                return ServiceResult<object>.Ok(null!, "Nếu email tồn tại trong hệ thống, mã OTP đã được gửi.");
            }

            if (!user.IsActive)
            {
                return ServiceResult<object>.Fail("Tài khoản của bạn đã bị khóa.");
            }

            // Generate a 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();
            user.ResetPasswordOtp = otp;
            user.ResetPasswordExpiry = DateTime.UtcNow.AddMinutes(15);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Send Email
            var body = $"Mã OTP để đặt lại mật khẩu của bạn là: {otp}. Mã này sẽ hết hạn sau 15 phút.";
            await _emailService.SendEmailAsync(user.Email, "Đặt lại mật khẩu - BioPestControl", body);

            return ServiceResult<object>.Ok(null!, "Nếu email tồn tại trong hệ thống, mã OTP đã được gửi.");
        }

        public async Task<ServiceResult<object>> VerifyOtpAsync(VerifyOtpRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || user.ResetPasswordOtp != request.Otp)
            {
                return ServiceResult<object>.Fail("Mã OTP không hợp lệ.");
            }

            if (user.ResetPasswordExpiry == null || user.ResetPasswordExpiry < DateTime.UtcNow)
            {
                return ServiceResult<object>.Fail("Mã OTP đã hết hạn.");
            }

            return ServiceResult<object>.Ok(null!, "Mã OTP hợp lệ.");
        }

        public async Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || user.ResetPasswordOtp != request.Otp)
            {
                return ServiceResult<object>.Fail("Mã OTP không hợp lệ.");
            }

            if (user.ResetPasswordExpiry == null || user.ResetPasswordExpiry < DateTime.UtcNow)
            {
                return ServiceResult<object>.Fail("Mã OTP đã hết hạn.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.ResetPasswordOtp = null;
            user.ResetPasswordExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return ServiceResult<object>.Ok(null!, "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập bằng mật khẩu mới.");
        }
    }
}
