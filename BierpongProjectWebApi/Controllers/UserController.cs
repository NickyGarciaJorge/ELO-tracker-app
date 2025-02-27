using BierpongProjectWebApi.Domain.Entities;
using BierpongProjectWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BierpongProjectWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
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

        [HttpGet("{username}")]
        public IActionResult GetUser(string username)
        {
            var user = _userService.GetUser(username);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpPost("validate")]
        public IActionResult ValidateUser([FromBody] UserCredentials credentials)
        {
            if (string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Password))
                return BadRequest("Invalid username or password");

            bool isValid = _userService.ValidateUser(credentials.Username, credentials.Password);
            if (!isValid)
                return Unauthorized("Invalid credentials");

            return Ok("User validated successfully");
        }

        [HttpPut("{username}")]
        public IActionResult UpdateUser(string username, [FromBody] User updatedUser)
        {
            if (updatedUser == null)
                return BadRequest("Invalid user data");

            var existingUser = _userService.GetUser(username);
            if (existingUser == null)
                return NotFound("User not found");

            _userService.UpdateUser(username, updatedUser.Name, updatedUser.Email, updatedUser.Password);
            return NoContent();
        }

        [HttpDelete("{username}")]
        public IActionResult DeleteUser(string username)
        {
            if (!_userService.UserExists(username))
                return NotFound("User not found");

            _userService.DeleteUser(username);
            return NoContent();
        }
    }
}
