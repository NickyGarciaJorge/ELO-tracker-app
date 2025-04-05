using BierpongProjectWebApi.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BierpongBackEndApiTest.ControllerTests
{
    public class UserProfileControllerTest
    {
        private readonly Mock<UserProfileService> _mockUserProfileService;
        private readonly UserProfileController _controller;
        private ClaimsPrincipal _user;

        public UserProfileControllerTest()
        {
            _mockUserProfileService = new Mock<UserProfileService>();
            _controller = new UserProfileController(_mockUserProfileService.Object);

            // Mock authenticated user with claims
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()), // Mock User ID
                new Claim(ClaimTypes.Role, "User") // Regular user
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };
        }

        [Fact]
        public async Task GetUserProfile_Should_Return_UserProfile_If_Exists()
        {
            var userId = Guid.NewGuid();
            var userProfile = new UserProfile { UserId = userId, Name = "Test User", Bio = "Hello world", ProfilePictureUrl = "http://example.com/pic.jpg" };
            _mockUserProfileService.Setup(s => s.GetUserProfileAsync(userId)).ReturnsAsync(userProfile);

            var result = await _controller.GetUserProfile(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userProfile, okResult.Value);
        }

        [Fact]
        public async Task GetUserProfile_Should_Return_NotFound_If_Not_Exists()
        {
            var userId = Guid.NewGuid();
            _mockUserProfileService.Setup(s => s.GetUserProfileAsync(userId)).ReturnsAsync((UserProfile)null);

            var result = await _controller.GetUserProfile(userId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User profile not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateUserProfile_Should_Return_Forbidden_If_User_Not_Authorized()
        {
            var userId = Guid.NewGuid();
            var updatedProfile = new UserProfile { Name = "Updated Name", Bio = "New Bio", ProfilePictureUrl = "http://example.com/newpic.jpg" };

            // Simulating a user that is not authorized to update the profile (either not owner or not admin)
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()), // Different user than the profile owner
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };

            var result = await _controller.UpdateUserProfile(userId, updatedProfile);

            var forbidResult = Assert.IsType<ForbidResult>(result); // Should return Forbid if not authorized
        }

        [Fact]
        public async Task UpdateUserProfile_Should_Return_UpdatedProfile_If_Successful()
        {
            var userId = Guid.NewGuid();
            var updatedProfile = new UserProfile { Name = "Updated Name", Bio = "New Bio", ProfilePictureUrl = "http://example.com/newpic.jpg" };

            // Mock the user to be the owner of the profile
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // User is the owner of the profile
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };

            _mockUserProfileService.Setup(s => s.UpdateUserProfileAsync(userId, updatedProfile.Name, updatedProfile.Bio, updatedProfile.ProfilePictureUrl)).ReturnsAsync(updatedProfile);

            var result = await _controller.UpdateUserProfile(userId, updatedProfile);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedProfile, okResult.Value);
        }

        [Fact]
        public async Task AddFriend_Should_Return_Ok_If_Successful()
        {
            var userId = Guid.NewGuid();
            var friendUserId = Guid.NewGuid();

            // Mock the user as the profile owner
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // User is the profile owner
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };

            _mockUserProfileService.Setup(s => s.AddFriendAsync(userId, friendUserId)).ReturnsAsync(true);

            var result = await _controller.AddFriend(userId, friendUserId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Friend request sent.", okResult.Value);
        }

        //[Fact]
        //public async Task AddFriend_Should_Return_BadRequest_If_Friendship_Already_Exists()
        //{
        //    var userId = Guid.NewGuid();
        //    var friendUserId = Guid.NewGuid();

        //    // Mock the user as the profile owner
        //    _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //    new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // User is the profile owner
        //    new Claim(ClaimTypes.Role, "User")
        //    }, "mock"));

        //    _controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext { User = _user }
        //    };

        //    _mockUserProfileService.Setup(s => s.AddFriendAsync(userId, friendUserId)).ReturnsAsync(false);

        //    var result = await _controller.AddFriend(userId, friendUserId);

        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.Equal("Users are already friends.", badRequestResult.Value);
        //}

        [Fact]
        public async Task AcceptFriendRequest_Should_Return_Ok_If_Successful()
        {
            var userId = Guid.NewGuid();
            var friendUserId = Guid.NewGuid();

            // Mock the user as the profile owner
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // User is the profile owner
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };

            _mockUserProfileService.Setup(s => s.AcceptFriendRequestAsync(userId, friendUserId)).ReturnsAsync(true);

            var result = await _controller.AcceptFriendRequest(userId, friendUserId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Friend request accepted.", okResult.Value);
        }

        [Fact]
        public async Task AcceptFriendRequest_Should_Return_BadRequest_If_Request_Not_Found()
        {
            var userId = Guid.NewGuid();
            var friendUserId = Guid.NewGuid();

            // Mock the user as the profile owner
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // User is the profile owner
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };

            _mockUserProfileService.Setup(s => s.AcceptFriendRequestAsync(userId, friendUserId)).ReturnsAsync(false);

            var result = await _controller.AcceptFriendRequest(userId, friendUserId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Friend request not found.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetUserFriends_Should_Return_Ok_With_Friends_List()
        {
            var userId = Guid.NewGuid();
            var friendsList = new List<UserProfile>
            {
                new UserProfile { UserId = Guid.NewGuid(), Name = "Friend1" },
                new UserProfile { UserId = Guid.NewGuid(), Name = "Friend2" }
            };

            _mockUserProfileService.Setup(s => s.GetUserFriendsAsync(userId)).ReturnsAsync(friendsList);

            var result = await _controller.GetUserFriends(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFriends = Assert.IsType<List<UserProfile>>(okResult.Value);
            Assert.Equal(2, returnedFriends.Count);
        }
    }
}
