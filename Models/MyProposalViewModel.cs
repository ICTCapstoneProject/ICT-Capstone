using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization.Infrastructure;

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

    [Display(Name = "Resources")]
    public string Resources { get; set; }

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

    [Display(Name = "Method Image")]
    public string MethodImage { get; set; } 
}
