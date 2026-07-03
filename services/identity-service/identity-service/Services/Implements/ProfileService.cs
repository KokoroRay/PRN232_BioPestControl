using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;
using identity_service.Repositories.Interfaces;
using identity_service.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace identity_service.Services.Implements
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public ProfileService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
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

        public async Task<ServiceResult<object>> UploadAvatarAsync(Guid userId, IFormFile file)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult<object>.NotFound("Người dùng không tồn tại.");
            }

            // Get upload directory from config or use default
            var uploadDir = _configuration["UploadSettings:AvatarDirectory"] ?? "wwwroot/uploads/avatars";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadDir);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{userId}_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(fullPath, fileName);
            var relativeUrl = $"/{uploadDir.Replace("\\", "/")}/{fileName}";

            // Delete old avatar if exists and is a local file
            if (!string.IsNullOrEmpty(user.AvatarUrl) && user.AvatarUrl.StartsWith("/"))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), user.AvatarUrl.TrimStart('/'));
                if (File.Exists(oldPath))
                {
                    try { File.Delete(oldPath); } catch { /* ignore */ }
                }
            }

            // Save new file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update user avatar URL
            user.AvatarUrl = relativeUrl;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { url = relativeUrl }, "Tải lên ảnh đại diện thành công.");
        }
    }
}
