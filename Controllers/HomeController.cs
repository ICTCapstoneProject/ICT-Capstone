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

    public IActionResult Index()
    {
        var totalProposals = _context.Proposals.Count();
        // not sure
        var pendingApprovals = _context.Proposals.Count(p => p.StatusId == 1); 
        var recentlyReviewed = _context.Proposals.Count(p => p.StatusId == 2 && p.UpdatedAt >= DateTime.Now.AddDays(-7)); 
        var recentlyCommented = _context.Comments.Count(c => c.Timestamp >= DateTime.Now.AddDays(-7));

        var recentActivities = _context.Proposals
            .OrderByDescending(p => p.CreatedAt)
            .Take(3)
            .Select(p => new DashboardActivity
            {
                Description = $"Proposal #{p.Id} submitted by UserId {p.SubmittedBy}",
                TimeAgo = (DateTime.Now - p.CreatedAt).TotalHours < 1 ? "just now" : $"{(int)(DateTime.Now - p.CreatedAt).TotalHours} hours ago"
            }).ToList();

        var model = new DashboardViewModel
        {
            TotalProposals = totalProposals,
            PendingApprovals = pendingApprovals,
            RecentlyReviewed = recentlyReviewed,
            RecentlyCommented = recentlyCommented,
            RecentActivities = recentActivities
        };

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
