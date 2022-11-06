using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api/user")]

    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("registatrion")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDTO userForRegistration, CancellationToken cancellationToken)
        {
            RegistrationResponseDTO registrationResponse = await _userService.RegisterUser(userForRegistration, cancellationToken);

            return registrationResponse.Errors == null
                ? StatusCode((int)HttpStatusCode.Created)
                : BadRequest(registrationResponse);
        }
    }
}
