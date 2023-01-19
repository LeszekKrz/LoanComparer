using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("users-count")]
        public async Task<ActionResult<UsersCountDTO>> GetUserCountAsync()
        {
            UsersCountDTO usersCount = await _userService.GetUserCountAsync();
            return Ok(usersCount);
        }

        [HttpGet("user/info")]
        public async Task<ActionResult<UserInfoDTO>> GetUserInfoAsync()
        {
            var username = GetUsername();
            if (username is null)
            {
                return Unauthorized();
            }

            UserInfoDTO userInfo = await _userService.GetUserInfoAsync(username);
            return Ok(userInfo);
        }

        private string? GetUsername()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }
    }
}
