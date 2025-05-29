using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class ProposalHistory
    {
        [Key]
        public int HistoryId { get; set; }
        public int ProposalId { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public string Method { get; set; }
        public int ProjectLevelId { get; set; }
        public string PhysicalResources { get; set; }
        public string EthicalConsiderations { get; set; }
        public string Outcomes { get; set; }
        public string Milestones { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }
        public int LeadResearcherId { get; set; }
        public int StatusId { get; set; }

        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public int ChangedBy { get; set; }
    }
}