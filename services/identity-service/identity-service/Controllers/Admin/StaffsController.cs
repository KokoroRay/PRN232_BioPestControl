using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using identity_service.DTOs;
using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;
using identity_service.Services.Interfaces;

namespace identity_service.Controllers.Admin
{
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
        // ─────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] StaffSearchRequest request)
        {
            if (request.Page < 1)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Số trang phải lớn hơn 0." });
            if (request.PageSize < 1 || request.PageSize > 100)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "PageSize phải nằm trong khoảng từ 1 đến 100." });

            var result = await _staffService.GetPagedAsync(request);

            return Ok(new ApiResponse<PagedResult<StaffDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data    = result.Data
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [GET] api/admin/staffs/search
        // ─────────────────────────────────────────────────────────────
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword,
                                                [FromQuery] int page = 1,
                                                [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Từ khóa tìm kiếm không được để trống." });

            if (keyword.Length < 2)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Từ khóa tìm kiếm phải có ít nhất 2 ký tự." });

            var request = new StaffSearchRequest { Keyword = keyword, Page = page, PageSize = pageSize };
            var result  = await _staffService.GetPagedAsync(request);

            return Ok(new ApiResponse<PagedResult<StaffDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data    = result.Data
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [GET] api/admin/staffs/{id}
        // ─────────────────────────────────────────────────────────────
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _staffService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });

            return Ok(new ApiResponse<StaffDto> { Success = true, Message = result.Message, Data = result.Data });
        }

        // ─────────────────────────────────────────────────────────────
        // [POST] api/admin/staffs
        // ─────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStaffRequest request)
        {
            if (!request.IsFullAccess && !request.PermissionIds.Any())
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Vui lòng chọn ít nhất một quyền hoặc bật IsFullAccess." });

            var adminId = GetCurrentUserId();
            if (adminId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ hoặc đã hết hạn." });

            var result = await _staffService.CreateAsync(request, adminId.Value);

            return result.StatusCode switch
            {
                201 => StatusCode(201, new ApiResponse<StaffDto> { Success = true, Message = result.Message, Data = result.Data }),
                409 => Conflict(new ApiResponse<object>          { Success = false, Message = result.Message }),
                _   => BadRequest(new ApiResponse<object>        { Success = false, Message = result.Message })
            };
        }

        // ─────────────────────────────────────────────────────────────
        // [PUT] api/admin/staffs/{id}
        // ─────────────────────────────────────────────────────────────
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffRequest request)
        {
            if (!request.IsFullAccess && !request.PermissionIds.Any())
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Vui lòng chọn ít nhất một quyền hoặc bật IsFullAccess." });

            var adminId = GetCurrentUserId();
            if (adminId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ hoặc đã hết hạn." });

            var result = await _staffService.UpdateAsync(id, request, adminId.Value);

            if (!result.Success)
            {
                return result.StatusCode switch
                {
                    404 => NotFound(new ApiResponse<object>  { Success = false, Message = result.Message }),
                    400 => BadRequest(new ApiResponse<object>{ Success = false, Message = result.Message }),
                    _   => StatusCode(result.StatusCode, new ApiResponse<object> { Success = false, Message = result.Message })
                };
            }

            return Ok(new ApiResponse<StaffDto> { Success = true, Message = result.Message, Data = result.Data });
        }

        // ─────────────────────────────────────────────────────────────
        // [DELETE] api/admin/staffs/{id}
        // ─────────────────────────────────────────────────────────────
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _staffService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });

            return Ok(new ApiResponse<object> { Success = true, Message = result.Message });
        }

        // ─────────────────────────────────────────────────────────────
        // [PATCH] api/admin/staffs/{id}/status
        // ─────────────────────────────────────────────────────────────
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStaffStatusRequest request)
        {
            var adminId = GetCurrentUserId();
            if (adminId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ hoặc đã hết hạn." });

            var result = await _staffService.UpdateStatusAsync(id, request, adminId.Value);

            if (!result.Success)
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });

            return Ok(new ApiResponse<object> { Success = true, Message = result.Message });
        }

        // ─────────────────────────────────────────────────────────────
        // [GET] api/admin/staffs/{id}/permissions
        // ─────────────────────────────────────────────────────────────
        [HttpGet("{id:guid}/permissions")]
        public async Task<IActionResult> GetPermissions(Guid id)
        {
            var result = await _staffService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });

            var staff = result.Data!;
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Staff có {staff.PermissionCount} quyền. IsFullAccess: {staff.IsFullAccess}",
                Data    = new { StaffId = staff.Id, staff.IsFullAccess, staff.Permissions }
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [PUT] api/admin/staffs/{id}/permissions
        // ─────────────────────────────────────────────────────────────
        [HttpPut("{id:guid}/permissions")]
        public async Task<IActionResult> UpdatePermissions(Guid id, [FromBody] UpdateStaffPermissionsRequest request)
        {
            if (!request.IsFullAccess && !request.PermissionIds.Any())
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Vui lòng chọn ít nhất một quyền hoặc bật IsFullAccess." });

            var adminId = GetCurrentUserId();
            if (adminId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ hoặc đã hết hạn." });

            var result = await _staffService.UpdatePermissionsAsync(id, request, adminId.Value);

            if (!result.Success)
            {
                return result.StatusCode switch
                {
                    404 => NotFound(new ApiResponse<object>  { Success = false, Message = result.Message }),
                    400 => BadRequest(new ApiResponse<object>{ Success = false, Message = result.Message }),
                    _   => StatusCode(result.StatusCode, new ApiResponse<object> { Success = false, Message = result.Message })
                };
            }

            return Ok(new ApiResponse<StaffDto> { Success = true, Message = result.Message, Data = result.Data });
        }

        // ─────────────────────────────────────────────────────────────
        // PRIVATE HELPER
        // ─────────────────────────────────────────────────────────────
        private Guid? GetCurrentUserId()
        {
            var claim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (claim == null || !Guid.TryParse(claim, out var userId)) return null;
            return userId;
        }
    }
}
