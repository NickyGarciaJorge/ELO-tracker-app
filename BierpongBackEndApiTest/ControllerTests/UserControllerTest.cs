using BierpongProjectWebApi.Controllers;
using BierpongProjectWebApi.Models.Entities;
using BierpongProjectWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BierpongBackEndApiTest.ControllerTests
{
    public class UserControllerTest
    {
        private readonly Mock<UserService> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _mockUserService = new Mock<UserService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new UserController(_mockUserService.Object, _mockConfiguration.Object);
        }

        // Helper method to mock authenticated user context
        private void SetUserContext(string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public void AddUser_Should_Return_Created()
        {
            var newUser = new User { Username = "newuser", Name = "New User", Email = "new@example.com", Password = "password123", Role = UserRole.User };

            _mockUserService.Setup(s => s.UserExists(newUser.Username)).Returns(false);
            _mockUserService.Setup(s => s.AddUser(newUser));

            var result = _controller.AddUser(newUser);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetUser", createdResult.ActionName);
            Assert.Equal(newUser.Username, ((User)createdResult.Value).Username);
        }

        [Fact]
        public void AddUser_Should_Return_Conflict_If_User_Exists()
        {
            var newUser = new User { Username = "existingUser" };

            _mockUserService.Setup(s => s.UserExists(newUser.Username)).Returns(true);

            var result = _controller.AddUser(newUser);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("User already exists", conflictResult.Value);
        }

        [Fact]
        public void GetUser_Should_Return_User_If_Authorized()
        {
            var user = new User { Username = "testuser", Name = "Test User" };
            _mockUserService.Setup(s => s.GetUser("testuser")).Returns(user);

            SetUserContext("testuser", UserRole.User.ToString());

            var result = _controller.GetUser("testuser");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public void GetUser_Should_Return_Forbidden_If_Unauthorized()
        {
            SetUserContext("otheruser", UserRole.User.ToString());

            var result = _controller.GetUser("testuser");

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void GetUser_Should_Return_NotFound_If_User_Not_Exists()
        {
            _mockUserService.Setup(s => s.GetUser("nonexistent")).Returns((User)null);

            SetUserContext("admin", UserRole.Administrator.ToString());

            var result = _controller.GetUser("nonexistent");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public void GetAllUsers_Should_Return_Users_If_Admin()
        {
            var users = new List<User>
            {
                new User { Username = "user1" },
                new User { Username = "user2" }
            };

            _mockUserService.Setup(s => s.GetUsers()).Returns(users);

            SetUserContext("admin", UserRole.Administrator.ToString());

            var result = _controller.GetAllUsers("admin");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public void GetAllUsers_Should_Return_Forbidden_If_Not_Admin()
        {
            SetUserContext("testuser", UserRole.User.ToString());

            var result = _controller.GetAllUsers("testuser");

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void UpdateUser_Should_Return_NoContent_If_Successful()
        {
            var updatedUser = new User { Name = "Updated Name", Email = "updated@example.com", Password = "newpassword" };

            _mockUserService.Setup(s => s.GetUser("testuser")).Returns(new User { Username = "testuser" });
            _mockUserService.Setup(s => s.UpdateUser("testuser", updatedUser.Name, updatedUser.Email, updatedUser.Password));

            SetUserContext("testuser", UserRole.User.ToString());

            var result = _controller.UpdateUser("testuser", updatedUser);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateUser_Should_Return_NotFound_If_User_Not_Exists()
        {
            _mockUserService.Setup(s => s.GetUser("nonexistent")).Returns((User)null);

            SetUserContext("admin", UserRole.Administrator.ToString());

            var result = _controller.UpdateUser("nonexistent", new User());

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public void DeleteUser_Should_Return_NoContent_If_Successful()
        {
            _mockUserService.Setup(s => s.UserExists("testuser")).Returns(true);
            _mockUserService.Setup(s => s.DeleteUser("testuser"));

            SetUserContext("testuser", UserRole.User.ToString());

            var result = _controller.DeleteUser("testuser");

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteUser_Should_Return_NotFound_If_User_Not_Exists()
        {
            _mockUserService.Setup(s => s.UserExists("nonexistent")).Returns(false);

            SetUserContext("admin", UserRole.Administrator.ToString());

            var result = _controller.DeleteUser("nonexistent");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public void DeleteUser_Should_Return_Forbidden_If_Unauthorized()
        {
            SetUserContext("otheruser", UserRole.User.ToString());

            var result = _controller.DeleteUser("testuser");

            Assert.IsType<ForbidResult>(result);
        }
    }
}
