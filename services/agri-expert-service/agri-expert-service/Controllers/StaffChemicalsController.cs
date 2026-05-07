using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using agri_expert_service.DTOs.Requests;
using agri_expert_service.DTOs.Responses;
using agri_expert_service.Services.Interfaces;

namespace agri_expert_service.Controllers
{
    [Route("api/staff/chemicals")]
    [ApiController]
    [Authorize(Roles = "Staff,Admin")]
    public class StaffChemicalsController : ControllerBase
    {
        private readonly IChemicalService _chemicalService;

        public StaffChemicalsController(IChemicalService chemicalService)
        {
            _chemicalService = chemicalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ChemicalSearchRequest request)
        {
            if (request.Page < 1)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Số trang phải lớn hơn 0." });
            if (request.PageSize < 1 || request.PageSize > 100)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "PageSize phải trong khoảng 1-100." });

            // Staff chỉ xem hóa chất đang active
            request.IsActive = true;

            var result = await _chemicalService.GetPagedAsync(request);
            return Ok(new ApiResponse<PagedResult<ChemicalDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data    = result.Data
            });
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            var result = await _chemicalService.GetGroupsAsync();
            return Ok(new ApiResponse<List<string>>
            {
                Success = result.Success,
                Message = result.Message,
                Data    = result.Data
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _chemicalService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });

            if (result.Data?.IsActive == false)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy hóa chất." });

            return Ok(new ApiResponse<ChemicalDto> { Success = true, Message = result.Message, Data = result.Data });
        }
    }
}
