using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerMvc.Models;
using FSSA.Models; 

namespace ProjectManagerMvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProjectManagerContext _context; 

    public HomeController(ILogger<HomeController> logger, ProjectManagerContext context) 
    {
        _logger = logger;
        _context = context;
    }

    private string GetTimeAgo(DateTime createdAt)
    {
        var span = DateTime.Now - createdAt;

        if (span.TotalMinutes < 1)
            return "just now";

        if (span.TotalMinutes < 60)
        {
            int minutes = (int)span.TotalMinutes;
            return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
        }

        if (span.TotalHours < 24)
        {
            int hours = (int)span.TotalHours;
            return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
        }

        if (span.TotalDays < 7)
        {
            int days = (int)span.TotalDays;
            return days == 1 ? "1 day ago" : $"{days} days ago";
        }

        if (span.TotalDays < 30)
        {
            int weeks = (int)(span.TotalDays / 7);
            return weeks == 1 ? "1 week ago" : $"{weeks} weeks ago";
        }

        if (span.TotalDays < 365)
        {
            int months = (int)(span.TotalDays / 30);
            return months == 1 ? "1 month ago" : $"{months} months ago";
        }

        int years = (int)(span.TotalDays / 365);
        return years == 1 ? "1 year ago" : $"{years} years ago";
    }

    public IActionResult Index()
    {
        var totalProposals = _context.Proposals.Count();
        var pendingApprovals = _context.Proposals.Count(p => p.StatusId == 1);
        var recentlyReviewed = _context.Proposals.Count(p => p.StatusId == 2 && p.UpdatedAt >= DateTime.Now.AddDays(-7));
        var recentlyCommented = _context.Comments.Count(c => c.Timestamp >= DateTime.Now.AddDays(-7));

        var recentActivities = _context.ProposalLogs
            .OrderByDescending(log => log.Timestamp)
            .Take(3)
            .Join(_context.Proposals,
                log => log.ProposalId,
                p => p.Id,
                (log, p) => new { log, p })
            .Join(_context.Users,
                combo => combo.log.ChangedBy,
                u => u.UserId,
                (combo, u) => new
                {
                    ProposalId = combo.p.Id,
                    combo.p.Title,
                    combo.log.Action,
                    combo.log.Timestamp,
                    UserName = u.Name
                })
            .AsEnumerable()
            .Select(x => new DashboardActivity
            {
                Description = x.Action switch
                {
                    "submitted"         => $"Proposal #{x.ProposalId}, <strong>'{x.Title}'</strong> was <span class='text-success' style='font-weight:600;'>Submitted</span> by {x.UserName}",
                    "modified"          => $"Proposal #{x.ProposalId}, <strong>'{x.Title}'</strong> was <span class='text-warning' style='font-weight:600;'>Modified</span> by {x.UserName}",
                    "approve"           => $"Proposal #{x.ProposalId}, <strong>'{x.Title}'</strong> was granted <span style='color:rgb(6, 55, 103); font-weight:600;'>Committee Approval</span> by {x.UserName}",
                    "chair_approve"     => $"Proposal #{x.ProposalId}, <strong>'{x.Title}'</strong> was granted <span class='text-primary' style='font-weight:600;'>Chair Approval</span> by {x.UserName}",
                    "marked_complete"   => $"Proposal #{x.ProposalId}, <strong>'{x.Title}'</strong> was <span style='color:rgb(38, 99, 103); font-weight:600;'>Marked Complete</span> by {x.UserName}",
                    "commenced"         => $"Proposal #{x.ProposalId}, <strong>'{x.Title}'</strong> was set to <span style='color:rgb(89, 108, 230); font-weight:600;'>Commenced</span> by {x.UserName}",
                    "requestModification"=> $"Proposal #{x.ProposalId}, <strong>'{x.Title}'</strong> was <span style='color:#ff8800; font-weight:600;'>Requested Revision</span> by {x.UserName}",
                    "request_additional_review"  => $"Proposal #{x.ProposalId}, <strong>'{x.Title}'</strong> was <span style='color:#495057; font-weight:600;'>Requested Additional Review</span> by {x.UserName}",
                    "reject"            => $"Proposal #{x.ProposalId}, <strong>'{x.Title}'</strong> was <span class='text-danger' style='font-weight:600;'>Rejected</span> by {x.UserName}",
                    _                   => $"Proposal #{x.ProposalId}, <strong>'{x.Title}'</strong> was updated by {x.UserName}"
                },
                TimeAgo = GetTimeAgo(x.Timestamp),
                
            })
            .ToList();
        var model = new DashboardViewModel
        {
            TotalProposals = totalProposals,
            PendingApprovals = pendingApprovals,
            RecentlyReviewed = recentlyReviewed,
            RecentlyCommented = recentlyCommented,
            RecentActivities = recentActivities
        };

        var email = User.Identity?.Name;
        var userName = _context.Users.FirstOrDefault(u => u.Email == email)?.Name ?? "Guest";

        ViewBag.UserName = userName;

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
