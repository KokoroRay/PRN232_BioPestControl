using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using engagement_service.Data;
using engagement_service.DTOs;
using engagement_service.Models;
using engagement_service.Services;

namespace engagement_service.Controllers
{
    /// <summary>
    /// REST: /api/feedbacks. Swagger gom theo vai trò: Công khai, Khách hàng, Nhân viên &amp; Quản trị, Chỉ Admin.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly EngagementDbContext _db;
        private readonly IPurchaseEligibilityService _purchaseEligibility;

        public FeedbacksController(EngagementDbContext db, IPurchaseEligibilityService purchaseEligibility)
        {
            _db = db;
            _purchaseEligibility = purchaseEligibility;
        }

        /// <summary>GET /api/feedbacks — Đánh giá theo sản phẩm (public).</summary>
        [Tags("Đánh giá — 01 Công khai (không cần JWT)")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProduct(
            [FromQuery] int productId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? rating = null)
        {
            if (productId <= 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "productId là bắt buộc và phải lớn hơn 0." });
            if (rating.HasValue && (rating.Value < 1 || rating.Value > 5))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "rating phải nằm trong khoảng 1..5." });

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.Feedbacks.AsNoTracking().Where(f => f.ProductId == productId);
            if (rating is >= 1 and <= 5)
                query = query.Where(f => f.Rating == rating);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(f => f.Images)
                .Include(f => f.FeedbackTags)
                .Include(f => f.Replies)
                .ToListAsync();

            var dto = new PagedData<FeedbackDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(new ApiResponse<PagedData<FeedbackDto>> { Success = true, Message = "OK", Data = dto });
        }

        /// <summary>GET /api/feedbacks/manage — Staff/Admin: danh sách lọc (giống trang Staff monolith).</summary>
        [Tags("Đánh giá — 03 Nhân viên & Quản trị (JWT: Staff hoặc Admin)")]
        [HttpGet("manage")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetManage(
            [FromQuery] int? rating = null,
            [FromQuery] int? productId = null,
            [FromQuery] int? customerId = null,
            [FromQuery] bool? hasReply = null)
        {
            var query = _db.Feedbacks.AsNoTracking().AsQueryable();
            if (rating.HasValue && (rating.Value < 1 || rating.Value > 5))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "rating phải nằm trong khoảng 1..5." });

            if (rating is >= 1 and <= 5)
                query = query.Where(f => f.Rating == rating);
            if (productId is > 0)
                query = query.Where(f => f.ProductId == productId.Value);
            if (customerId is > 0)
                query = query.Where(f => f.CustomerId == customerId.Value);
            if (hasReply == true)
                query = query.Where(f => f.Replies.Any());
            if (hasReply == false)
                query = query.Where(f => !f.Replies.Any());

            var list = await query
                .OrderByDescending(f => f.Id)
                .Include(f => f.Images)
                .Include(f => f.FeedbackTags)
                .Include(f => f.Replies)
                .ToListAsync();

            return Ok(new ApiResponse<List<FeedbackDto>>
            {
                Success = true,
                Data = list.Select(MapToDto).ToList()
            });
        }

        /// <summary>GET /api/feedbacks/{id}</summary>
        [Tags("Đánh giá — 01 Công khai (không cần JWT)")]
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _db.Feedbacks.AsNoTracking()
                .Include(f => f.Images)
                .Include(f => f.FeedbackTags)
                .Include(f => f.Replies)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (entity == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đánh giá." });

            return Ok(new ApiResponse<FeedbackDto> { Success = true, Data = MapToDto(entity) });
        }

        /// <summary>POST /api/feedbacks — Customer (hoặc Admin hộ khách).</summary>
        [Tags("Đánh giá — 02 Khách hàng (JWT: Customer; Admin có thể gửi hộ với CustomerId)")]
        [HttpPost]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            int customerId;
            if (User.IsInRole("Admin"))
            {
                if (request.CustomerId <= 0)
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Admin cần chỉ định CustomerId hợp lệ." });
                customerId = request.CustomerId;
            }
            else
            {
                var cid = GetCustomerIdFromClaims();
                if (!cid.HasValue)
                    return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token thiếu customer_id / CustomerId." });
                customerId = cid.Value;
            }

            var canPurchase = await _purchaseEligibility.HasDeliveredProductInOrderAsync(
                customerId, request.OrderId, request.ProductId, cancellationToken);
            if (!canPurchase)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Chỉ được đánh giá sản phẩm đã mua và giao thành công trong đơn hàng."
                });

            var already = await _db.Feedbacks.AnyAsync(
                f => f.OrderId == request.OrderId && f.ProductId == request.ProductId && f.CustomerId == customerId,
                cancellationToken);
            if (already)
                return Conflict(new ApiResponse<object> { Success = false, Message = "Đã đánh giá sản phẩm này trong đơn hàng." });

            var now = DateTime.UtcNow;
            var entity = new Feedback
            {
                Rating = request.Rating,
                Comment = request.Comment,
                ProductId = request.ProductId,
                CustomerId = customerId,
                OrderId = request.OrderId,
                CreatedAt = now,
                UpdatedAt = now
            };

            foreach (var url in request.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)).Take(5))
                entity.Images.Add(new FeedbackImage { ImageUrl = url.Trim() });

            foreach (var tagId in request.TagsIds.Distinct())
                entity.FeedbackTags.Add(new FeedbackReviewTag { ReviewTagId = tagId });

            _db.Feedbacks.Add(entity);
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                return Conflict(new ApiResponse<object> { Success = false, Message = "Đã đánh giá sản phẩm này trong đơn hàng." });
            }

            await _db.Entry(entity).Collection(e => e.Images).LoadAsync(cancellationToken);
            await _db.Entry(entity).Collection(e => e.FeedbackTags).LoadAsync(cancellationToken);
            await _db.Entry(entity).Collection(e => e.Replies).LoadAsync(cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id },
                new ApiResponse<FeedbackDto> { Success = true, Message = "Đã gửi đánh giá.", Data = MapToDto(entity) });
        }

        /// <summary>PUT /api/feedbacks/{id} — Chủ sở hữu hoặc Admin.</summary>
        [Tags("Đánh giá — 02 Khách hàng (JWT: Customer; Admin có thể sửa hộ)")]
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFeedbackRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = await _db.Feedbacks
                .Include(f => f.Images)
                .Include(f => f.FeedbackTags)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

            if (entity == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đánh giá." });

            if (!User.IsInRole("Admin"))
            {
                var cid = GetCustomerIdFromClaims();
                if (!cid.HasValue || cid.Value != entity.CustomerId)
                    return Forbid();
            }

            entity.Rating = request.Rating;
            entity.Comment = request.Comment;
            entity.UpdatedAt = DateTime.UtcNow;

            entity.Images.Clear();
            foreach (var url in request.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)).Take(5))
                entity.Images.Add(new FeedbackImage { ImageUrl = url.Trim() });

            entity.FeedbackTags.Clear();
            foreach (var tagId in request.TagsIds.Distinct())
                entity.FeedbackTags.Add(new FeedbackReviewTag { ReviewTagId = tagId });

            await _db.SaveChangesAsync(cancellationToken);

            await _db.Entry(entity).Collection(e => e.Replies).LoadAsync(cancellationToken);
            return Ok(new ApiResponse<FeedbackDto> { Success = true, Message = "Đã cập nhật đánh giá.", Data = MapToDto(entity) });
        }

        /// <summary>DELETE /api/feedbacks/{id} — Admin.</summary>
        [Tags("Đánh giá — 04 Chỉ Admin (JWT: Admin)")]
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await _db.Feedbacks.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
            if (entity == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đánh giá." });

            _db.Feedbacks.Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return Ok(new ApiResponse<object> { Success = true, Message = "Đã xóa đánh giá." });
        }

        /// <summary>POST /api/feedbacks/{id}/replies — Staff/Admin (monolith ReplyFeedbackAsync).</summary>
        [Tags("Đánh giá — 03 Nhân viên & Quản trị (JWT: Staff hoặc Admin)")]
        [HttpPost("{id:int}/replies")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Reply(int id, [FromBody] ReplyFeedbackRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var staffId = GetStaffOrAdminActorId();
            if (!staffId.HasValue)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Token thiếu StaffId/staff_id hoặc AdminId (cho Admin)." });

            var feedback = await _db.Feedbacks.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
            if (feedback == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đánh giá." });

            var reply = new FeedbackReply
            {
                FeedbackId = id,
                StaffId = staffId.Value,
                Reply = request.Reply.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            _db.FeedbackReplies.Add(reply);
            feedback.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new ApiResponse<FeedbackReplyDto>
            {
                Success = true,
                Message = "Đã gửi phản hồi.",
                Data = new FeedbackReplyDto
                {
                    Id = reply.Id,
                    Reply = reply.Reply,
                    StaffId = reply.StaffId,
                    CreatedAt = reply.CreatedAt,
                    UpdatedAt = reply.UpdatedAt
                }
            });
        }

        private int? GetCustomerIdFromClaims()
        {
            var v = User.FindFirst("customer_id")?.Value
                ?? User.FindFirst("CustomerId")?.Value;
            return int.TryParse(v, out var id) ? id : null;
        }

        private int? GetStaffIdFromClaims()
        {
            var v = User.FindFirst("StaffId")?.Value
                ?? User.FindFirst("staff_id")?.Value;
            if (int.TryParse(v, out var id))
                return id;
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id2) ? id2 : null;
        }

        /// <summary>Staff dùng StaffId; Admin có thể dùng claim AdminId làm actor khi không có StaffId.</summary>
        private int? GetStaffOrAdminActorId()
        {
            var staff = GetStaffIdFromClaims();
            if (staff.HasValue)
                return staff;
            if (User.IsInRole("Admin"))
            {
                var v = User.FindFirst("AdminId")?.Value ?? User.FindFirst("admin_id")?.Value;
                if (int.TryParse(v, out var adminId))
                    return adminId;
            }
            return null;
        }

        private static FeedbackDto MapToDto(Feedback f) => new()
        {
            Id = f.Id,
            Rating = f.Rating,
            Comment = f.Comment,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt,
            ProductId = f.ProductId,
            CustomerId = f.CustomerId,
            OrderId = f.OrderId,
            Images = f.Images.Select(i => new FeedbackImageDto { Id = i.Id, ImageUrl = i.ImageUrl }).ToList(),
            Replies = f.Replies.Select(r => new FeedbackReplyDto
            {
                Id = r.Id,
                Reply = r.Reply,
                StaffId = r.StaffId,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList(),
            Tags = f.FeedbackTags.Select(t => new FeedbackTagDto { ReviewTagId = t.ReviewTagId }).ToList()
        };

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
            => ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
    }
}
