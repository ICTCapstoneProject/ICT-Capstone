using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class Review
    {
        [Key]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [Required]
        [Column("proposal_id")]
        public int ProposalId { get; set; }

        [Required]
        [Column("reviewed_by")]
        public int ReviewedBy { get; set; }

        [Column("decision")]
        public string Decision { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
