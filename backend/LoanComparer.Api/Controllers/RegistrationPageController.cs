using LoanComparer.Application.DTO;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api/registration-page")]

    public class RegistrationPageController : ControllerBase
    {
        private readonly UserService _userService;

        public RegistrationPageController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserForRegistrationDTO userForRegistration, CancellationToken cancellationToken)
        {
            IEnumerable<ErrorResponseDTO>? registrationResponse = await _userService.RegisterUserAsync(userForRegistration, cancellationToken);

            return registrationResponse == null
                ? StatusCode((int)HttpStatusCode.Created)
                : BadRequest(registrationResponse);
        }
    }
}
