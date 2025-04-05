namespace BierpongProjectWebApi.DTO.GameDTO_s
{
    public class CreateGameRequest
    {
        public Guid Player1Id { get; set; }
        public Guid Player2Id { get; set; }
    }

    public class AcceptGameRequest
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
    }

    public class RejectGameRequest
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
    }

    public class SubmitScoreRequest
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
    }

    public class ConfirmScoreRequest
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
    }

    public class CancelGameRequest
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
    }
}
