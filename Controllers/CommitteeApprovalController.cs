using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FSSA.Models;
using FSSA.DTOs;

[Authorize(Roles = "Ethics Committee")]
public class CommitteeApprovalController : Controller
{
    private readonly ProjectManagerContext _context;

    public CommitteeApprovalController(ProjectManagerContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var proposals = _context.Proposals
            .Where(p => p.StatusId == 1)
            .Include(p => p.Attachments)
            .ToList();

        return View("CommitteeApprovals", proposals);
    }

    [HttpPost]
    public IActionResult UpdateStatus(int id, string actionType)
    {
        var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();

        switch (actionType.ToLower())
        {
            case "approve":
                proposal.StatusId = 2;
                break;
            case "reject":
                proposal.StatusId = 6;
                break;
            default:
                return BadRequest("Invalid action");
        }

        proposal.UpdatedAt = DateTime.Now;
        var userId = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name)?.UserId ?? 0;

        _context.ProposalLogs.Add(new ProposalLog
        {
            ProposalId = proposal.Id,
            StatusId = proposal.StatusId,
            ChangedBy = userId,
            Action = actionType,
            Timestamp = DateTime.Now
        });

        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Details(int id)
    {
        var proposal = _context.Proposals
            .Include(p => p.Attachments)
            .FirstOrDefault(p => p.Id == id);

        if (proposal == null)
            return NotFound();

        var submitter = _context.Users.FirstOrDefault(u => u.UserId == proposal.SubmittedBy);
        var leadResearcher = _context.Users.FirstOrDefault(u => u.UserId == proposal.LeadResearcherId);

        var projectLevelName = _context.ProjectLevels
            .FirstOrDefault(pl => pl.LevelId == proposal.ProjectLevelId)?.LevelName ?? "Unknown";

        var financialResources = _context.FinancialResources
            .Where(fr => fr.ProposalId == proposal.Id)
            .Select(fr => new FinancialResourceDto
            {
                Title = fr.Title,
                Cost = (double)fr.Cost
            }).ToList();

        var coResearchers = _context.ProposalResearchers
            .Where(pr => pr.ProposalId == proposal.Id)
            .Join(_context.Users,
                pr => pr.UserId,
                u => u.UserId,
                (pr, u) => new CoResearcherDto
                {
                    Id = u.UserId.ToString(),
                    Name = u.Name
                }).ToList();

        var model = new MyProposalViewModel
        {
            Id = proposal.Id,
            Title = proposal.Title,
            Synopsis = proposal.Synopsis,
            Method = proposal.Method,
            ProjectLevel = projectLevelName,
            PhysicalResources = proposal.PhysicalResources,
            EthicalConsiderations = proposal.EthicalConsiderations,
            Outcomes = proposal.Outcomes,
            Milestones = proposal.Milestones,
            EstimatedCompletionDate = proposal.EstimatedCompletionDate,
            SubmittedByName = submitter?.Name ?? "Unknown",
            LeadResearcherName = leadResearcher?.Name ?? "Unknown",
            FinancialResources = financialResources,
            CoResearchers = coResearchers,
            Attachments = proposal.Attachments.ToList(),
            StatusName = _context.Statuses.FirstOrDefault(s => s.StatusId == proposal.StatusId)?.StatusName ?? "Unknown"
        };

        return View("Details", model);
    }



}
