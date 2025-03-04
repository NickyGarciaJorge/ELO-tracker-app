using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BierpongProjectWebApi.Models.Entities
{
    [Table("Friendship")]
    public class Friendship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid FriendshipId { get; set; }  // Unique identifier for each friendship

        // User who sent the friend request
        [ForeignKey("User")]
        public virtual Guid UserId { get; set; }

        public UserProfile User { get; set; } // Navigation property

        // Friend (recipient of request)
        [ForeignKey("FriendUser")]
        public virtual Guid FriendUserId { get; set; }

        public virtual UserProfile FriendUser { get; set; } // Navigation property

        [Column("Status")]
        public virtual FriendshipStatus Status { get; set; }  // Enum to track the friendship status

        [Column("DateRequested")]
        public virtual DateTime DateRequested { get; set; }

        [Column("DateAccepted")]
        public virtual DateTime? DateAccepted { get; set; }
    }

    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Blocked,
        Rejected
    }
}
