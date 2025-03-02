using System;
using System.Collections.Generic;
using System.Linq;
using BierpongProjectWebApi.Data;
using BierpongProjectWebApi.Domain.Entities;
using BierpongProjectWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BierpongBackEndApiTest
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<CustomDbContext> _mockDbContext;
        private readonly List<User> _users;

        public UserServiceTests()
        {
            // Mock DbSet<User>
            var mockDbSet = new Mock<DbSet<User>>();

            // Convert list to queryable and attach to mock
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Username = "testuser", Name = "Test User", Email = "test@example.com", Password = "password123", Role = UserRole.User }
            }.AsQueryable();

            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Mock DbContextOptions<CustomDbContext> to pass into the CustomDbContext constructor
            var mockDbContextOptions = new Mock<DbContextOptions<CustomDbContext>>();

            // Create the mock CustomDbContext without calling the constructor
            _mockDbContext = new Mock<CustomDbContext>(mockDbContextOptions.Object) { CallBase = true };  // CallBase allows invoking base methods (but still mocking)

            // Set up the DbSet<User> to be returned when accessing the Users property
            _mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            // Pass _mockDbContext.Object to UserService
            _userService = new UserService(_mockDbContext.Object);
        }

        [Fact]
        public void AddUser_Should_Add_User_To_Database()
        {
            var newUser = new User { Username = "newuser", Name = "New User", Email = "new@example.com", Password = "securepassword", Role = UserRole.User };
            _userService.AddUser(newUser);
            _mockDbContext.Verify(db => db.Users.Add(It.IsAny<User>()), Times.Once);
            _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GetUser_Should_Return_Correct_User()
        {
            var result = _userService.GetUser("testuser");
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public void GetUserRole_Should_Return_Correct_Role()
        {
            var result = _userService.GetUserRole("testuser");
            Assert.Equal(UserRole.User.ToString(), result);
        }

        [Fact]
        public void GetUsers_Should_Return_All_Users()
        {
            var result = _userService.GetUsers();
            Assert.Single(result);
        }

        [Fact]
        public void ValidateUser_Should_Return_True_If_Valid()
        {
            var result = _userService.ValidateUser("testuser", "password123");
            Assert.True(result);
        }

        [Fact]
        public void ValidateUser_Should_Return_False_If_Invalid()
        {
            var result = _userService.ValidateUser("testuser", "wrongpassword");
            Assert.False(result);
        }

        [Fact]
        public void UserExists_Should_Return_True_If_User_Exists()
        {
            var result = _userService.UserExists("testuser");
            Assert.True(result);
        }

        [Fact]
        public void UserExists_Should_Return_False_If_User_Does_Not_Exist()
        {
            var result = _userService.UserExists("nonexistentuser");
            Assert.False(result);
        }

        [Fact]
        public void UpdateUser_Should_Update_User_Details()
        {
            _userService.UpdateUser("testuser", "Updated Name", "updated@example.com", "newpassword");
            var updatedUser = _userService.GetUser("testuser");
            Assert.Equal("Updated Name", updatedUser.Name);
            Assert.Equal("updated@example.com", updatedUser.Email);
        }

        [Fact]
        public void DeleteUser_Should_Remove_User()
        {
            _userService.DeleteUser("testuser");
            var result = _userService.GetUser("testuser");
            Assert.Null(result);
        }
    }
}
