using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class Comment
    {
        [Column("comment_id")]
        public int Id { get; set; }

        [Column("comment")]
        public string Content { get; set; } = string.Empty;

        [Column("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Foreign Keys
        [Column("review_id")]
        public int ProposalId { get; set; }

        [ForeignKey("ProposalId")]
        public Proposal? Proposal { get; set; }

        [Column("commenter")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
