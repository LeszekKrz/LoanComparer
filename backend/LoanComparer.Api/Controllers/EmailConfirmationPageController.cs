using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api/confirm-email-page")]
    public class EmailConfirmationPageController : ControllerBase
    {
        private readonly UserService _userService;

        public EmailConfirmationPageController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string email, [FromQuery] string token)
        {
            await _userService.ConfirmEmailAsync(email, token);
            return Ok();
        }
    }
}
