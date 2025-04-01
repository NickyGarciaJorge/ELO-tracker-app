using BierpongProjectWebApi.Data;
using BierpongProjectWebApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BierpongProjectWebApi.Services
{
    public class GameService
    {
        private readonly CustomDbContext _context;

        public GameService()
        {
            
        }

        public GameService(CustomDbContext context)
        {
            _context = context;
        }

        public virtual async Task<Game> CreateGameAsync(Guid player1Id, Guid player2Id)
        {
            var game = new Game
            {
                Player1Id = player1Id,
                Player2Id = player2Id,
                StartTime = DateTime.UtcNow,
                Status = GameStatus.Pending
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }

        public virtual async Task<bool> AcceptGameAsync(Guid gameId, Guid playerId)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameId == gameId);

            if (game == null || game.Status != GameStatus.Pending)
                return false;

            if (game.Player2Id == playerId)
            {
                game.Status = GameStatus.InProgress;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public virtual async Task<bool> RejectGameAsync(Guid gameId, Guid playerId)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameId == gameId);

            if (game == null || game.Status != GameStatus.Pending)
                return false;

            if (game.Player2Id == playerId)
            {
                _context.Games.Remove(game); // Remove rejected game from DB
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public virtual async Task<bool> SubmitScoreAsync(Guid gameId, Guid playerId, int player1Score, int player2Score)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameId == gameId);

            if (game == null || game.Status != GameStatus.InProgress)
                return false;

            // Allow either player to submit the score
            if (game.Player1Id == playerId || game.Player2Id == playerId)
            {
                game.Player1Score = player1Score;
                game.Player2Score = player2Score;

                // After score submission, the game enters awaiting confirmation state
                game.Status = GameStatus.AwaitingConfirmation;

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public virtual async Task<bool> ConfirmScoreAsync(Guid gameId, Guid playerId)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameId == gameId);

            if (game == null || game.Status != GameStatus.AwaitingConfirmation)
                return false;

            // Check if it's the other player's turn to confirm
            if (game.Player1Id != playerId && game.Player2Id != playerId)
                return false;

            // The second player confirms the score
            game.ConfirmedBy = playerId;

            // Set the winner based on the score
            game.WinnerId = (Guid)(game.Player1Score == 0 ? game.Player2Id : game.Player1Id);
            game.Scoreline = $"{game.Player1Score}-{game.Player2Score}";
            game.Status = GameStatus.Finished;
            game.EndTime = DateTime.UtcNow;

            // Update Elo scores
            await AddMatchHistoryAsync(game);

            await _context.SaveChangesAsync();
            return true;
        }

        //change elo calc
        private async Task AddMatchHistoryAsync(Game game)
        {
            var player1Profile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == game.Player1Id);
            var player2Profile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == game.Player2Id);

            double kFactor = CalcKFactor(game.Player1Score, game.Player2Score);
            double ePlayer1 = 0;
            double ePlayer2 = 0;


            if (game.Player1Score > game.Player2Score)
            {
                ePlayer1 = 1 / (1 + Math.Pow(10, (double)(player2Profile.ELO - player1Profile.ELO) / 600));
                ePlayer2 = 1 - ePlayer1;
            }
            else
            {
                ePlayer2 = 1 / (1 + Math.Pow(10, (double)(player1Profile.ELO - player2Profile.ELO) / 600));
                ePlayer1 = 1 - ePlayer2;
            }

            double sPlayer1 = game.Player1Score > game.Player2Score ? 1 : 0;
            double sPlayer2 = game.Player2Score > game.Player1Score ? 1 : 0;

            int newPlayer1Rating = (int)Math.Round(CalcElo(player1Profile.ELO, kFactor, sPlayer1, ePlayer1));
            int newPlayer2Rating = (int)Math.Round(CalcElo(player2Profile.ELO, kFactor, sPlayer2, ePlayer2));

            var matchHistory1 = new MatchHistory
            {
                PlayerId = (Guid)game.Player1Id,
                GameId = game.GameId,
                Date = DateTime.UtcNow,
                Scoreline = game.Scoreline,
                EloChange = player1Profile.ELO - newPlayer1Rating,
                NewElo = newPlayer1Rating
            };

            var matchHistory2 = new MatchHistory
            {
                PlayerId = (Guid)game.Player2Id,
                GameId = game.GameId,
                Date = DateTime.UtcNow,
                Scoreline = game.Scoreline,
                EloChange = player2Profile.ELO - newPlayer2Rating,
                NewElo = newPlayer2Rating
            };

            _context.MatchHistories.Add(matchHistory1);
            _context.MatchHistories.Add(matchHistory2);

            await _context.SaveChangesAsync();
        }

        private double CalcKFactor(int scorePlayer1, int scorePlayer2)
        {
            return 0.009 * Math.Pow(Math.Abs(scorePlayer1 - scorePlayer2), 4) + 15;
        }

        private double CalcElo(double rOld, double k, double s, double e)
        {
            return rOld + k * (s - e);
        }
    }
}
