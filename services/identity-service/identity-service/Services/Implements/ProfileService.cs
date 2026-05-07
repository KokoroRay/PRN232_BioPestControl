using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;
using identity_service.Repositories.Interfaces;
using identity_service.Services.Interfaces;

namespace identity_service.Services.Implements
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;

        public ProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResult<ProfileDto>> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult<ProfileDto>.NotFound("Người dùng không tồn tại.");
            }

            var profileDto = new ProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return ServiceResult<ProfileDto>.Ok(profileDto, "Lấy thông tin thành công.");
        }

        public async Task<ServiceResult<ProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult<ProfileDto>.NotFound("Người dùng không tồn tại.");
            }

            if (request.FullName != null) user.FullName = request.FullName.Trim();
            if (request.AvatarUrl != null) user.AvatarUrl = request.AvatarUrl.Trim();
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber.Trim();

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            var profileDto = new ProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return ServiceResult<ProfileDto>.Ok(profileDto, "Cập nhật thông tin thành công.");
        }

        public async Task<ServiceResult<object>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult<object>.NotFound("Người dùng không tồn tại.");
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return ServiceResult<object>.Fail("Tài khoản chưa có mật khẩu, không thể thực hiện đổi mật khẩu.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                return ServiceResult<object>.Fail("Mật khẩu cũ không chính xác.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return ServiceResult<object>.Ok(null!, "Đổi mật khẩu thành công.");
        }
    }
}
