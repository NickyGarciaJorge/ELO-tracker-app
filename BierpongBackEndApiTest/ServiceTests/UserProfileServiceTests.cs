using BierpongProjectWebApi.Data;
using BierpongProjectWebApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BierpongBackEndApiTest.ServiceTests
{
    public class UserProfileServiceTests
    {
        private readonly CustomDbContext _dbContext;
        private readonly UserProfileService _userProfileService;

        public UserProfileServiceTests()
        {
            var options = new DbContextOptionsBuilder<CustomDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new CustomDbContext(options);
            _dbContext.UserProfiles.Add(new UserProfile
            {
                UserId = Guid.NewGuid(),
                Name = "Test User",
                Bio = "Test Bio",
                ProfilePictureUrl = "http://example.com/pic.jpg"
            });
            _dbContext.SaveChanges();

            _userProfileService = new UserProfileService(_dbContext);
        }

        [Fact]
        public async Task GetUserProfileAsync_Should_Return_UserProfile()
        {
            var user = _dbContext.UserProfiles.First();
            var result = await _userProfileService.GetUserProfileAsync(user.UserId);

            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_Should_Update_Profile()
        {
            var user = _dbContext.UserProfiles.First();
            var updatedProfile = await _userProfileService.UpdateUserProfileAsync(user.UserId, "Updated Name", "Updated Bio", "http://newpic.com");

            Assert.NotNull(updatedProfile);
            Assert.Equal("Updated Name", updatedProfile.Name);
            Assert.Equal("Updated Bio", updatedProfile.Bio);
            Assert.Equal("http://newpic.com", updatedProfile.ProfilePictureUrl);
        }

        [Fact]
        public async Task AddFriendAsync_Should_Add_Friend()
        {
            var user1 = new UserProfile { UserId = Guid.NewGuid(), Name = "User1" };
            var user2 = new UserProfile { UserId = Guid.NewGuid(), Name = "User2" };
            _dbContext.UserProfiles.AddRange(user1, user2);
            _dbContext.SaveChanges();

            var result = await _userProfileService.AddFriendAsync(user1.UserId, user2.UserId);

            Assert.True(result);
        }

        [Fact]
        public async Task AddFriendAsync_Should_Return_False_If_Friendship_Already_Exists()
        {
            var user1 = new UserProfile { UserId = Guid.NewGuid(), Name = "User1" };
            var user2 = new UserProfile { UserId = Guid.NewGuid(), Name = "User2" };
            _dbContext.UserProfiles.AddRange(user1, user2);
            _dbContext.Friendships.Add(new Friendship { UserId = user1.UserId, FriendUserId = user2.UserId, Status = FriendshipStatus.Accepted });
            _dbContext.SaveChanges();

            var result = await _userProfileService.AddFriendAsync(user1.UserId, user2.UserId);

            Assert.False(result);
        }

        [Fact]
        public async Task AcceptFriendRequestAsync_Should_Accept_Request()
        {
            var user1 = new UserProfile { UserId = Guid.NewGuid(), Name = "User1" };
            var user2 = new UserProfile { UserId = Guid.NewGuid(), Name = "User2" };
            _dbContext.UserProfiles.AddRange(user1, user2);
            _dbContext.Friendships.Add(new Friendship { UserId = user1.UserId, FriendUserId = user2.UserId, Status = FriendshipStatus.Pending });
            _dbContext.SaveChanges();

            var result = await _userProfileService.AcceptFriendRequestAsync(user2.UserId, user1.UserId);

            Assert.True(result);
        }

        [Fact]
        public async Task AcceptFriendRequestAsync_Should_Return_False_If_No_Pending_Request()
        {
            var user1 = new UserProfile { UserId = Guid.NewGuid(), Name = "User1" };
            var user2 = new UserProfile { UserId = Guid.NewGuid(), Name = "User2" };
            _dbContext.UserProfiles.AddRange(user1, user2);
            _dbContext.SaveChanges();

            var result = await _userProfileService.AcceptFriendRequestAsync(user2.UserId, user1.UserId);

            Assert.False(result);
        }

        [Fact]
        public async Task RejectOrBlockFriendRequestAsync_Should_Reject_Request()
        {
            var user1 = new UserProfile { UserId = Guid.NewGuid(), Name = "User1" };
            var user2 = new UserProfile { UserId = Guid.NewGuid(), Name = "User2" };
            _dbContext.UserProfiles.AddRange(user1, user2);
            _dbContext.Friendships.Add(new Friendship { UserId = user1.UserId, FriendUserId = user2.UserId, Status = FriendshipStatus.Pending });
            _dbContext.SaveChanges();

            var result = await _userProfileService.RejectOrBlockFriendRequestAsync(user2.UserId, user1.UserId, false);

            Assert.True(result);
        }

        [Fact]
        public async Task GetUserFriendsAsync_Should_Return_Friends()
        {
            var user1 = new UserProfile { UserId = Guid.NewGuid(), Name = "User1" };
            var user2 = new UserProfile { UserId = Guid.NewGuid(), Name = "User2" };
            _dbContext.UserProfiles.AddRange(user1, user2);
            _dbContext.Friendships.Add(new Friendship { UserId = user1.UserId, FriendUserId = user2.UserId, Status = FriendshipStatus.Accepted });
            _dbContext.SaveChanges();

            var friends = await _userProfileService.GetUserFriendsAsync(user1.UserId);

            Assert.Single(friends);
            Assert.Equal(user2.UserId, friends[0].UserId);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
