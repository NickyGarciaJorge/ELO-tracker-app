using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BierpongProjectWebApi.Models.Entities
{
    [Table("Friendship")]
    public class Friendship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FriendshipId { get; set; }  // Unique identifier for each friendship

        // User who sent the friend request
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public UserProfile User { get; set; } // Navigation property

        // Friend (recipient of request)
        [ForeignKey("FriendUser")]
        public Guid FriendUserId { get; set; }

        public UserProfile FriendUser { get; set; } // Navigation property

        [Column("Status")]
        public FriendshipStatus Status { get; set; }  // Enum to track the friendship status

        [Column("DateRequested")]
        public DateTime DateRequested { get; set; }

        [Column("DateAccepted")]
        public DateTime? DateAccepted { get; set; }
    }

    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Blocked,
        Rejected
    }
}
