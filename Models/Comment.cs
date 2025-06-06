using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class Comment
    {
        [Key]
        [Column("comment_id")]
        public int CommentId { get; set; }

        [Required]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [Required]
        [Column("commenter")]
        public int Commenter { get; set; }

        [Required]
        [Column("comment")]
        public string CommentText { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        // Foreign Key for Proposal
        [Required]
        [Column("proposal_id")]
        public int ProposalId { get; set; }

        // Navigation property
        [ForeignKey("ProposalId")]
        public Proposal Proposal { get; set; }
    }
}
