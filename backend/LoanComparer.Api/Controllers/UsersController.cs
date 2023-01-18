using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
    }
}
