using article_service.Models;
using article_service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace article_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var feedbacks = await _feedbackService.GetByProductIdAsync(productId);
            return Ok(feedbacks);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var feedbacks = await _feedbackService.GetAllAsync();
            return Ok(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Feedback request)
        {
            if (string.IsNullOrEmpty(request.Comment) || request.Rating < 1 || request.Rating > 5)
                return BadRequest("Invalid feedback data");

            request.CreatedAt = DateTime.UtcNow;
            var created = await _feedbackService.CreateAsync(request);
            return Ok(created);
        }

        [HttpPost("{id}/reply")]
        public async Task<IActionResult> Reply(string id, [FromBody] ReplyRequest request)
        {
            if (string.IsNullOrEmpty(request.ReplyMessage))
                return BadRequest("Reply message is required");

            var result = await _feedbackService.ReplyAsync(id, request.ReplyMessage, request.StaffId);
            if (!result) return NotFound();

            return Ok(new { Success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _feedbackService.DeleteAsync(id);
            if (!result) return NotFound();

            return Ok(new { Success = true });
        }
    }

    public class ReplyRequest
    {
        public string ReplyMessage { get; set; } = string.Empty;
        public Guid StaffId { get; set; }
    }
}
