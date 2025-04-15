using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class EthicsReview
    {
        [Key]
        [Column("ethics_id")]
        public int EthicsId { get; set; }

        [Required]
        [Column("proposal_id")]
        public int ProposalId { get; set; }

        [Required]
        [Column("ethics_required")]
        public bool EthicsRequired { get; set; }

        [Column("hrec_name")]
        public string HrecName { get; set; }

        [Column("hrec_number")]
        public string HrecNumber { get; set; }

        [Column("opinion")]
        public string Opinion { get; set; }

        [Column("reviewed_by")]
        public int ReviewedBy { get; set; }

        [Column("review_date")]
        public DateTime ReviewDate { get; set; }
    }
}
