using inventory_service.DTOs.Requests;
using inventory_service.DTOs.Responses;
using inventory_service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace inventory_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Staff và Admin xem danh sách tồn kho hiện tại (Filter, Search, Sort)
        /// </summary>
        [HttpGet("stock")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetStock([FromQuery] string? search, [FromQuery] string? sortBy, [FromQuery] bool ascending = true, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _inventoryService.GetProductStocksAsync(search, sortBy, ascending, page, pageSize);
            return Ok(ApiResponse<PagedResult<ProductStockResponse>>.Ok(result));
        }

        /// <summary>
        /// Admin xem chi tiết sản phẩm kèm lịch sử nhập kho (Filter, Search, Sort)
        /// </summary>
        [HttpGet("details")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDetails([FromQuery] string? search, [FromQuery] string? sortBy, [FromQuery] bool ascending = true, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _inventoryService.GetProductDetailsAsync(search, sortBy, ascending, page, pageSize);
            return Ok(ApiResponse<PagedResult<ProductDetailResponse>>.Ok(result));
        }

        /// <summary>
        /// Xem chi tiết một sản phẩm và lịch sử nhập kho của nó
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _inventoryService.GetProductDetailByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<string>.Fail("Không tìm thấy sản phẩm."));

            return Ok(ApiResponse<ProductDetailResponse>.Ok(result));
        }

        /// <summary>
        /// Admin nhập kho (Import) cùng lúc nhiều sản phẩm
        /// </summary>
        [HttpPost("import")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportProducts([FromBody] ImportProductsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu đầu vào không hợp lệ."));

            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.Email) ?? "Admin";
                
                if (!Guid.TryParse(userIdStr, out var adminId))
                {
                    adminId = Guid.Empty; // Fallback for test
                }

                var result = await _inventoryService.ImportProductsAsync(request, adminId, userName);
                return Ok(ApiResponse<ImportBatchSummaryResponse>.Ok(result, "Nhập kho thành công."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail("Lỗi hệ thống khi nhập kho: " + ex.Message));
            }
        }
    }
}
