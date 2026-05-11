using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using trading_service.Data;
using trading_service.DTOs;
using trading_service.Models;

namespace trading_service.Controllers
{
    /// <summary>
    /// REST resource: /api/discounts — Guest/Customer/Staff: đọc; Admin: tạo/sửa/xóa (cùng logic phân quyền BioPestControl).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly TradingDbContext _context;

        public DiscountsController(TradingDbContext context)
        {
            _context = context;
        }

        /// <summary>GET /api/discounts — Danh sách (lọc tùy chọn). Cho phép khách vãng lai.</summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<List<DiscountDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDiscounts(
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? productId = null,
            [FromQuery] string? status = null)
        {
            var query = _context.Discounts.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(d => d.Name.Contains(term));
            }

            if (productId is > 0)
                query = query.Where(d => d.ProductId == productId.Value);

            var now = DateTime.UtcNow;

            // status: running | not_running — khớp filter monolith
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (string.Equals(status, "running", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(d => d.IsActive && d.StartDate <= now && d.EndDate >= now);
                else if (string.Equals(status, "not_running", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(d => !d.IsActive || d.EndDate < now || d.StartDate > now);
            }
            else if (isActive.HasValue)
            {
                if (isActive.Value)
                    query = query.Where(d => d.IsActive && d.StartDate <= now && d.EndDate >= now);
                else
                    query = query.Where(d => !d.IsActive || d.EndDate < now || d.StartDate > now);
            }

            var list = await query.OrderByDescending(d => d.Id).ToListAsync();
            var data = list.Select(MapToDto).ToList();

            return Ok(new ApiResponse<List<DiscountDto>>
            {
                Success = true,
                Message = "OK",
                Data = data
            });
        }

        /// <summary>GET /api/discounts/{id}</summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<DiscountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _context.Discounts.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (entity == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy khuyến mãi." });

            return Ok(new ApiResponse<DiscountDto>
            {
                Success = true,
                Data = MapToDto(entity)
            });
        }

        /// <summary>POST /api/discounts — Chỉ Admin.</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<DiscountDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateDiscountRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var err = ValidateDateRange(request.StartDate, request.EndDate);
            if (err != null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = err });

            var entity = new Discount
            {
                Name = request.Name.Trim(),
                DiscountPercent = request.DiscountPercent,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive,
                ProductId = request.ProductId,
                CreatedByAdminId = request.CreatedByAdminId ?? TryParseAdminIdFromClaims()
            };

            _context.Discounts.Add(entity);
            await _context.SaveChangesAsync();

            var dto = MapToDto(entity);
            return CreatedAtAction(
                nameof(GetById),
                new { id = entity.Id },
                new ApiResponse<DiscountDto> { Success = true, Message = "Đã tạo khuyến mãi.", Data = dto });
        }

        /// <summary>PUT /api/discounts/{id} — Chỉ Admin.</summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<DiscountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDiscountRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var err = ValidateDateRange(request.StartDate, request.EndDate);
            if (err != null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = err });

            var entity = await _context.Discounts.FirstOrDefaultAsync(d => d.Id == id);
            if (entity == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy khuyến mãi." });

            entity.Name = request.Name.Trim();
            entity.DiscountPercent = request.DiscountPercent;
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.IsActive = request.IsActive;
            entity.ProductId = request.ProductId;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<DiscountDto>
            {
                Success = true,
                Message = "Đã cập nhật khuyến mãi.",
                Data = MapToDto(entity)
            });
        }

        /// <summary>DELETE /api/discounts/{id} — Chỉ Admin.</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Discounts.FirstOrDefaultAsync(d => d.Id == id);
            if (entity == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy khuyến mãi." });

            _context.Discounts.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Đã xóa khuyến mãi." });
        }

        private static string? ValidateDateRange(DateTime start, DateTime end)
        {
            if (start >= end)
                return "Ngày bắt đầu phải trước ngày kết thúc.";
            return null;
        }

        private int? TryParseAdminIdFromClaims()
        {
            var v = User.FindFirst("AdminId")?.Value
                ?? User.FindFirst("admin_id")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(v, out var id) ? id : null;
        }

        private static DiscountDto MapToDto(Discount d)
        {
            var now = DateTime.UtcNow;
            var running = d.IsActive && d.StartDate <= now && d.EndDate >= now;
            return new DiscountDto
            {
                Id = d.Id,
                Name = d.Name,
                DiscountPercent = d.DiscountPercent,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                IsActive = d.IsActive,
                ProductId = d.ProductId,
                CreatedByAdminId = d.CreatedByAdminId,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                IsCurrentlyRunning = running
            };
        }
    }
}
