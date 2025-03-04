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
            var game = await _gameService.CreateGameAsync(request.Player1Id, request.Player2Id);
            return Ok(game);
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptGame([FromBody] AcceptGameRequest request)
        {
            bool accepted = await _gameService.AcceptGameAsync(request.GameId, request.PlayerId);
            if (accepted)
                return Ok("Game accepted");
            return BadRequest("Unable to accept the game");
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectGame([FromBody] RejectGameRequest request)
        {
            bool rejected = await _gameService.RejectGameAsync(request.GameId, request.PlayerId);
            if (rejected)
                return Ok("Game rejected");
            return BadRequest("Unable to reject the game");
        }

        [HttpPost("submit-score")]
        public async Task<IActionResult> SubmitScore([FromBody] SubmitScoreRequest request)
        {
            bool submitted = await _gameService.SubmitScoreAsync(request.GameId, request.PlayerId, request.Player1Score, request.Player2Score);

            if (submitted)
                return Ok("Score submitted, awaiting confirmation from the other player.");
            return BadRequest("Error submitting score.");
        }
        [HttpPost("confirm-score")]
        public async Task<IActionResult> ConfirmScore([FromBody] ConfirmScoreRequest request)
        {
            bool confirmed = await _gameService.ConfirmScoreAsync(request.GameId, request.PlayerId);

            if (confirmed)
                return Ok("Score confirmed, game finished.");
            return BadRequest("Error confirming score.");
        }
    }

}
