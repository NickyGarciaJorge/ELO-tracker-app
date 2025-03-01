using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BierpongProjectWebApi.Domain.Entities;
using BierpongProjectWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BierpongProjectWebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username), // Standard claim for "sub" (subject)
                new Claim(ClaimTypes.Name, user.Username), // Short claim for Name
                new Claim(ClaimTypes.Role, _userService.GetUserRole(user.Username)) // Short claim for Role
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            if (string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Password))
                return BadRequest("Invalid username or password");

            bool isValid = _userService.ValidateUser(credentials.Username, credentials.Password);
            if (!isValid)
                return Unauthorized("Invalid credentials");

            var user = _userService.GetUser(credentials.Username);
            var token = GenerateJwtToken(user);

            return Ok(new { Token = "Bearer " + token });
        }
    }
}
