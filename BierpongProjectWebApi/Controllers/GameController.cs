using BierpongProjectWebApi.DTO.GameDTO_s;
using BierpongProjectWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BierpongProjectWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensure all requests require authentication
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
        {
            if (request == null || request.Player1Id == Guid.Empty || request.Player2Id == Guid.Empty)
                return BadRequest("Invalid request data.");

            if (!IsAuthorizedUser(request.Player1Id) && !IsAuthorizedUser(request.Player2Id) && !User.IsInRole("Admin"))
                return Forbid();

            var game = await _gameService.CreateGameAsync(request.Player1Id, request.Player2Id);
            return Ok(game);
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptGame([FromBody] AcceptGameRequest request)
        {
            if (request == null || request.GameId == Guid.Empty || request.PlayerId == Guid.Empty)
                return BadRequest("Invalid request data.");

            if (!IsGameParticipant(request.GameId, request.PlayerId))
                return Forbid();

            bool accepted = await _gameService.AcceptGameAsync(request.GameId, request.PlayerId);
            return accepted ? Ok("Game accepted") : BadRequest("Unable to accept the game");
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectGame([FromBody] RejectGameRequest request)
        {
            if (request == null || request.GameId == Guid.Empty || request.PlayerId == Guid.Empty)
                return BadRequest("Invalid request data.");

            if (!IsGameParticipant(request.GameId, request.PlayerId))
                return Forbid();

            bool rejected = await _gameService.RejectGameAsync(request.GameId, request.PlayerId);
            return rejected ? Ok("Game rejected") : BadRequest("Unable to reject the game");
        }

        [HttpPost("submit-score")]
        public async Task<IActionResult> SubmitScore([FromBody] SubmitScoreRequest request)
        {
            if (request == null || request.GameId == Guid.Empty || request.PlayerId == Guid.Empty)
                return BadRequest("Invalid request data.");

            if (!IsGameParticipant(request.GameId, request.PlayerId) && !User.IsInRole("Admin"))
                return Forbid();

            bool submitted = await _gameService.SubmitScoreAsync(request.GameId, request.PlayerId, request.Player1Score, request.Player2Score);
            return submitted ? Ok("Score submitted, awaiting confirmation from the other player.") : BadRequest("Error submitting score.");
        }

        [HttpPost("confirm-score")]
        public async Task<IActionResult> ConfirmScore([FromBody] ConfirmScoreRequest request)
        {
            if (request == null || request.GameId == Guid.Empty || request.PlayerId == Guid.Empty)
                return BadRequest("Invalid request data.");

            if (!IsGameParticipant(request.GameId, request.PlayerId) && !User.IsInRole("Admin"))
                return Forbid();

            bool confirmed = await _gameService.ConfirmScoreAsync(request.GameId, request.PlayerId);
            return confirmed ? Ok("Score confirmed, game finished.") : BadRequest("Error confirming score.");
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelGame([FromBody] CancelGameRequest request)
        {
            if (request == null || request.GameId == Guid.Empty || request.PlayerId == Guid.Empty)
                return BadRequest("Invalid request data.");
            if (!IsGameParticipant(request.GameId, request.PlayerId) && !User.IsInRole("Admin"))
                return Forbid();
            bool canceled = await _gameService.CancelGameAsync(request.GameId, request.PlayerId);
            return canceled ? Ok("Game canceled") : BadRequest("Error canceling game.");
        }

        // Helper: Check if user is one of the players
        private bool IsGameParticipant(Guid gameId, Guid userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return currentUserId != null && Guid.Parse(currentUserId) == userId;
        }

        // Helper: Check if user is authorized to act as a player or admin
        private bool IsAuthorizedUser(Guid userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return User.IsInRole("Admin") || (currentUserId != null && Guid.Parse(currentUserId) == userId);
        }
    }
}
