using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api/login-page")]
    public class LoginPageController : ControllerBase
    {
        private readonly UserService _userService;

        public LoginPageController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] UserForAuthenticationDTO userForAuthentication)
        {
            AuthenticationResponseDTO authenticationResponse = await _userService.LoginUser(userForAuthentication);
            return authenticationResponse.Token == null
                ? Unauthorized(authenticationResponse)
                : Ok(authenticationResponse);
        }
    }
}
