using LoanComparer.Application.DTO;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api/reset-password-page")]
    public class ResetPasswordController : ControllerBase
    {
        private readonly UserService _userService;

        public ResetPasswordController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword, CancellationToken cancellationToken)
        {
            IEnumerable<ErrorResponseDTO>? resetPasswordResponse = await _userService.ResetPasswordAsync(resetPassword, cancellationToken);
            return resetPasswordResponse == null
                ? Ok()
                : BadRequest(resetPasswordResponse);
        }
    }
}
