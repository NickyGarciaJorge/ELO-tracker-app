using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BierpongProjectWebApi.Models.Entities
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        public Guid UserId { get; set; }  // This should be the foreign key from the User table

        [Column("Name")]
        public string Name { get; set; }

        [Column("Bio")]
        public string Bio { get; set; }

        [Column("ProfilePictureUrl")]
        public string ProfilePictureUrl { get; set; }

        [Column("ELO")]
        public int ELO { get; set; }

        // Navigation property for friends (many-to-many relationship via a junction table)
        public List<Friendship> Friendships { get; set; }

        // You can also create a list for game history if needed
        //public List<GameHistory> GameHistory { get; set; }
    }
}
