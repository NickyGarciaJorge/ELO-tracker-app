using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BierpongProjectWebApi.Models.Entities
{
    [Table("MatchHistory")]
    public class MatchHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public virtual int MatchHistoryId { get; set; }

        [Column("player_id")]
        public virtual Guid PlayerId { get; set; }

        [Column("game_id")]
        public virtual Guid GameId { get; set; }

        [Column("date")]
        public virtual DateTime Date { get; set; }

        [Column("scoreline")]
        public virtual string Scoreline { get; set; }

        [Column("elo_change")]
        public virtual int EloChange { get; set; }
        [Column("new_elo")]
        public virtual int NewElo { get; set; }

        public virtual User Player { get; set; }
        public virtual Game Game { get; set; }
    }
}
