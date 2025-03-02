using System;
using System.Collections.Generic;
using System.Linq;
using BierpongProjectWebApi.Data;
using BierpongProjectWebApi.Domain.Entities;
using BierpongProjectWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BierpongBackEndApiTest.ServiceTests
{
    public class UserServiceTests : IDisposable
    {
        private readonly CustomDbContext _dbContext;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            // Create in-memory database
            var options = new DbContextOptionsBuilder<CustomDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test run
                .Options;

            _dbContext = new CustomDbContext(options);

            // Seed the database
            _dbContext.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Name = "Test User",
                Email = "test@example.com",
                Password = "password123",
                Role = UserRole.User
            });

            _dbContext.SaveChanges();

            _userService = new UserService(_dbContext);
        }

        [Fact]
        public void AddUser_Should_Add_User_To_Database()
        {
            var newUser = new User
            {
                Username = "newuser",
                Name = "New User",
                Email = "new@example.com",
                Password = "securepassword",
                Role = UserRole.User
            };

            _userService.AddUser(newUser);
            var userInDb = _dbContext.Users.FirstOrDefault(u => u.Username == "newuser");

            Assert.NotNull(userInDb);
            Assert.Equal("newuser", userInDb.Username);
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

            Assert.NotNull(updatedUser);
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

        // Dispose the database after each test
        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
