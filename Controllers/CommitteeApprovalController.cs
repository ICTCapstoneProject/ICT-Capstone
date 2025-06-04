using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FSSA.Models;
using FSSA.DTOs;

[Authorize(Roles = "Ethics Committee,Committee Chair")]
public class CommitteeApprovalController : Controller
{
    private readonly ProjectManagerContext _context;

    public CommitteeApprovalController(ProjectManagerContext context)
    {
        _context = context;
    }

    public IActionResult Index(string search = null)
    {
        var query = _context.Proposals
            .Where(p => p.StatusId == 1);

        if (!string.IsNullOrWhiteSpace(search))
        {
            if (int.TryParse(search, out int searchId))
                query = query.Where(p => p.Id == searchId);
            else
                query = query.Where(p => p.Title.ToLower().Contains(search.ToLower()));
        }

        var proposals = query
            .Include(p => p.Attachments)
            .ToList();

        ViewBag.Search = search ?? "";
        return View("CommitteeApprovals", proposals);
    }

    [HttpPost]
    public IActionResult UpdateStatus(int id, string actionType)
    {
        Console.WriteLine($"UpdateStatus called: id={id}, actionType={actionType}");
    
        var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();

        string outcome;
        switch (actionType.ToLower())
        {
            case "approve":
                proposal.StatusId = 2;
                outcome = "approved";
                break;
            case "reject":
                proposal.StatusId = 6;
                outcome = "rejected";
                break;
            case "requestmodification":
                proposal.StatusId = 7;
                outcome = "requires modification";
                break;
            default:
                return BadRequest("Invalid action");
        }

        proposal.UpdatedAt = DateTime.Now;

        var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
        if (user == null)
            return Unauthorized();

        var userId = user.UserId;

        _context.ProposalLogs.Add(new ProposalLog
        {
            ProposalId = proposal.Id,
            StatusId = proposal.StatusId,
            ChangedBy = userId,
            Action = actionType,
            Timestamp = DateTime.Now
        });

        _context.SaveChanges();
        return RedirectToAction("Success", new { id = proposal.Id, outcome = outcome });
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
    
    public IActionResult Success(int id, string outcome)
    {
        var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();

        var model = new MyProposalViewModel
        {
            Id = proposal.Id,
            Title = proposal.Title
        };
        ViewBag.Outcome = outcome;
        return View("Success", model); 
    }



}
