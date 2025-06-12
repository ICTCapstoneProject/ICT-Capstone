using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FSSA.Models;
using FSSA.DTOs;
using ProjectManagerMvc.Services;

[Authorize(Roles = "Committee Chair")]
public class ChairApprovalController : Controller
{
    private readonly ProjectManagerContext _context;
    private readonly INotificationService _notificationService;

    public ChairApprovalController(ProjectManagerContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public IActionResult Index(string search = null)
    {
        var status = _context.Statuses
            .FirstOrDefault(s => s.StatusName == "Committee Approved");

        if (status == null)
            return NotFound("Status not found.");

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
    public async Task<IActionResult> Approve(int id)
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

        await _context.SaveChangesAsync();

        // Send notifications 
        var approvalTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        var message = $"The proposal, <strong>#{proposal.Id} '{proposal.Title}'</strong> received <strong>Chair approval</strong> on <strong>({approvalTime})</strong> and may now be set to <strong>commence</strong>.";

        // Notify manager role
        await _notificationService.CreateNotificationForRoleAsync(
            "Manager", message, proposal.Id, "ChairApproved", userId);


        return RedirectToAction("Success", new { id = proposal.Id, outcome = "approved" });
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

    [HttpPost]
    public IActionResult RequestAdditionalReview(int id)
    {
        var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();

        proposal.StatusId = 1; // Back to Committee Review
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
            Action = "request_additional_review",
            Timestamp = DateTime.Now
        });

        _context.SaveChanges();

        // Send to a matching Success view
        return RedirectToAction("Success", new { id = proposal.Id, outcome = "additional_review" });
    }

    [HttpPost]
    public IActionResult Reject(int id)
    {
        var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();

        proposal.StatusId = 6;
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
            Action = "reject",
            Timestamp = DateTime.Now
        });

        _context.SaveChanges();

        return RedirectToAction("Success", new { id = proposal.Id, outcome = "rejected" });
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
    
}
