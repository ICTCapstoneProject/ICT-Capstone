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
        public User User { get; set; }

        [Required]
        [Column("message")]
        public string Message { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; } = false; // Default to false

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default to current time

        [Column("proposal_id")]
        public int? ProposalId { get; set; } 
        [ForeignKey("ProposalId")]
        public Proposal Proposal { get; set; } 

        [Column("notification_type")]
        [StringLength(50)] 
        public string NotificationType { get; set; } 
    }
}
