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
        public async Task<ActionResult<AuthenticationResponseDTO>> LoginUserAsync([FromBody] UserForAuthenticationDTO userForAuthentication, CancellationToken cancellationToken)
        {
            AuthenticationResponseDTO authenticationResponse = await _userService.LoginUserAsync(userForAuthentication, cancellationToken);
            return Ok(authenticationResponse);
        }

        [HttpPost("login-with-google")]
        public async Task<ActionResult<AuthenticationResponseDTO>> LoginUserWithGoogleAsync([FromBody] UserForGoogleAuthenticationDTO userForGoogleAuthentication)
        {
            AuthenticationResponseDTO authenticationResponse = await _userService.LoginUserWithGoogleAsync(userForGoogleAuthentication);
            return Ok(authenticationResponse);
        }
    }
}
