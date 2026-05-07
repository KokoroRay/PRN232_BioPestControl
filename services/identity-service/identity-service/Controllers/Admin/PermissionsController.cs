using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using identity_service.DTOs;
using identity_service.Services;

namespace identity_service.Controllers.Admin
{
    /// <summary>
    /// Controller cung cấp danh sách Permissions — chỉ Admin mới được phép truy cập.
    /// Dùng để Admin lấy danh sách quyền khi tạo/sửa Staff (hiển thị checkbox UI).
    /// Route: api/admin/permissions
    /// </summary>
    [Route("api/admin/permissions")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        // ─────────────────────────────────────────────────────────────
        // [GET] api/admin/permissions
        // Lấy toàn bộ danh sách Permission (dạng flat list)
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Lấy toàn bộ danh sách quyền (Permission) đang active trong hệ thống.
        /// Dùng khi cần bind ID vào request.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _permissionService.GetAllActiveAsync();

            return Ok(new ApiResponse<List<PermissionDto>>
            {
                Success = true,
                Message = result.Message,
                Data    = result.Data
            });
        }

        // ─────────────────────────────────────────────────────────────
        // [GET] api/admin/permissions/grouped
        // Lấy danh sách Permission được nhóm theo UC (để render checkbox UI)
        // ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Lấy danh sách quyền được nhóm theo Use Case (UC14-UC20).
        /// Dùng để render giao diện checkbox phân quyền cho Admin.
        /// </summary>
        [HttpGet("grouped")]
        public async Task<IActionResult> GetGrouped()
        {
            var result = await _permissionService.GetGroupedAsync();

            return Ok(new ApiResponse<List<PermissionGroupDto>>
            {
                Success = true,
                Message = result.Message,
                Data    = result.Data
            });
        }
    }
}
