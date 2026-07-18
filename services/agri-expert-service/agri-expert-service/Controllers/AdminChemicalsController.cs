using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using agri_expert_service.DTOs.Requests;
using agri_expert_service.DTOs.Responses;
using agri_expert_service.Services.Interfaces;

namespace agri_expert_service.Controllers
{
    [Route("api/admin/chemicals")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminChemicalsController : ControllerBase
    {
        private readonly IChemicalService _chemicalService;

        public AdminChemicalsController(IChemicalService chemicalService)
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

            return Ok(new ApiResponse<ChemicalDto>
            {
                Success = true,
                Message = result.Message,
                Data    = result.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChemicalRequest request)
        {
            var result = await _chemicalService.CreateAsync(request);

            return result.StatusCode switch
            {
                201 => StatusCode(201, new ApiResponse<ChemicalDto> { Success = true, Message = result.Message, Data = result.Data }),
                409 => Conflict(new ApiResponse<object>             { Success = false, Message = result.Message }),
                _   => BadRequest(new ApiResponse<object>           { Success = false, Message = result.Message })
            };
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChemicalRequest request)
        {
            var result = await _chemicalService.UpdateAsync(id, request);

            if (!result.Success)
            {
                return result.StatusCode switch
                {
                    404 => NotFound(new ApiResponse<object> { Success = false, Message = result.Message }),
                    409 => Conflict(new ApiResponse<object> { Success = false, Message = result.Message }),
                    _   => BadRequest(new ApiResponse<object>{ Success = false, Message = result.Message })
                };
            }

            return Ok(new ApiResponse<ChemicalDto> { Success = true, Message = result.Message, Data = result.Data });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _chemicalService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });

            return Ok(new ApiResponse<object> { Success = true, Message = result.Message });
        }
    }
}
