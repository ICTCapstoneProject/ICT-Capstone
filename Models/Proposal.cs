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

        [Required]
        [Column("synopsis")]
        public string Synopsis { get; set; }

        [Required]
        [Column("method")]
        public string Method { get; set; }

        [Required]
        [Display(Name = "Project Level")]
        [Column("project_level_id")]
        public int ProjectLevelId { get; set; }

        [Required]
        [Column("physical_resources")]
        public string PhysicalResources { get; set; }

        [Required]
        [Display(Name = "Ethical Considerations")]
        [Column("ethical_considerations")]
        public string EthicalConsiderations { get; set; }

        [Required]
        [Column("outcomes")]
        public string Outcomes { get; set; }

        [Required]
        [Column("milestones")]
        public string Milestones { get; set; }

        [Required]
        [Display(Name = "Estimated Completion Date")]
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

        [Column("lead_researcher_id")]
        public int LeadResearcherId { get; set; }

        [InverseProperty("Proposal")]
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        
        [Column("comments")]
        public string Comments;
    }
}
