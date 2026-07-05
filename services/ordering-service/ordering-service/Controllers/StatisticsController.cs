using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ordering_service.Data;
using ordering_service.DTOs;
using ordering_service.Models;

namespace ordering_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền xem thống kê
    public class StatisticsController : ControllerBase
    {
        private readonly OrderingDbContext _context;

        public StatisticsController(OrderingDbContext context)
        {
            _context = context;
        }

        // 1. View total revenue
        [HttpGet("total-revenue")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetTotalRevenue()
        {
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount);

            return Ok(new ApiResponse<decimal>
            {
                Success = true,
                Message = "Lấy tổng doanh thu thành công.",
                Data = totalRevenue
            });
        }

        // 2. View total sold statistics
        [HttpGet("total-sold")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalSold()
        {
            var totalSold = await _context.OrderItems
                .Where(i => i.Order.Status != OrderStatus.Cancelled)
                .SumAsync(i => i.Quantity);

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Lấy tổng số lượng đã bán thành công.",
                Data = totalSold
            });
        }

        // 3. View total linked product (Số lượng sản phẩm duy nhất đã được bán)
        [HttpGet("total-linked-products")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalLinkedProducts()
        {
            var totalLinked = await _context.OrderItems
                .Where(i => i.Order.Status != OrderStatus.Cancelled)
                .Select(i => i.ProductId)
                .Distinct()
                .CountAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Lấy tổng số sản phẩm đã được liên kết trong đơn hàng thành công.",
                Data = totalLinked
            });
        }

        // 4. Filter statistic (Thống kê tổng hợp có lọc theo ngày)
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetSummary([FromQuery] StatsFilterRequest filter)
        {
            var query = _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered);

            if (filter.FromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= filter.ToDate.Value);
            }

            var totalRevenue = await query.SumAsync(o => o.TotalAmount);
            
            var totalSold = _context.OrderItems
                .Where(i => i.Order.Status != OrderStatus.Cancelled);
            
            if (filter.FromDate.HasValue)
                totalSold = totalSold.Where(i => i.Order.OrderDate >= filter.FromDate.Value);
            if (filter.ToDate.HasValue)
                totalSold = totalSold.Where(i => i.Order.OrderDate <= filter.ToDate.Value);

            var totalSoldQuantity = await totalSold.SumAsync(i => i.Quantity);
            var totalLinkedProducts = await totalSold.Select(i => i.ProductId).Distinct().CountAsync();

            return Ok(new ApiResponse<DashboardStatsDto>
            {
                Success = true,
                Data = new DashboardStatsDto
                {
                    TotalRevenue = totalRevenue,
                    TotalSoldQuantity = totalSoldQuantity,
                    TotalLinkedProducts = totalLinkedProducts
                }
            });
        }

        // Thống kê doanh thu theo ngày (để vẽ biểu đồ)
        [HttpGet("revenue-chart")]
        public async Task<ActionResult<ApiResponse<List<RevenueStatDto>>>> GetRevenueChart([FromQuery] StatsFilterRequest filter)
        {
            var query = _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered);

            if (filter.FromDate.HasValue)
                query = query.Where(o => o.OrderDate >= filter.FromDate.Value);
            if (filter.ToDate.HasValue)
                query = query.Where(o => o.OrderDate <= filter.ToDate.Value);

            var stats = await query
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new RevenueStatDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(s => s.Date)
                .ToListAsync();

            return Ok(new ApiResponse<List<RevenueStatDto>>
            {
                Success = true,
                Data = stats
            });
        }
    }
}
