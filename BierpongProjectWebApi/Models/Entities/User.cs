using BierpongProjectWebApi.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BierpongProjectWebApi.Domain.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("role")]
        public UserRole Role { get; set; }
        public UserProfile UserProfile { get; set; }

        public List<Friendship> Friendships { get; set; }

    }

    public enum UserRole { Administrator, User }


    public class UserCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
