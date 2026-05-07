using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using identity_service.DTOs;
using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;
using identity_service.Services.Interfaces;

namespace identity_service.Controllers
{
    [Route("api/staff/customers")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public class StaffCustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public StaffCustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CustomerSearchRequest request)
        {
            if (request.Page < 1)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Số trang phải lớn hơn 0." });
            if (request.PageSize < 1 || request.PageSize > 100)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "PageSize phải nằm trong khoảng từ 1 đến 100." });

            var result = await _customerService.GetPagedAsync(request);

            return Ok(new ApiResponse<PagedResult<CustomerDto>>
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Data
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _customerService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });

            return Ok(new ApiResponse<CustomerDto> { Success = true, Message = result.Message, Data = result.Data });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            var result = await _customerService.UpdateAsync(id, request);

            if (!result.Success)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });
            }

            return Ok(new ApiResponse<CustomerDto> { Success = true, Message = result.Message, Data = result.Data });
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateCustomerStatusRequest request)
        {
            var result = await _customerService.UpdateStatusAsync(id, request);

            if (!result.Success)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = result.Message });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = result.Message });
        }
    }
}
