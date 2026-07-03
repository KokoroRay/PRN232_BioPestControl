using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using agri_expert_service.DTOs;
using agri_expert_service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace agri_expert_service.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIAssistantController : ControllerBase
    {
        private readonly IDeepSeekService _deepSeekService;

        public AIAssistantController(IDeepSeekService deepSeekService)
        {
            _deepSeekService = deepSeekService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] AiChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message) && (request.Images == null || request.Images.Count == 0))
            {
                return BadRequest(new { Success = false, ErrorMessage = "Message or images must be provided." });
            }

            var result = await _deepSeekService.ChatAsync(request);
            
            // Return 200 OK even if Success is false, so the frontend can gracefully read the ErrorMessage
            return Ok(result);
        }
    }
}
