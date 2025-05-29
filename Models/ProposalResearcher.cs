using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class ProposalResearcher
    {
        [Column("proposal_id")]
        public int ProposalId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        public Proposal Proposal { get; set; }
        public User User { get; set; }
    }
}