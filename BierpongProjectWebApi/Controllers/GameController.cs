using BierpongProjectWebApi.DTO.GameDTO_s;
using BierpongProjectWebApi.Services;
using Microsoft.AspNetCore.Mvc;

//TODO add authorization to the controller!!!!!!!!!!!!!!!

namespace BierpongProjectWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            try
            {
                if (request == null || request.Player1Id == Guid.Empty || request.Player2Id == Guid.Empty)
                    return BadRequest("Invalid request data.");

                var game = await _gameService.CreateGameAsync(request.Player1Id, request.Player2Id);
                return Ok(game);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptGame([FromBody] AcceptGameRequest request)
        {
            try
            {
                if (request == null || request.GameId == Guid.Empty || request.PlayerId == Guid.Empty)
                    return BadRequest("Invalid request data.");

                bool accepted = await _gameService.AcceptGameAsync(request.GameId, request.PlayerId);
                return accepted ? Ok("Game accepted") : BadRequest("Unable to accept the game");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectGame([FromBody] RejectGameRequest request)
        {
            try
            {
                if (request == null || request.GameId == Guid.Empty || request.PlayerId == Guid.Empty)
                    return BadRequest("Invalid request data.");

                bool rejected = await _gameService.RejectGameAsync(request.GameId, request.PlayerId);
                return rejected ? Ok("Game rejected") : BadRequest("Unable to reject the game");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("submit-score")]
        public async Task<IActionResult> SubmitScore([FromBody] SubmitScoreRequest request)
        {
            try
            {
                if (request == null || request.GameId == Guid.Empty || request.PlayerId == Guid.Empty)
                    return BadRequest("Invalid request data.");

                bool submitted = await _gameService.SubmitScoreAsync(request.GameId, request.PlayerId, request.Player1Score, request.Player2Score);
                return submitted ? Ok("Score submitted, awaiting confirmation from the other player.") : BadRequest("Error submitting score.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("confirm-score")]
        public async Task<IActionResult> ConfirmScore([FromBody] ConfirmScoreRequest request)
        {
            try
            {
                if (request == null || request.GameId == Guid.Empty || request.PlayerId == Guid.Empty)
                    return BadRequest("Invalid request data.");

                bool confirmed = await _gameService.ConfirmScoreAsync(request.GameId, request.PlayerId);
                return confirmed ? Ok("Score confirmed, game finished.") : BadRequest("Error confirming score.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
