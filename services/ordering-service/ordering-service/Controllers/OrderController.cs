using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ordering_service.Data;
using ordering_service.DTOs;
using ordering_service.Kafka;
using ordering_service.Models;

namespace ordering_service.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderingDbContext _context;
        private readonly IOrderEventProducer _producer;
        private readonly ILogger<OrderController> _logger;

        // Bảng trạng thái tịnh tuyến: key = trạng thái hiện tại, value = trạng thái tiếp theo hợp lệ
        private static readonly Dictionary<OrderStatus, OrderStatus> _nextStatusMap = new()
        {
            { OrderStatus.WaitingConfirmation, OrderStatus.Confirmed  },
            { OrderStatus.Confirmed,           OrderStatus.Processing },
            { OrderStatus.Processing,          OrderStatus.Shipping   },
            { OrderStatus.Shipping,            OrderStatus.Delivered  }
            // Delivered & Cancelled là trạng thái cuối, không có next
        };

        public OrderController(OrderingDbContext context, IOrderEventProducer producer, ILogger<OrderController> logger)
        {
            _context  = context;
            _producer = producer;
            _logger   = logger;
        }

        // ─────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────

        private Guid? GetUserId()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }

        private string GetUserRole() =>
            User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        // ─────────────────────────────────────────────────────────────────────
        // CUSTOMER ENDPOINTS
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// [POST] /api/orders
        /// Customer đặt hàng từ cart hiện tại.
        /// Publish Kafka event → inventory-service tự động trừ kho.
        /// </summary>
        [HttpPost("api/orders")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            var customerId = GetUserId();
            if (customerId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            // Lấy cart của customer
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null || !cart.Items.Any())
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi đặt hàng." });

            // Parse payment method
            if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Phương thức thanh toán không hợp lệ. Chỉ hỗ trợ COD hoặc PayOS." });

            // Tạo Order
            var order = new Order
            {
                CustomerId     = customerId.Value,
                TotalAmount    = cart.Items.Sum(i => i.UnitPrice * i.Quantity),
                ShippingAddress = request.ShippingAddress,
                PaymentMethod  = paymentMethod,
                Status         = OrderStatus.WaitingConfirmation,
                PaymentStatus  = PaymentStatus.Unpaid
            };

            // Tạo OrderItems từ CartItems (snapshot data)
            foreach (var item in cart.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId       = item.ProductId,
                    ProductName     = item.ProductName,
                    ProductImageUrl = item.ProductImageUrl,
                    UnitPrice       = item.UnitPrice,
                    Quantity        = item.Quantity
                });
            }

            _context.Orders.Add(order);

            // Xóa cart sau khi đặt hàng
            _context.CartItems.RemoveRange(cart.Items);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            // Publish Kafka event (không blocking — lỗi Kafka không fail đơn hàng)
            var orderEvent = new OrderPlacedEvent
            {
                OrderId    = order.Id,
                CustomerId = order.CustomerId,
                PlacedAt   = order.OrderDate,
                Items      = order.OrderItems.Select(i => new OrderItemEvent
                {
                    ProductId = i.ProductId,
                    Quantity  = i.Quantity
                }).ToList()
            };
            await _producer.PublishOrderPlacedAsync(orderEvent);

            return StatusCode(201, new ApiResponse<OrderResponse>
            {
                Success = true,
                Message = "Đặt hàng thành công!",
                Data    = MapToOrderResponse(order)
            });
        }

        /// <summary>
        /// [GET] /api/orders
        /// Customer xem danh sách đơn hàng của mình (có filter, tìm kiếm).
        /// </summary>
        [HttpGet("api/orders")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderFilterRequest filter)
        {
            var customerId = GetUserId();
            if (customerId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .AsQueryable();

            query = ApplyFilter(query, filter);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return Ok(new ApiResponse<PagedResult<OrderResponse>>
            {
                Success = true,
                Data = new PagedResult<OrderResponse>
                {
                    Items      = items.Select(MapToOrderResponse).ToList(),
                    TotalCount = total,
                    Page       = filter.Page,
                    PageSize   = filter.PageSize
                }
            });
        }

        /// <summary>
        /// [GET] /api/orders/{id}
        /// Customer xem chi tiết một đơn hàng (chỉ đơn của chính mình).
        /// </summary>
        [HttpGet("api/orders/{id:guid}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrderById(Guid id)
        {
            var customerId = GetUserId();
            if (customerId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customerId);

            if (order == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đơn hàng." });

            return Ok(new ApiResponse<OrderResponse> { Success = true, Data = MapToOrderResponse(order) });
        }

        /// <summary>
        /// [GET] /api/orders/history
        /// Customer xem lịch sử đơn hàng (đã giao hoặc đã hủy).
        /// </summary>
        [HttpGet("api/orders/history")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrderHistory([FromQuery] OrderFilterRequest filter)
        {
            var customerId = GetUserId();
            if (customerId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId
                         && (o.Status == OrderStatus.Delivered || o.Status == OrderStatus.Cancelled))
                .AsQueryable();

            query = ApplyFilter(query, filter);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return Ok(new ApiResponse<PagedResult<OrderResponse>>
            {
                Success = true,
                Data = new PagedResult<OrderResponse>
                {
                    Items      = items.Select(MapToOrderResponse).ToList(),
                    TotalCount = total,
                    Page       = filter.Page,
                    PageSize   = filter.PageSize
                }
            });
        }

        /// <summary>
        /// [DELETE] /api/orders/{id}/cancel
        /// Customer hủy đơn hàng — CHỈ được hủy khi đơn ở trạng thái WaitingConfirmation.
        /// </summary>
        [HttpDelete("api/orders/{id:guid}/cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CustomerCancelOrder(Guid id)
        {
            var customerId = GetUserId();
            if (customerId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customerId);

            if (order == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đơn hàng." });

            if (order.Status != OrderStatus.WaitingConfirmation)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Không thể hủy đơn hàng ở trạng thái '{order.Status}'. Chỉ có thể hủy khi đơn đang chờ xác nhận."
                });

            order.Status        = OrderStatus.Cancelled;
            order.CancelledAt   = DateTime.UtcNow;
            order.CancelledByUserId = customerId;
            order.CancelledByRole   = "Customer";
            order.UpdatedAt     = DateTime.UtcNow;

            // Nếu đã thanh toán → chuyển sang chờ hoàn tiền
            if (order.PaymentStatus == PaymentStatus.Paid)
                order.PaymentStatus = PaymentStatus.Refunded;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<OrderResponse>
            {
                Success = true,
                Message = order.PaymentStatus == PaymentStatus.Refunded
                    ? "Đã hủy đơn hàng. Vì bạn đã thanh toán, chúng tôi sẽ xử lý hoàn tiền cho bạn."
                    : "Đã hủy đơn hàng thành công.",
                Data = MapToOrderResponse(order)
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // STAFF ENDPOINTS
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// [GET] /api/staff/orders
        /// Staff xem tất cả đơn hàng (có filter, search, phân trang).
        /// </summary>
        [HttpGet("api/staff/orders")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffGetAllOrders([FromQuery] OrderFilterRequest filter)
        {
            return await GetAllOrdersInternal(filter);
        }

        /// <summary>
        /// [GET] /api/staff/orders/{id}
        /// Staff xem chi tiết đơn hàng.
        /// </summary>
        [HttpGet("api/staff/orders/{id:guid}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffGetOrderById(Guid id)
        {
            return await GetOrderByIdInternal(id);
        }

        /// <summary>
        /// [PUT] /api/staff/orders/{id}/status
        /// Staff cập nhật trạng thái đơn hàng theo luồng tịnh tuyến.
        /// </summary>
        [HttpPut("api/staff/orders/{id:guid}/status")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffUpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            return await UpdateOrderStatusInternal(id, request);
        }

        /// <summary>
        /// [DELETE] /api/staff/orders/{id}/cancel
        /// Staff hủy đơn hàng — bắt buộc có lý do hủy.
        /// </summary>
        [HttpDelete("api/staff/orders/{id:guid}/cancel")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffCancelOrder(Guid id, [FromBody] CancelOrderRequest request)
        {
            return await CancelOrderByStaffAdminInternal(id, request, "Staff");
        }

        // ─────────────────────────────────────────────────────────────────────
        // ADMIN ENDPOINTS
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// [GET] /api/admin/orders
        /// Admin xem tất cả đơn hàng (có filter, search, phân trang).
        /// </summary>
        [HttpGet("api/admin/orders")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminGetAllOrders([FromQuery] OrderFilterRequest filter)
        {
            return await GetAllOrdersInternal(filter);
        }

        /// <summary>
        /// [GET] /api/admin/orders/{id}
        /// Admin xem chi tiết đơn hàng.
        /// </summary>
        [HttpGet("api/admin/orders/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminGetOrderById(Guid id)
        {
            return await GetOrderByIdInternal(id);
        }

        /// <summary>
        /// [PUT] /api/admin/orders/{id}/status
        /// Admin cập nhật trạng thái đơn hàng theo luồng tịnh tuyến.
        /// </summary>
        [HttpPut("api/admin/orders/{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminUpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            return await UpdateOrderStatusInternal(id, request);
        }

        /// <summary>
        /// [DELETE] /api/admin/orders/{id}/cancel
        /// Admin hủy đơn hàng — bắt buộc có lý do hủy.
        /// </summary>
        [HttpDelete("api/admin/orders/{id:guid}/cancel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminCancelOrder(Guid id, [FromBody] CancelOrderRequest request)
        {
            return await CancelOrderByStaffAdminInternal(id, request, "Admin");
        }

        // ─────────────────────────────────────────────────────────────────────
        // INTERNAL SHARED LOGIC
        // ─────────────────────────────────────────────────────────────────────

        private async Task<IActionResult> GetAllOrdersInternal(OrderFilterRequest filter)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .AsQueryable();

            query = ApplyFilter(query, filter);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return Ok(new ApiResponse<PagedResult<OrderResponse>>
            {
                Success = true,
                Data = new PagedResult<OrderResponse>
                {
                    Items      = items.Select(MapToOrderResponse).ToList(),
                    TotalCount = total,
                    Page       = filter.Page,
                    PageSize   = filter.PageSize
                }
            });
        }

        private async Task<IActionResult> GetOrderByIdInternal(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đơn hàng." });

            return Ok(new ApiResponse<OrderResponse> { Success = true, Data = MapToOrderResponse(order) });
        }

        private async Task<IActionResult> UpdateOrderStatusInternal(Guid id, UpdateOrderStatusRequest request)
        {
            if (!Enum.TryParse<OrderStatus>(request.NewStatus, true, out var newStatus))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Trạng thái '{request.NewStatus}' không hợp lệ. Các trạng thái hợp lệ: {string.Join(", ", Enum.GetNames<OrderStatus>())}"
                });

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đơn hàng." });

            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Delivered)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Đơn hàng ở trạng thái '{order.Status}' không thể cập nhật thêm."
                });

            // Kiểm tra tịnh tuyến: newStatus phải là trạng thái tiếp theo hợp lệ
            if (!_nextStatusMap.TryGetValue(order.Status, out var expectedNext) || expectedNext != newStatus)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Không thể chuyển từ '{order.Status}' sang '{newStatus}'. Trạng thái tiếp theo hợp lệ là '{(_nextStatusMap.TryGetValue(order.Status, out var next) ? next : "không có")}' ."
                });

            order.Status    = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            // Tự động cập nhật PaymentStatus nếu COD và đã giao thành công
            if (newStatus == OrderStatus.Delivered && order.PaymentMethod == PaymentMethod.COD)
            {
                order.PaymentStatus = PaymentStatus.Paid;
                order.PaidAt        = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<OrderResponse>
            {
                Success = true,
                Message = $"Đã cập nhật trạng thái đơn hàng sang '{newStatus}'.",
                Data    = MapToOrderResponse(order)
            });
        }

        private async Task<IActionResult> CancelOrderByStaffAdminInternal(Guid id, CancelOrderRequest request, string role)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đơn hàng." });

            if (order.Status == OrderStatus.Cancelled)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Đơn hàng này đã bị hủy trước đó." });

            if (order.Status == OrderStatus.Delivered)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Không thể hủy đơn hàng đã giao thành công." });

            order.Status            = OrderStatus.Cancelled;
            order.CancelledAt       = DateTime.UtcNow;
            order.CancelledByUserId = userId;
            order.CancelledByRole   = role;
            order.CancellationReason = request.Reason;
            order.UpdatedAt         = DateTime.UtcNow;

            // Nếu đã thanh toán → đánh dấu chờ hoàn tiền
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                order.PaymentStatus = PaymentStatus.Refunded;
                order.RefundedAt    = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<OrderResponse>
            {
                Success = true,
                Message = order.PaymentStatus == PaymentStatus.Refunded
                    ? $"Đã hủy đơn hàng. Vì đơn đã được thanh toán, sẽ được hoàn tiền lại cho khách hàng."
                    : "Đã hủy đơn hàng thành công.",
                Data = MapToOrderResponse(order)
            });
        }

        private static IQueryable<Order> ApplyFilter(IQueryable<Order> query, OrderFilterRequest filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<OrderStatus>(filter.Status, true, out var status))
                query = query.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                // Tìm theo OrderId (prefix) hoặc tên sản phẩm trong đơn
                var searchLower = filter.Search.ToLower();
                query = query.Where(o =>
                    o.Id.ToString().Contains(searchLower) ||
                    o.OrderItems.Any(i => i.ProductName.ToLower().Contains(searchLower)));
            }

            if (filter.FromDate.HasValue)
                query = query.Where(o => o.OrderDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(o => o.OrderDate <= filter.ToDate.Value);

            return query;
        }

        // ─────────────────────────────────────────────
        // MAPPER
        // ─────────────────────────────────────────────
        private static OrderResponse MapToOrderResponse(Order order) => new OrderResponse
        {
            Id              = order.Id,
            CustomerId      = order.CustomerId,
            OrderDate       = order.OrderDate,
            UpdatedAt       = order.UpdatedAt,
            Status          = order.Status.ToString(),
            StatusCode      = (int)order.Status,
            PaymentStatus   = order.PaymentStatus.ToString(),
            PaymentStatusCode = (int)order.PaymentStatus,
            PaymentMethod   = order.PaymentMethod.ToString(),
            PaidAt          = order.PaidAt,
            RefundedAt      = order.RefundedAt,
            TotalAmount     = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            CancelledByUserId   = order.CancelledByUserId,
            CancelledByRole     = order.CancelledByRole,
            CancellationReason  = order.CancellationReason,
            CancelledAt         = order.CancelledAt,
            Items = order.OrderItems.Select(i => new OrderItemResponse
            {
                Id              = i.Id,
                ProductId       = i.ProductId,
                ProductName     = i.ProductName,
                ProductImageUrl = i.ProductImageUrl,
                UnitPrice       = i.UnitPrice,
                Quantity        = i.Quantity
            }).ToList()
        };
    }
}
