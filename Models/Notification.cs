using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class Notification
    {
        [Key]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("message")]
        public string Message { get; set; }

        [Column("proposal_id")]
        public int? ProposalId { get; set; }

        [Column("notification_type")]
        public string NotificationType { get; set; } = "General";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; } = false;
        public virtual User? User { get; set; }
        public virtual Proposal? Proposal { get; set; }
    }
}
