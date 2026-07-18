using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using ordering_service.Data;
using ordering_service.DTOs;
using ordering_service.Models;

namespace ordering_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer,Admin,Staff")] // Thêm Staff để không bị lỗi 403 khi load layout
    public class CartController : ControllerBase
    {
        private readonly OrderingDbContext _context;

        public CartController(OrderingDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────
        // Helper: Lấy CustomerId từ JWT token hiện tại
        // ─────────────────────────────────────────────
        private Guid? GetCurrentCustomerId()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }

        // Helper: Lấy hoặc tạo mới Cart cho Customer hiện tại
        private async Task<Cart> GetOrCreateCartAsync(Guid customerId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        // ─────────────────────────────────────────────────────────
        // [GET] api/cart  —  Xem toàn bộ giỏ hàng của mình
        // ─────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            // Nếu chưa có giỏ hàng, trả về giỏ rỗng
            if (cart == null)
            {
                return Ok(new ApiResponse<CartDto>
                {
                    Success = true,
                    Message = "Giỏ hàng của bạn đang trống.",
                    Data = new CartDto { CustomerId = customerId.Value }
                });
            }

            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Data = MapToCartDto(cart)
            });
        }

        // ─────────────────────────────────────────────────────────
        // [POST] api/cart/items  —  Thêm sản phẩm vào giỏ hàng
        // Nếu sản phẩm đã có trong giỏ → cộng thêm số lượng
        // ─────────────────────────────────────────────────────────
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            var cart = await GetOrCreateCartAsync(customerId.Value);

            // Kiểm tra xem sản phẩm này đã có trong giỏ chưa
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            bool isNewItem   = existingItem == null;

            if (!isNewItem)
            {
                // Sản phẩm đã có → chỉ cộng thêm số lượng (không tạo resource mới → 200 OK)
                existingItem!.Quantity  += request.Quantity;
                existingItem.UpdatedAt   = DateTime.UtcNow;
            }
            else
            {
                // Sản phẩm chưa có → tạo mới CartItem (resource mới → 201 Created)
                var newItem = new CartItem
                {
                    CartId          = cart.Id,
                    ProductId       = request.ProductId,
                    ProductName     = request.ProductName,
                    UnitPrice       = request.UnitPrice,
                    ProductImageUrl = request.ProductImageUrl,
                    Quantity        = request.Quantity
                };
                _context.CartItems.Add(newItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var responseData = new ApiResponse<CartDto>
            {
                Success = true,
                Message = isNewItem ? "Đã thêm sản phẩm mới vào giỏ hàng." : "Đã cập nhật số lượng sản phẩm trong giỏ.",
                Data    = MapToCartDto(cart)
            };

            // 201 Created khi thêm mới, 200 OK khi cộng dồn số lượng
            return isNewItem ? StatusCode(201, responseData) : Ok(responseData);
        }

        // ─────────────────────────────────────────────────────────
        // [PUT] api/cart/items/{itemId}  —  Cập nhật số lượng
        // ─────────────────────────────────────────────────────────
        [HttpPut("items/{itemId:guid}")]
        public async Task<IActionResult> UpdateCartItem(Guid itemId, [FromBody] UpdateCartItemRequest request)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            // Lấy cart item và xác nhận nó thuộc về giỏ của customer hiện tại
            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.Cart.CustomerId == customerId);

            if (item == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy sản phẩm trong giỏ hàng." });

            item.Quantity  = request.Quantity;
            item.UpdatedAt = DateTime.UtcNow;

            item.Cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<CartItemDto>
            {
                Success = true,
                Message = "Đã cập nhật số lượng.",
                Data = MapToCartItemDto(item)
            });
        }

        // ─────────────────────────────────────────────────────────
        // [DELETE] api/cart/items/{itemId}  —  Xóa 1 sản phẩm khỏi giỏ
        // ─────────────────────────────────────────────────────────
        [HttpDelete("items/{itemId:guid}")]
        public async Task<IActionResult> RemoveFromCart(Guid itemId)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.Cart.CustomerId == customerId);

            if (item == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy sản phẩm trong giỏ hàng." });

            _context.CartItems.Remove(item);

            item.Cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Đã xóa sản phẩm khỏi giỏ hàng." });
        }

        // ─────────────────────────────────────────────────────────
        // [DELETE] api/cart  —  Xóa toàn bộ giỏ hàng (Clear cart)
        // ─────────────────────────────────────────────────────────
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token không hợp lệ." });

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null || !cart.Items.Any())
                return Ok(new ApiResponse<object> { Success = true, Message = "Giỏ hàng của bạn đã trống." });

            _context.CartItems.RemoveRange(cart.Items);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Đã xóa toàn bộ giỏ hàng." });
        }

        // ─────────────────────────────────────────────────────────
        // Helper: Map Cart entity → CartDto
        // ─────────────────────────────────────────────────────────
        private static CartDto MapToCartDto(Cart cart) => new CartDto
        {
            Id         = cart.Id,
            CustomerId = cart.CustomerId,
            Items      = cart.Items.Select(MapToCartItemDto).ToList(),
            CreatedAt  = cart.CreatedAt,
            UpdatedAt  = cart.UpdatedAt
        };

        private static CartItemDto MapToCartItemDto(CartItem item) => new CartItemDto
        {
            Id              = item.Id,
            ProductId       = item.ProductId,
            ProductName     = item.ProductName,
            ProductImageUrl = item.ProductImageUrl,
            UnitPrice       = item.UnitPrice,
            Quantity        = item.Quantity,
            AddedAt         = item.AddedAt,
            UpdatedAt       = item.UpdatedAt
        };
    }
}
