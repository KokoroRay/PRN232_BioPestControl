using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using identity_service.DTOs;
using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;
using identity_service.Services.Interfaces;
using System.Security.Claims;
using System.IO;

namespace identity_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires login for all endpoints
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IConfiguration _configuration;

        public ProfileController(IProfileService profileService, IConfiguration configuration)
        {
            _profileService = profileService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ hoặc đã hết hạn." });
            }

            var result = await _profileService.GetProfileAsync(userId.Value);

            if (!result.Success)
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });

            return Ok(new ApiResponse<ProfileDto> { Success = true, Message = result.Message, Data = result.Data });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ hoặc đã hết hạn." });
            }

            var result = await _profileService.UpdateProfileAsync(userId.Value, request);

            if (!result.Success)
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });

            return Ok(new ApiResponse<ProfileDto> { Success = true, Message = result.Message, Data = result.Data });
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ hoặc đã hết hạn." });
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Vui lòng chọn một file." });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Chỉ chấp nhận file ảnh (jpg, png, gif, webp)." });
            }

            // Validate file size (max 1MB)
            if (file.Length > 1 * 1024 * 1024)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "File phải nhỏ hơn 1MB." });
            }

            var result = await _profileService.UploadAvatarAsync(userId.Value, file);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Message });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = result.Message, Data = result.Data });
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ hoặc đã hết hạn." });
            }

            var result = await _profileService.ChangePasswordAsync(userId.Value, request);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Message });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = result.Message });
        }

        private Guid? GetCurrentUserId()
        {
            var claim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (claim == null || !Guid.TryParse(claim, out var userId)) return null;
            return userId;
        }
    }
}
