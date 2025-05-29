using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class FinancialResource
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("proposal_id")]
        public int ProposalId { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("cost")]
        public decimal Cost { get; set; }

        [ForeignKey("ProposalId")]
        public Proposal Proposal { get; set; }
    }

}