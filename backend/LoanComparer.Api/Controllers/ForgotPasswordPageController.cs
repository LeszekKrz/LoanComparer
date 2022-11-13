using LoanComparer.Application.DTO;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api/forgot-password-page")]
    public class ForgotPasswordPageController : ControllerBase
    {
        private readonly UserService _userService;

        public ForgotPasswordPageController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordDTO forgotPassword, CancellationToken cancellationToken)
        {
            ErrorResponseDTO? errorResponse = await _userService.ForgotPasswordAsync(forgotPassword, cancellationToken);
            return errorResponse == null
                ? Ok()
                : BadRequest(errorResponse);
        }
    }
}
