using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api/reset-password-page")]
    public class ResetPasswordPageController : ControllerBase
    {
        private readonly UserService _userService;

        public ResetPasswordPageController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword, CancellationToken cancellationToken)
        {
            await _userService.ResetPasswordAsync(resetPassword, cancellationToken);
            return Ok();
        }
    }
}
