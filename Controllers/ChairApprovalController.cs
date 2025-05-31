using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FSSA.Models;
using FSSA.DTOs;

[Authorize(Roles = "Committee Chair")]
public class ChairApprovalController : Controller
{
    private readonly ProjectManagerContext _context;

    public ChairApprovalController(ProjectManagerContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var proposals = _context.Proposals
            .Where(p => p.StatusId == 2)
            .Include(p => p.Attachments)
            .ToList();

        return View("ChairApproval", proposals);
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
        var projectLevelName = _context.ProjectLevels.FirstOrDefault(pl => pl.LevelId == proposal.ProjectLevelId)?.LevelName ?? "Unknown";

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

    [HttpPost]
    public IActionResult Approve(int id)
    {
        var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();

        proposal.StatusId = 3;
        proposal.UpdatedAt = DateTime.Now;

        var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
        if (user == null)
            return Unauthorized();

        var userId = user.UserId;
        
        _context.ProposalLogs.Add(new ProposalLog
        {
            ProposalId = id,
            StatusId = proposal.StatusId,
            ChangedBy = userId,
            Action = "chair_approve",
            Timestamp = DateTime.Now
        });

        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}
