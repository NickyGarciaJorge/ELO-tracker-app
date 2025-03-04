using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BierpongProjectWebApi.Models.Entities;
using BierpongProjectWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BierpongProjectWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username))
                return BadRequest("Invalid user data");

            if (_userService.UserExists(user.Username))
                return Conflict("User already exists");

            _userService.AddUser(user);
            return CreatedAtAction(nameof(GetUser), new { username = user.Username }, user);
        }

        [HttpGet("allUsers")]
        [Authorize]
        public IActionResult GetAllUsers(string username)
        {
            var loggedInUser = User.Identity.Name;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != UserRole.Administrator.ToString())
                return Forbid();

            var users = _userService.GetUsers();
            if (users == null && users.Count > 0)
                return NotFound("User not found");

            return Ok(users);
        }

        [HttpGet("{username}")]
        [Authorize]
        public IActionResult GetUser(string username)
        {
            var loggedInUser = User.Identity.Name;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (loggedInUser != username && userRole != UserRole.Administrator.ToString())
                return Forbid();

            var user = _userService.GetUser(username);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpPut("{username}")]
        [Authorize]
        public IActionResult UpdateUser(string username, [FromBody] User updatedUser)
        {
            if (updatedUser == null)
                return BadRequest("Invalid user data");

            var loggedInUser = User.Identity.Name;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (loggedInUser != username && userRole != UserRole.Administrator.ToString())
                return Forbid();

            var existingUser = _userService.GetUser(username);
            if (existingUser == null)
                return NotFound("User not found");

            _userService.UpdateUser(username, updatedUser.Name, updatedUser.Email, updatedUser.Password);
            return NoContent();
        }

        [HttpDelete("{username}")]
        [Authorize]
        public IActionResult DeleteUser(string username)
        {
            var loggedInUser = User.Identity.Name;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (loggedInUser != username && userRole != UserRole.Administrator.ToString())
                return Forbid();

            if (!_userService.UserExists(username))
                return NotFound("User not found");

            _userService.DeleteUser(username);
            return NoContent();
        }
    }
}
