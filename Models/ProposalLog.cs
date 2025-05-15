using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class ProposalLog
    {
        [Key]
        [Column("log_id")]
        public int LogId { get; set; }

        [Required]
        [Column("proposal_id")]
        public int ProposalId { get; set; }

        [Required]
        [Column("status_id")]
        public int StatusId { get; set; }

        [Required]
        [Column("changed_by")]
        public int ChangedBy { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        
        [Required]
        [Column("action")]
        public string Action { get; set; }
    }
}
