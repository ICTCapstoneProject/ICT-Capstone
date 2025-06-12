using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FSSA.Models;
using FSSA.DTOs;
using ProjectManagerMvc.Services;

[Authorize]
public class CompleteController : Controller
{
    private readonly ProjectManagerContext _context;
    private readonly INotificationService _notificationService;

    public CompleteController(ProjectManagerContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

   public IActionResult Index(string search = null)
    {
        var email = User.Identity?.Name;
        var user = _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefault(u => u.Email == email);
        if (user == null) return Unauthorized();

        var isCommitteeMember = user.UserRoles.Any(r => r.Role.RoleName == "Ethics Committee");
        var isChair = user.UserRoles.Any(r => r.Role.RoleName == "Committee Chair");

        var commencedStatus = _context.Statuses
            .FirstOrDefault(s => s.StatusName == "Commenced");

        if (commencedStatus == null) return NotFound("Status not found.");


        var query = _context.Proposals.Where(p => p.StatusId == commencedStatus.StatusId &&
                            (p.SubmittedBy == user.UserId || isCommitteeMember || isChair));

        if (!string.IsNullOrWhiteSpace(search))
        {
            if (int.TryParse(search, out int searchId))
                query = query.Where(p => p.Id == searchId);
            else
                query = query.Where(p => p.Title.ToLower().Contains(search.ToLower()));
        }

        var proposals = query.Include(p => p.Attachments).ToList();

        ViewBag.Search = search ?? "";
        return View("Complete", proposals);
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
    public async Task<IActionResult> MarkComplete(int id)
    {
        var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();

        proposal.StatusId = 5;
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
            Action = "marked_complete",
            Timestamp = DateTime.Now
        });

       await _context.SaveChangesAsync();
        // Notify Chair that a proposal was complete
       var completionTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        var message = $"Proposal <strong>#{proposal.Id} '{proposal.Title}'</strong> was <strong>marked as complete</strong> on <strong>{completionTime}.</strong>";

        await _notificationService.CreateNotificationForRoleAsync(
            "Committee Chair", message, proposal.Id, "ProposalCompleted", userId);

        var model = new MyProposalViewModel
        {
            Id = proposal.Id,
            Title = proposal.Title
        };
        return View("Success", model);

        return RedirectToAction("Success", new { id = proposal.Id });
    }

    public IActionResult Success(int id)
    {
        var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();

        var model = new MyProposalViewModel
        {
            Id = proposal.Id,
            Title = proposal.Title
        };
        return View("Success", model);
    }
}
