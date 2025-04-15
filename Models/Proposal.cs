using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSSA.Models
{
    public class Proposal
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; }

        [Column("synopsis")]
        public string Synopsis { get; set; }

        [Column("method")]
        public string Method { get; set; }

        [Column("project_level_id")]
        public int ProjectLevelId { get; set; }

        [Column("resources")]
        public string Resources { get; set; }

        [Column("ethical_considerations")]
        public string EthicalConsiderations { get; set; }

        [Column("outcomes")]
        public string Outcomes { get; set; }

        [Column("milestones")]
        public string Milestones { get; set; }

        [Column("estimated_completion_date")]
        public DateTime EstimatedCompletionDate { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [Column("submitted_by")]
        public int SubmittedBy { get; set; }
    }
}
