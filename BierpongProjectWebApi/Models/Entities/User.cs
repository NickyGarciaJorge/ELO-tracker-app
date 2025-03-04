using BierpongProjectWebApi.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BierpongProjectWebApi.Models.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public virtual Guid Id { get; set; }
        [Column("name")]
        public virtual string Name { get; set; }
        [Column("email")]
        public virtual string Email { get; set; }
        [Column("username")]
        public virtual string Username { get; set; }
        [Column("password")]
        public virtual string Password { get; set; }
        [Column("role")]
        public virtual UserRole Role { get; set; }
        public virtual UserProfile UserProfile { get; set; }

        public virtual List<Friendship> Friendships { get; set; }
    }

    public enum UserRole { Administrator, User }


    public class UserCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
