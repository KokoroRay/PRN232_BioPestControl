using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using identity_service.Data;
using identity_service.DTOs;
using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;

namespace identity_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // [GET] api/users - Lấy danh sách toàn bộ Users (chỉ Admin)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDto
                {
                    Id         = u.Id,
                    Email      = u.Email,
                    FullName   = u.FullName,
                    AvatarUrl  = u.AvatarUrl,
                    PhoneNumber = u.PhoneNumber,
                    Role       = u.Role,
                    IsActive   = u.IsActive,
                    CreatedAt  = u.CreatedAt,
                    UpdatedAt  = u.UpdatedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<UserDto>>
            {
                Success = true,
                Message = $"Lấy danh sách thành công ({users.Count} người dùng)",
                Data = users
            });
        }

        // [GET] api/users/me - Lấy thông tin của chính mình (User đang đăng nhập)
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            // Lấy UserId từ JWT Claims (claim "sub")
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy người dùng." });

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Data = new UserDto
                {
                    Id          = user.Id,
                    Email       = user.Email,
                    FullName    = user.FullName,
                    AvatarUrl   = user.AvatarUrl,
                    PhoneNumber = user.PhoneNumber,
                    Role        = user.Role,
                    IsActive    = user.IsActive,
                    CreatedAt   = user.CreatedAt,
                    UpdatedAt   = user.UpdatedAt
                }
            });
        }

        // [GET] api/users/{id} - Lấy thông tin 1 User theo ID (chỉ Admin)
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy người dùng." });

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Data = new UserDto
                {
                    Id          = user.Id,
                    Email       = user.Email,
                    FullName    = user.FullName,
                    AvatarUrl   = user.AvatarUrl,
                    PhoneNumber = user.PhoneNumber,
                    Role        = user.Role,
                    IsActive    = user.IsActive,
                    CreatedAt   = user.CreatedAt,
                    UpdatedAt   = user.UpdatedAt
                }
            });
        }

        // [PUT] api/users/{id} - Cập nhật thông tin User (FullName, PhoneNumber, AvatarUrl)
        // Admin cập nhật bất kỳ user, User chỉ cập nhật chính mình
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy người dùng." });

            // Kiểm tra quyền: nếu không phải Admin thì chỉ được sửa tài khoản của mình
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            var isAdmin     = User.IsInRole("Admin");

            if (!isAdmin && userIdClaim != id.ToString())
                return StatusCode(403, new ApiResponse<object> { Success = false, Message = "Bạn không có quyền chỉnh sửa tài khoản này." });

            // Chỉ cập nhật những trường được gửi lên (partial update style)
            if (request.FullName   != null) user.FullName   = request.FullName;
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;
            if (request.AvatarUrl  != null) user.AvatarUrl  = request.AvatarUrl;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Cập nhật thông tin thành công." });
        }

        // [PATCH] api/users/{id}/role - Thay đổi Role của User (chỉ Admin)
        [HttpPatch("{id:guid}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleRequest request)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy người dùng." });

            user.Role      = request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = $"Đã cập nhật Role thành '{request.Role}'." });
        }

        // [PATCH] api/users/{id}/status - Kích hoạt hoặc khóa tài khoản (chỉ Admin)
        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserStatusRequest request)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy người dùng." });

            user.IsActive  = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var statusText = request.IsActive ? "kích hoạt" : "khóa";
            return Ok(new ApiResponse<object> { Success = true, Message = $"Tài khoản đã được {statusText} thành công." });
        }

        // [DELETE] api/users/{id} - Xóa User (chỉ Admin)
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy người dùng." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Xóa tài khoản thành công." });
        }
    }
}
