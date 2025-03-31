using BierpongProjectWebApi.Controllers;
using BierpongProjectWebApi.DTO.GameDTO_s;
using BierpongProjectWebApi.Models.Entities;
using BierpongProjectWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BierpongBackEndApiTest.ControllerTests
{
    public class GameControllerTest
    {
        private readonly Mock<GameService> _mockGameService;
        private readonly GameController _controller;

        public GameControllerTest()
        {
            _mockGameService = new Mock<GameService>();
            _controller = new GameController(_mockGameService.Object);
        }

        [Fact]
        public async Task CreateGame_Should_Return_Ok_With_Game()
        {
            var request = new CreateGameRequest { Player1Id = Guid.NewGuid(), Player2Id = Guid.NewGuid() };
            var game = new Game();
            _mockGameService.Setup(s => s.CreateGameAsync(request.Player1Id, request.Player2Id)).ReturnsAsync(game);

            var result = await _controller.CreateGame(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(game, okResult.Value);
        }

        [Fact]
        public async Task AcceptGame_Should_Return_Ok_If_Accepted()
        {
            var request = new AcceptGameRequest { GameId = Guid.NewGuid(), PlayerId = Guid.NewGuid() };
            _mockGameService.Setup(s => s.AcceptGameAsync(request.GameId, request.PlayerId)).ReturnsAsync(true);

            var result = await _controller.AcceptGame(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Game accepted", okResult.Value);
        }

        [Fact]
        public async Task AcceptGame_Should_Return_BadRequest_If_Not_Accepted()
        {
            var request = new AcceptGameRequest { GameId = Guid.NewGuid(), PlayerId = Guid.NewGuid() };
            _mockGameService.Setup(s => s.AcceptGameAsync(request.GameId, request.PlayerId)).ReturnsAsync(false);

            var result = await _controller.AcceptGame(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unable to accept the game", badRequestResult.Value);
        }

        [Fact]
        public async Task RejectGame_Should_Return_Ok_If_Rejected()
        {
            var request = new RejectGameRequest { GameId = Guid.NewGuid(), PlayerId = Guid.NewGuid() };
            _mockGameService.Setup(s => s.RejectGameAsync(request.GameId, request.PlayerId)).ReturnsAsync(true);

            var result = await _controller.RejectGame(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Game rejected", okResult.Value);
        }

        [Fact]
        public async Task SubmitScore_Should_Return_Ok_If_Submitted()
        {
            var request = new SubmitScoreRequest { GameId = Guid.NewGuid(), PlayerId = Guid.NewGuid(), Player1Score = 10, Player2Score = 8 };
            _mockGameService.Setup(s => s.SubmitScoreAsync(request.GameId, request.PlayerId, request.Player1Score, request.Player2Score)).ReturnsAsync(true);

            var result = await _controller.SubmitScore(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Score submitted, awaiting confirmation from the other player.", okResult.Value);
        }

        [Fact]
        public async Task ConfirmScore_Should_Return_Ok_If_Confirmed()
        {
            var request = new ConfirmScoreRequest { GameId = Guid.NewGuid(), PlayerId = Guid.NewGuid() };
            _mockGameService.Setup(s => s.ConfirmScoreAsync(request.GameId, request.PlayerId)).ReturnsAsync(true);

            var result = await _controller.ConfirmScore(request);

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
