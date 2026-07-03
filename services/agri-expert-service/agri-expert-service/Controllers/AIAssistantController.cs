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
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { Success = false, ErrorMessage = "Message cannot be empty." });
            }

            var result = await _deepSeekService.ChatAsync(request.Message);
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        [HttpPost("analyze-disease")]
        public async Task<IActionResult> AnalyzeDisease([FromBody] AiDiseaseAnalysisRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Base64Image))
            {
                return BadRequest(new { Success = false, ErrorMessage = "Image is required." });
            }

            var result = await _deepSeekService.AnalyzeDiseaseAsync(request.Base64Image);
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }
    }
}
