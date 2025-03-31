using System;
using System.Linq;
using System.Threading.Tasks;
using BierpongProjectWebApi.Data;
using BierpongProjectWebApi.Models.Entities;
using BierpongProjectWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BierpongBackEndApiTest.ServiceTests
{
    public class GameServiceTests : IDisposable
    {
        private readonly CustomDbContext _dbContext;
        private readonly GameService _gameService;
        private readonly UserProfile _player1;
        private readonly UserProfile _player2;

        public GameServiceTests()
        {
            var options = new DbContextOptionsBuilder<CustomDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new CustomDbContext(options);
            _gameService = new GameService(_dbContext);

            _player1 = new UserProfile { UserId = Guid.NewGuid(), ELO = 1200 };
            _player2 = new UserProfile { UserId = Guid.NewGuid(), ELO = 1200 };

            _dbContext.UserProfiles.AddRange(_player1, _player2);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task CreateGameAsync_Should_Create_Game()
        {
            var game = await _gameService.CreateGameAsync(_player1.UserId, _player2.UserId);

            Assert.NotNull(game);
            Assert.Equal(_player1.UserId, game.Player1Id);
            Assert.Equal(_player2.UserId, game.Player2Id);
            Assert.Equal(GameStatus.Pending, game.Status);
        }

        [Fact]
        public async Task AcceptGameAsync_Should_Accept_Game()
        {
            var game = await _gameService.CreateGameAsync(_player1.UserId, _player2.UserId);
            var result = await _gameService.AcceptGameAsync(game.GameId, _player2.UserId);

            Assert.True(result);
            Assert.Equal(GameStatus.InProgress, game.Status);
        }

        [Fact]
        public async Task AcceptGameAsync_Should_Fail_If_Not_Player2()
        {
            var otherPlayerId = Guid.NewGuid();
            var game = await _gameService.CreateGameAsync(_player1.UserId, _player2.UserId);
            var result = await _gameService.AcceptGameAsync(game.GameId, otherPlayerId);

            Assert.False(result);
            Assert.Equal(GameStatus.Pending, game.Status);
        }

        [Fact]
        public async Task RejectGameAsync_Should_Remove_Game()
        {
            var game = await _gameService.CreateGameAsync(_player1.UserId, _player2.UserId);
            var result = await _gameService.RejectGameAsync(game.GameId, _player2.UserId);

            Assert.True(result);
            Assert.Null(await _dbContext.Games.FindAsync(game.GameId));
        }

        [Fact]
        public async Task SubmitScoreAsync_Should_Update_Score_And_Change_Status()
        {
            var game = await _gameService.CreateGameAsync(_player1.UserId, _player2.UserId);
            await _gameService.AcceptGameAsync(game.GameId, _player2.UserId);

            var result = await _gameService.SubmitScoreAsync(game.GameId, _player1.UserId, 10, 5);

            Assert.True(result);
            Assert.Equal(10, game.Player1Score);
            Assert.Equal(5, game.Player2Score);
            Assert.Equal(GameStatus.AwaitingConfirmation, game.Status);
        }

        [Fact]
        public async Task ConfirmScoreAsync_Should_Finish_Game()
        {
            var game = await _gameService.CreateGameAsync(_player1.UserId, _player2.UserId);
            await _gameService.AcceptGameAsync(game.GameId, _player2.UserId);
            await _gameService.SubmitScoreAsync(game.GameId, _player1.UserId, 10, 5);

            var result = await _gameService.ConfirmScoreAsync(game.GameId, _player2.UserId);

            Assert.True(result);
            Assert.Equal(GameStatus.Finished, game.Status);
            Assert.Equal(_player1.UserId, game.WinnerId);
            Assert.NotNull(game.EndTime);
        }

        [Fact]
        public async Task ConfirmScoreAsync_Should_Fail_If_Not_Player()
        {
            var otherPlayerId = Guid.NewGuid();
            var game = await _gameService.CreateGameAsync(_player1.UserId, _player2.UserId);
            await _gameService.AcceptGameAsync(game.GameId, _player2.UserId);
            await _gameService.SubmitScoreAsync(game.GameId, _player1.UserId, 10, 5);

            var result = await _gameService.ConfirmScoreAsync(game.GameId, otherPlayerId);

            Assert.False(result);
            Assert.Equal(GameStatus.AwaitingConfirmation, game.Status);
        }

        [Fact]
        public async Task ConfirmScoreAsync_Should_Fail_If_Game_Not_Awaiting_Confirmation()
        {
            var game = await _gameService.CreateGameAsync(_player1.UserId, _player2.UserId);
            await _gameService.AcceptGameAsync(game.GameId, _player2.UserId);

            var result = await _gameService.ConfirmScoreAsync(game.GameId, _player2.UserId);

            Assert.False(result);
            Assert.NotEqual(GameStatus.Finished, game.Status);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
