using BierpongProjectWebApi.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BierpongBackEndApiTest.ControllerTests
{
    public class UserProfileControllerTest
    {
        private readonly Mock<UserProfileService> _mockUserProfileService;
        private readonly UserProfileController _controller;

        public UserProfileControllerTest()
        {
            _mockUserProfileService = new Mock<UserProfileService>();
            _controller = new UserProfileController(_mockUserProfileService.Object);
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
        public async Task UpdateUserProfile_Should_Return_UpdatedProfile_If_Successful()
        {
            var userId = Guid.NewGuid();
            var updatedProfile = new UserProfile { Name = "Updated Name", Bio = "New Bio", ProfilePictureUrl = "http://example.com/newpic.jpg" };
            _mockUserProfileService.Setup(s => s.UpdateUserProfileAsync(userId, updatedProfile.Name, updatedProfile.Bio, updatedProfile.ProfilePictureUrl)).ReturnsAsync(updatedProfile);

            var result = await _controller.UpdateUserProfile(userId, updatedProfile);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedProfile, okResult.Value);
        }

        [Fact]
        public async Task UpdateUserProfile_Should_Return_NotFound_If_User_Not_Exists()
        {
            var userId = Guid.NewGuid();
            var updatedProfile = new UserProfile();
            _mockUserProfileService.Setup(s => s.UpdateUserProfileAsync(userId, updatedProfile.Name, updatedProfile.Bio, updatedProfile.ProfilePictureUrl)).ReturnsAsync((UserProfile)null);

            var result = await _controller.UpdateUserProfile(userId, updatedProfile);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User profile not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task AddFriend_Should_Return_Ok_If_Successful()
        {
            var userId = Guid.NewGuid();
            var friendUserId = Guid.NewGuid();
            _mockUserProfileService.Setup(s => s.AddFriendAsync(userId, friendUserId)).ReturnsAsync(true);

            var result = await _controller.AddFriend(userId, friendUserId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Friend request sent.", okResult.Value);
        }

        [Fact]
        public async Task AddFriend_Should_Return_BadRequest_If_Friendship_Already_Exists()
        {
            var userId = Guid.NewGuid();
            var friendUserId = Guid.NewGuid();
            _mockUserProfileService.Setup(s => s.AddFriendAsync(userId, friendUserId)).ReturnsAsync(false);

            var result = await _controller.AddFriend(userId, friendUserId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Friendship already exists or invalid request.", badRequestResult.Value);
        }

        [Fact]
        public async Task AcceptFriendRequest_Should_Return_Ok_If_Successful()
        {
            var userId = Guid.NewGuid();
            var friendUserId = Guid.NewGuid();
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
            _mockUserProfileService.Setup(s => s.AcceptFriendRequestAsync(userId, friendUserId)).ReturnsAsync(false);

            var result = await _controller.AcceptFriendRequest(userId, friendUserId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Friend request not found or already accepted.", badRequestResult.Value);
        }

        [Fact]
        public async Task RejectOrBlockFriendRequest_Should_Return_Ok_If_Successful()
        {
            var userId = Guid.NewGuid();
            var friendUserId = Guid.NewGuid();
            _mockUserProfileService.Setup(s => s.RejectOrBlockFriendRequestAsync(userId, friendUserId, false)).ReturnsAsync(true);

            var result = await _controller.RejectOrBlockFriendRequest(userId, friendUserId, false);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Friend request rejected.", okResult.Value);
        }

        [Fact]
        public async Task RejectOrBlockFriendRequest_Should_Return_BadRequest_If_Request_Not_Found()
        {
            var userId = Guid.NewGuid();
            var friendUserId = Guid.NewGuid();
            _mockUserProfileService.Setup(s => s.RejectOrBlockFriendRequestAsync(userId, friendUserId, false)).ReturnsAsync(false);

            var result = await _controller.RejectOrBlockFriendRequest(userId, friendUserId, false);

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
