using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FSSA.Models;
using FSSA.DTOs;
using ProjectManagerMvc.Services;

// Only Manager and Chair users can access this controller
[Authorize(Roles = "Manager,Committee Chair")]
public class ManagerCommenceController : Controller
{
    private readonly ProjectManagerContext _context;
    private readonly INotificationService _notificationService;


    public ManagerCommenceController(ProjectManagerContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    // Display proposals status is 'Chair Approved' for view
    public IActionResult Index(string search = null)
    {
        var status = _context.Statuses
            .FirstOrDefault(s => s.StatusName == "Chair Approved");

        if (status == null)
            return NotFound("Status not found.");

        var query = _context.Proposals.Where(p => p.StatusId == status.StatusId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            if (int.TryParse(search, out int searchId))
                query = query.Where(p => p.Id == searchId);
            else
                query = query.Where(p => p.Title.ToLower().Contains(search.ToLower()));
        }

        var proposals = query.Include(p => p.Attachments).ToList();

        ViewBag.Search = search ?? "";
        return View("ManagerCommence", proposals);
    }

    // Display details of a selected proposal
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
    // Method to set a selected proposal's status to 'Commenced'
    public async Task<IActionResult> Commence(int id)
    {
        var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
        if (proposal == null) return NotFound();

        // StatusId 4 is 'Commenced'
        proposal.StatusId = 4;
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
            Action = "commenced",
            Timestamp = DateTime.Now
        });

        await _context.SaveChangesAsync();

        var commenceTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        var message = $"Proposal <strong>#{proposal.Id} '{proposal.Title}'</strong> was set to commence. (<strong>{commenceTime}</strong>)";

        // Notify Committee Chair
        await _notificationService.CreateNotificationForRoleAsync(
            "Committee Chair", message, proposal.Id, "ProposalCommenced", userId);

        // Notify Ethics Committee
        await _notificationService.CreateNotificationForRoleAsync(
            "Ethics Committee", message, proposal.Id, "ProposalCommenced", userId);

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
