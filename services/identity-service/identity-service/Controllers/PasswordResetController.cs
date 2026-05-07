using Microsoft.AspNetCore.Mvc;
using identity_service.DTOs;
using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;
using identity_service.Services.Interfaces;

namespace identity_service.Controllers
{
    [Route("api/auth/password")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordResetService _passwordResetService;

        public PasswordResetController(IPasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _passwordResetService.ForgotPasswordAsync(request);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Message });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = result.Message });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _passwordResetService.VerifyOtpAsync(request);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Message });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = result.Message });
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _passwordResetService.ResetPasswordAsync(request);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = result.Message });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = result.Message });
        }
    }
}
