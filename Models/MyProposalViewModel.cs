using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FSSA.DTOs;


namespace FSSA.Models{
    public class MyProposalViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Synopsis")]
        public string Synopsis { get; set; }

        [Display(Name = "Method")]
        public string Method { get; set; }

        [Display(Name = "Project Level")]
        public string ProjectLevel { get; set; }

        [Display(Name = "Physical Resources")]
        public string PhysicalResources { get; set; }

        [Display(Name = "Financial Resources")]
        public List<FinancialResourceDto> FinancialResources { get; set; } = new List<FinancialResourceDto>();

        [Display(Name = "Ethical Considerations")]
        public string EthicalConsiderations { get; set; }

        [Display(Name = "Outcomes")]
        public string Outcomes { get; set; }

        [Display(Name = "Milestones")]
        public string Milestones { get; set; }

        [Display(Name = "Estimated Completion Date")]
        public DateTime EstimatedCompletionDate { get; set; }

        [Display(Name = "Submitted By")]
        public string SubmittedByName { get; set; }

        [Display(Name = "Project Status")]
        public string StatusName { get; set; }

        [Display(Name = "Attachments")]
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        [Display(Name = "Lead Researcher")]
        public string LeadResearcherName { get; set; }

        [Display(Name = "Co-Researchers")]
        public List<CoResearcherDto> CoResearchers { get; set; } = new List<CoResearcherDto>();

        public string MethodImage { get; set; }

        [Display(Name = "Comment")]
        public string? Comment { get; set; }
    }

}

