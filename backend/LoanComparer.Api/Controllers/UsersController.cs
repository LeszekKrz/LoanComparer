using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services;
using LoanComparer.Application.Services.Inquiries;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api/user")]
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

        [HttpGet("info")]
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
