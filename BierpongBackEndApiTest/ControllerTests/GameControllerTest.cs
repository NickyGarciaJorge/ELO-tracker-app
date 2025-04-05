using BierpongProjectWebApi.Controllers;
using BierpongProjectWebApi.DTO.GameDTO_s;
using BierpongProjectWebApi.Models.Entities;
using BierpongProjectWebApi.Services;
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
    public class GameControllerTest
    {
        private readonly Mock<GameService> _mockGameService;
        private readonly GameController _controller;
        private ClaimsPrincipal _user;

        public GameControllerTest()
        {
            _mockGameService = new Mock<GameService>();
            _controller = new GameController(_mockGameService.Object);

            // Mock authenticated user
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };
        }

        [Fact]
        public async Task CreateGame_Should_Return_Forbidden_If_User_Not_Admin_Or_Player()
        {
            var request = new CreateGameRequest { Player1Id = Guid.NewGuid(), Player2Id = Guid.NewGuid() };

            // Simulate a non-admin user
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = _user;

            var result = await _controller.CreateGame(request);

            Assert.IsType<ForbidResult>(result);  // Forbidden for non-admin users
        }

        [Fact]
        public async Task CreateGame_Should_Return_Ok_If_Admin_Creates_Game()
        {
            var request = new CreateGameRequest { Player1Id = Guid.NewGuid(), Player2Id = Guid.NewGuid() };
            var game = new Game();

            // Simulate an admin user
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = _user;

            _mockGameService.Setup(s => s.CreateGameAsync(request.Player1Id, request.Player2Id)).ReturnsAsync(game);

            var result = await _controller.CreateGame(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(game, okResult.Value);
        }

        [Fact]
        public async Task AcceptGame_Should_Return_Forbidden_If_Not_Player()
        {
            var request = new AcceptGameRequest { GameId = Guid.NewGuid(), PlayerId = Guid.NewGuid() };

            // Simulate a non-participant user
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = _user;

            var result = await _controller.AcceptGame(request);

            Assert.IsType<ForbidResult>(result);  // Forbidden for non-participants
        }

        [Fact]
        public async Task AcceptGame_Should_Return_Ok_If_Player_Accepts_Game()
        {
            // Create a new game with two players
            var gameId = Guid.NewGuid();
            var player1Id = Guid.NewGuid();
            var player2Id = Guid.NewGuid();

            // Mock the user to be player1 (one of the players in the game)
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, player1Id.ToString()), // Set the logged-in user as player1
            new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = _user;

            // Create an accept game request for player1
            var request = new AcceptGameRequest { GameId = gameId, PlayerId = player1Id };

            // Set up the service mock to simulate successful game acceptance
            _mockGameService.Setup(s => s.AcceptGameAsync(request.GameId, request.PlayerId)).ReturnsAsync(true);

            // Call the controller method
            var result = await _controller.AcceptGame(request);

            // Assert the result is OK and the message is "Game accepted"
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Game accepted", okResult.Value);
        }


        [Fact]
        public async Task RejectGame_Should_Return_Forbidden_If_Not_Player()
        {
            var request = new RejectGameRequest { GameId = Guid.NewGuid(), PlayerId = Guid.NewGuid() };

            // Simulate a non-participant user
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = _user;

            var result = await _controller.RejectGame(request);

            Assert.IsType<ForbidResult>(result);  // Forbidden for non-participants
        }

        [Fact]
        public async Task SubmitScore_Should_Return_Forbidden_If_Not_Player_Or_Admin()
        {
            var request = new SubmitScoreRequest { GameId = Guid.NewGuid(), PlayerId = Guid.NewGuid(), Player1Score = 10, Player2Score = 8 };

            // Simulate a non-participant user
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = _user;

            var result = await _controller.SubmitScore(request);

            Assert.IsType<ForbidResult>(result);  // Forbidden for non-participants or non-admins
        }

        [Fact]
        public async Task SubmitScore_Should_Return_Ok_If_Player_Submits_Score()
        {
            // Create a new game
            var gameId = Guid.NewGuid();
            var player1Id = Guid.NewGuid();
            var player2Id = Guid.NewGuid();

            // Mock the user to be player1 (one of the players in the game)
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, player1Id.ToString()), // Set the logged-in user as player1
            new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = _user;

            // Create a score submission request
            var request = new SubmitScoreRequest { GameId = gameId, PlayerId = player1Id, Player1Score = 10, Player2Score = 8 };

            // Set up the service mock to simulate successful score submission
            _mockGameService.Setup(s => s.SubmitScoreAsync(request.GameId, request.PlayerId, request.Player1Score, request.Player2Score)).ReturnsAsync(true);

            // Call the controller method
            var result = await _controller.SubmitScore(request);

            // Assert the result is OK
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Score submitted, awaiting confirmation from the other player.", okResult.Value);
        }

        [Fact]
        public async Task ConfirmScore_Should_Return_Forbidden_If_Not_Player_Or_Admin()
        {
            var request = new ConfirmScoreRequest { GameId = Guid.NewGuid(), PlayerId = Guid.NewGuid() };

            // Simulate a non-participant user
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = _user;

            var result = await _controller.ConfirmScore(request);

            Assert.IsType<ForbidResult>(result);  // Forbidden for non-participants or non-admins
        }

        [Fact]
        public async Task ConfirmScore_Should_Return_Ok_If_Player_Confirms_Score()
        {
            // Create a new game
            var gameId = Guid.NewGuid();
            var player1Id = Guid.NewGuid();
            var player2Id = Guid.NewGuid();

            // Mock the user to be player1 (one of the players in the game)
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, player1Id.ToString()), // Set the logged-in user as player1
            new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = _user;

            // Create a confirm score request
            var request = new ConfirmScoreRequest { GameId = gameId, PlayerId = player1Id };

            // Set up the service mock to simulate successful score confirmation
            _mockGameService.Setup(s => s.ConfirmScoreAsync(request.GameId, request.PlayerId)).ReturnsAsync(true);

            // Call the controller method
            var result = await _controller.ConfirmScore(request);

            // Assert the result is OK
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Score confirmed, game finished.", okResult.Value);
        }

        [Fact]
        public async Task CreateGame_Should_Return_BadRequest_If_Request_Is_Null()
        {
            var result = await _controller.CreateGame(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateGame_Should_Return_BadRequest_If_PlayerIds_Are_Empty()
        {
            var request = new CreateGameRequest { Player1Id = Guid.Empty, Player2Id = Guid.Empty };
            var result = await _controller.CreateGame(request);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AcceptGame_Should_Return_BadRequest_If_Request_Is_Null()
        {
            var result = await _controller.AcceptGame(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AcceptGame_Should_Return_BadRequest_If_GameId_Or_PlayerId_Is_Empty()
        {
            var request = new AcceptGameRequest { GameId = Guid.Empty, PlayerId = Guid.Empty };
            var result = await _controller.AcceptGame(request);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RejectGame_Should_Return_BadRequest_If_Request_Is_Null()
        {
            var result = await _controller.RejectGame(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RejectGame_Should_Return_BadRequest_If_GameId_Or_PlayerId_Is_Empty()
        {
            var request = new RejectGameRequest { GameId = Guid.Empty, PlayerId = Guid.Empty };
            var result = await _controller.RejectGame(request);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SubmitScore_Should_Return_BadRequest_If_Request_Is_Null()
        {
            var result = await _controller.SubmitScore(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SubmitScore_Should_Return_BadRequest_If_GameId_Or_PlayerId_Is_Empty()
        {
            var request = new SubmitScoreRequest { GameId = Guid.Empty, PlayerId = Guid.Empty, Player1Score = 10, Player2Score = 15 };
            var result = await _controller.SubmitScore(request);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ConfirmScore_Should_Return_BadRequest_If_Request_Is_Null()
        {
            var result = await _controller.ConfirmScore(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ConfirmScore_Should_Return_BadRequest_If_GameId_Or_PlayerId_Is_Empty()
        {
            var request = new ConfirmScoreRequest { GameId = Guid.Empty, PlayerId = Guid.Empty };
            var result = await _controller.ConfirmScore(request);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
