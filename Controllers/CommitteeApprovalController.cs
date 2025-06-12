using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FSSA.Models;
using FSSA.DTOs;
using ProjectManagerMvc.Services;


[Authorize(Roles = "Ethics Committee,Committee Chair")]
public class CommitteeApprovalController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly ProjectManagerContext _context;

    public CommitteeApprovalController(ProjectManagerContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public IActionResult Index(string search = null)
    {

        var status = _context.Statuses
            .FirstOrDefault(s = s.StatusName == "Unsigned");

        if (status == null)
            return NotFound("Status not found");

        var query = _context.Proposals
            .Where(p => p.StatusId == status.StatusId);

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
    public async Task<IActionResult> UpdateStatus(int id, string actionType)
    {
        var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();

        string outcome;
        var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
        if (user == null) return Unauthorized();
        var userId = user.UserId;

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

        _context.ProposalLogs.Add(new ProposalLog
        {
            ProposalId = proposal.Id,
            StatusId = proposal.StatusId,
            ChangedBy = userId,
            Action = actionType,
            Timestamp = DateTime.Now
        });

        await _context.SaveChangesAsync();

        // Send notifications on approval
        if (actionType.ToLower() == "approve")
        {
            var approvalTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            var message = $"The proposal, <strong>#{proposal.Id}</strong> '<strong>{proposal.Title}</strong>' recieved committee approval on <strong>({approvalTime})</strong>, and requires chair approval before it can commence.";
            await _notificationService.CreateNotificationForRoleAsync(
                "Ethics Committee", message, proposal.Id, "ProposalApproved", userId);
            await _notificationService.CreateNotificationForRoleAsync(
                "Committee Chair", message, proposal.Id, "ProposalApproved", userId);
        }

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
        
        var comments = _context.Comments
            .Where(c => c.ProposalId == proposal.Id)
            .Join(_context.Users,
                c => c.Commenter,
                u => u.UserId,
                (c, u) => new CommentDto
                {
                    CommentId = c.CommentId,
                    Author = u.Name,
                    Timestamp = c.Timestamp,
                    CommentText = c.CommentText
                })
            .OrderByDescending(c => c.Timestamp)
            .ToList();

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
            StatusName = _context.Statuses.FirstOrDefault(s => s.StatusId == proposal.StatusId)?.StatusName ?? "Unknown",
            Comments = comments
        };

        return View("Details", model);
    }
    
    [HttpPost]
    public IActionResult AddComment(int ProposalId, string CommentText)
    {
        if (string.IsNullOrWhiteSpace(CommentText))
        {
            return RedirectToAction("Details", new { id = ProposalId });
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
        if (user == null)
            return Unauthorized();

        var comment = new Comment
        {
            ProposalId = ProposalId,
            CommentText = CommentText,
            Commenter = user.UserId,
            Timestamp = DateTime.Now
        };

        _context.Comments.Add(comment);
        _context.SaveChanges();

        return RedirectToAction("Details", new { id = ProposalId });
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
