using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BierpongProjectWebApi.Models.Entities
{
    [Table("Game")]
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public virtual Guid GameId { get; set; }

        [Column("player1_id")]
        public virtual Guid? Player1Id { get; set; }

        [Column("player2_id")]
        public virtual Guid? Player2Id { get; set; }

        [Column("start_time")]
        public virtual DateTime StartTime { get; set; }

        [Column("status")]
        public virtual GameStatus Status { get; set; }

        [Column("player1_score")]
        public virtual int Player1Score { get; set; }

        [Column("player2_score")]
        public virtual int Player2Score { get; set; }

        [Column("winner_id")]
        public virtual Guid WinnerId { get; set; }

        [Column("scoreline")]
        public virtual string? Scoreline { get; set; }

        [Column("end_time")]
        public virtual DateTime? EndTime { get; set; }

        [Column("confirmed_by")]
        public virtual Guid? ConfirmedBy { get; set; }

        public virtual User? Player1 { get; set; }
        public virtual User? Player2 { get; set; }
    }

    public enum GameStatus
    {
        Pending,                  // Game invitation has been sent, awaiting acceptance
        InProgress,               // Game is currently being played
        AwaitingConfirmation,     // Score has been submitted and awaiting confirmation
        Finished                 // Game is finished, Elo updates and match history created
    }
}
