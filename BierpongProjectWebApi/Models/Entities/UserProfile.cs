using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BierpongProjectWebApi.Models.Entities
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [ForeignKey("User")]
        [Column("user_id")]
        public virtual Guid UserId { get; set; }  // This should be the foreign key from the User table

        [Column("Name")]
        public virtual string? Name { get; set; }

        [Column("Bio")]
        public virtual string Bio { get; set; } = "Default Bio";

        [Column("ProfilePictureUrl")]
        public virtual string ProfilePictureUrl { get; set; } = "http://default.com/profile.jpg";

        [Column("ELO")]
        public virtual int ELO { get; set; }

        public virtual User User { get; set; }

        // Navigation property for friends (many-to-many relationship via a junction table)
        public virtual List<Friendship> Friendships { get; set; }
    }
}
