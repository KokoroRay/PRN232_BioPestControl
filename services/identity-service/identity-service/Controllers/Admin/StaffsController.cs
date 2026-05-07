using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using identity_service.DTOs;
using identity_service.Services;

namespace identity_service.Controllers.Admin
{
    /// <summary>
    /// Controller quản lý Staff — chỉ Admin mới được phép truy cập.
    /// Route: api/admin/staffs
    /// </summary>
    [Route("api/admin/staffs")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        // ─────────────────────────────────────────────────────────────
        // [GET] api/admin/staffs
        // Lấy danh sách Staff có phân trang, lọc, tìm kiếm
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Lấy danh sách tất cả Staff với phân trang và tìm kiếm.
        /// Query params: keyword, department, isActive, isFullAccess, page, pageSize, sortBy, sortDesc
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] StaffSearchRequest request)
        {
            // Validate page/pageSize
            if (request.Page < 1)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Số trang phải lớn hơn 0."
                });

            if (request.PageSize < 1 || request.PageSize > 100)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "PageSize phải nằm trong khoảng từ 1 đến 100."
                });

            var result = await _staffService.GetPagedAsync(request);

            return Ok(new ApiResponse<PagedResult<StaffSummaryDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data    = result.Data
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [GET] api/admin/staffs/search
        // Endpoint search riêng (alias của GetAll với keyword bắt buộc)
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Tìm kiếm Staff theo từ khóa (email, tên, phòng ban, chức danh).
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword,
                                                [FromQuery] int page = 1,
                                                [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Từ khóa tìm kiếm không được để trống."
                });

            if (keyword.Length < 2)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Từ khóa tìm kiếm phải có ít nhất 2 ký tự."
                });

            var request = new StaffSearchRequest
            {
                Keyword  = keyword,
                Page     = page,
                PageSize = pageSize
            };

            var result = await _staffService.GetPagedAsync(request);

            return Ok(new ApiResponse<PagedResult<StaffSummaryDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data    = result.Data
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [GET] api/admin/staffs/{id}
        // Lấy chi tiết một Staff (kèm permissions)
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Lấy chi tiết thông tin Staff theo ID, bao gồm danh sách quyền đã cấp.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _staffService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message
                });

            return Ok(new ApiResponse<StaffDto>
            {
                Success = true,
                Message = result.Message,
                Data    = result.Data
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [POST] api/admin/staffs
        // Tạo Staff mới + cấp quyền IAM
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Admin tạo tài khoản Staff mới.
        /// Hỗ trợ 2 chế độ IAM:
        /// - IsFullAccess = true: cấp toàn bộ quyền Manager (UC14-UC20)
        /// - IsFullAccess = false: chỉ cấp các quyền trong PermissionIds
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStaffRequest request)
        {
            // Model validation (DataAnnotations) tự xử lý bởi [ApiController]
            // Kiểm tra bổ sung: nếu không full access phải có ít nhất 1 quyền
            if (!request.IsFullAccess && !request.PermissionIds.Any())
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một quyền hoặc bật IsFullAccess."
                });

            // Lấy AdminId từ JWT Claims
            var adminId = GetCurrentUserId();
            if (adminId == null)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Token không hợp lệ hoặc đã hết hạn."
                });

            var result = await _staffService.CreateAsync(request, adminId.Value);

            return result.StatusCode switch
            {
                201 => StatusCode(201, new ApiResponse<StaffDto>
                {
                    Success = true,
                    Message = result.Message,
                    Data    = result.Data
                }),
                409 => Conflict(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message
                }),
                _ => BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message
                })
            };
        }

        // ─────────────────────────────────────────────────────────────
        // [PUT] api/admin/staffs/{id}
        // Cập nhật thông tin Staff + cập nhật quyền IAM
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Admin cập nhật thông tin Staff và phân quyền IAM.
        /// Quyền IAM sẽ được thay thế hoàn toàn bằng danh sách mới.
        /// Để đổi mật khẩu, cung cấp trường NewPassword.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffRequest request)
        {
            // Kiểm tra: không full access phải có quyền cụ thể
            if (!request.IsFullAccess && !request.PermissionIds.Any())
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một quyền hoặc bật IsFullAccess."
                });

            var adminId = GetCurrentUserId();
            if (adminId == null)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Token không hợp lệ hoặc đã hết hạn."
                });

            var result = await _staffService.UpdateAsync(id, request, adminId.Value);

            if (!result.Success)
            {
                return result.StatusCode switch
                {
                    404 => NotFound(new ApiResponse<object>   { Success = false, Message = result.Message }),
                    400 => BadRequest(new ApiResponse<object> { Success = false, Message = result.Message }),
                    _   => StatusCode(result.StatusCode, new ApiResponse<object> { Success = false, Message = result.Message })
                };
            }

            return Ok(new ApiResponse<StaffDto>
            {
                Success = true,
                Message = result.Message,
                Data    = result.Data
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [DELETE] api/admin/staffs/{id}
        // Xóa Staff
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Admin xóa hoàn toàn tài khoản Staff khỏi hệ thống.
        /// Lưu ý: Thao tác này sẽ xóa cả User account liên kết (cascade).
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _staffService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message
                });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = result.Message
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [PATCH] api/admin/staffs/{id}/status
        // Kích hoạt / khóa tài khoản Staff
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Admin kích hoạt hoặc khóa tài khoản Staff.
        /// Staff bị khóa sẽ không thể đăng nhập vào hệ thống.
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStaffStatusRequest request)
        {
            var adminId = GetCurrentUserId();
            if (adminId == null)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Token không hợp lệ hoặc đã hết hạn."
                });

            var result = await _staffService.UpdateStatusAsync(id, request, adminId.Value);

            if (!result.Success)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message
                });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = result.Message
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [GET] api/admin/staffs/{id}/permissions
        // Xem quyền hiện tại của Staff
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Lấy danh sách quyền hiện tại đã được cấp cho một Staff cụ thể.
        /// </summary>
        [HttpGet("{id:guid}/permissions")]
        public async Task<IActionResult> GetPermissions(Guid id)
        {
            var result = await _staffService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message
                });

            var staff = result.Data!;
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Staff có {staff.Permissions.Count} quyền. IsFullAccess: {staff.IsFullAccess}",
                Data    = new
                {
                    StaffId       = staff.Id,
                    IsFullAccess  = staff.IsFullAccess,
                    Permissions   = staff.Permissions
                }
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [PUT] api/admin/staffs/{id}/permissions
        // Cập nhật quyền IAM riêng lẻ (không cần cập nhật thông tin cơ bản)
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Admin cập nhật riêng phần quyền IAM của Staff.
        /// Toàn bộ quyền cũ sẽ bị thay thế bằng danh sách mới.
        /// </summary>
        [HttpPut("{id:guid}/permissions")]
        public async Task<IActionResult> UpdatePermissions(
            Guid id, [FromBody] UpdateStaffPermissionsRequest request)
        {
            if (!request.IsFullAccess && !request.PermissionIds.Any())
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một quyền hoặc bật IsFullAccess."
                });

            var adminId = GetCurrentUserId();
            if (adminId == null)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Token không hợp lệ hoặc đã hết hạn."
                });

            var result = await _staffService.UpdatePermissionsAsync(id, request, adminId.Value);

            if (!result.Success)
            {
                return result.StatusCode switch
                {
                    404 => NotFound(new ApiResponse<object>   { Success = false, Message = result.Message }),
                    400 => BadRequest(new ApiResponse<object> { Success = false, Message = result.Message }),
                    _   => StatusCode(result.StatusCode, new ApiResponse<object> { Success = false, Message = result.Message })
                };
            }

            return Ok(new ApiResponse<StaffDto>
            {
                Success = true,
                Message = result.Message,
                Data    = result.Data
            });
        }

        // ─────────────────────────────────────────────────────────────
        // PRIVATE HELPER
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Lấy UserId của Admin đang đăng nhập từ JWT Claims.
        /// </summary>
        private Guid? GetCurrentUserId()
        {
            var claim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (claim == null || !Guid.TryParse(claim, out var userId))
                return null;
            return userId;
        }
    }
}
