using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BierpongProjectWebApi.Models.Entities
{
    [Table("Friendship")]
    public class Friendship
    {
        [Key]
        public Guid FriendshipId { get; set; }  // Unique identifier for each friendship

        [ForeignKey("User")]
        public Guid UserId { get; set; }  // The ID of the user who initiated the friendship

        [ForeignKey("FriendUser")]
        public Guid FriendUserId { get; set; }  // The ID of the user who is the friend

        [Column("Status")]
        public FriendshipStatus Status { get; set; }  // Enum to track the status of the friendship

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
