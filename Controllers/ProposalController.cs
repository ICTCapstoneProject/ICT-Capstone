using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FSSA.Models;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FSSA.Controllers
{
    public class ProposalController : Controller
    {
        private readonly ProjectManagerContext _context;

        public ProposalController(ProjectManagerContext context)
        {
            _context = context;
        }

        // GET: /Proposal/Create
        public IActionResult Create()
        {
            ViewBag.ProjectLevels = _context.ProjectLevels
                .Select(pl => new SelectListItem
                {
                    Value = pl.LevelId.ToString(),
                    Text = pl.LevelName
                })
                .ToList();

            return View();
        }

        

        // POST: /Proposal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Proposal proposal)
        {

            if (ModelState.IsValid)
            {
                proposal.StatusId = 1;              // Default to "New Proposal" status


                var Identity = User.Identity;
                if (Identity == null || !Identity.IsAuthenticated)
                {
                    return Unauthorized();
                }
                var email = Identity.Name;
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized();
                }

                var userId = _context.Users.FirstOrDefault(u => u.Email == email)?.UserId;

                if(userId == null)
                {
                    return Unauthorized();
                }

                proposal.SubmittedBy = userId.Value;

                proposal.CreatedAt = DateTime.Now;
                proposal.UpdatedAt = DateTime.Now;

                _context.Proposals.Add(proposal);
                _context.SaveChanges();
                _context.ProposalLogs.Add(new ProposalLog
                {
                    ProposalId = proposal.Id,
                    StatusId = proposal.StatusId,
                    ChangedBy = proposal.SubmittedBy,
                    Action = "submitted",
                    Timestamp = DateTime.Now
                });
                _context.SaveChanges();

                return RedirectToAction("Success", new { id = proposal.Id });
            }

            ViewBag.ProjectLevels = _context.ProjectLevels
                .Select(pl => new SelectListItem
                {
                    Value = pl.LevelId.ToString(),
                    Text = pl.LevelName
                })
                .ToList();

            return View(proposal);
        }

        // GET: /Proposal/Success/{id}
        public IActionResult Success(int id)
        {
            var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
            if (proposal == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == proposal.SubmittedBy);
            var level = _context.ProjectLevels.FirstOrDefault(pl => pl.LevelId == proposal.ProjectLevelId);

            ViewBag.SubmitterName = user?.Name ?? "Unknown User";
            ViewBag.LevelName = level?.LevelName ?? "Unknown Level";

            return View(proposal);
        }


        public IActionResult ViewProposals(bool showOnlyMine = false)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return Unauthorized();
            }

            var role = user.Role.ToLower();
            bool canToggle = role == "researcher" || role == "manager" || role == "assistant director";

            List<MyProposalViewModel> proposals;

            if (canToggle && showOnlyMine)
            {
                proposals = _context.Proposals
                    .Where(p => p.SubmittedBy == user.UserId)
                    .Join(_context.ProjectLevels,
                        p => p.ProjectLevelId,
                        pl => pl.LevelId,
                        (p, pl) => new { p, pl })
                    .Join(_context.Users,
                        combo => combo.p.SubmittedBy,
                        u => u.UserId,
                        (combo, u) => new { combo.p, combo.pl, u })
                    .Join(_context.Statuses,
                        combo => combo.p.StatusId,
                        s => s.StatusId,
                        (combo, s) => new MyProposalViewModel
                        {
                            Id = combo.p.Id,
                            Title = combo.p.Title,
                            Synopsis = combo.p.Synopsis,
                            ProjectLevel = combo.pl.LevelName,
                            EstimatedCompletionDate = combo.p.EstimatedCompletionDate,
                            SubmittedByName = combo.u.Name,
                            StatusName = s.StatusName
                        })
                    .ToList();
            }
            else
            {
                proposals = _context.Proposals
                    .Join(_context.ProjectLevels,
                        p => p.ProjectLevelId,
                        pl => pl.LevelId,
                        (p, pl) => new { p, pl })
                    .Join(_context.Users,
                        combo => combo.p.SubmittedBy,
                        u => u.UserId,
                        (combo, u) => new { combo.p, combo.pl, u })
                    .Join(_context.Statuses,
                        combo => combo.p.StatusId,
                        s => s.StatusId,
                        (combo, s) => new MyProposalViewModel
                        {
                            Id = combo.p.Id,
                            Title = combo.p.Title,
                            Synopsis = combo.p.Synopsis,
                            ProjectLevel = combo.pl.LevelName,
                            EstimatedCompletionDate = combo.p.EstimatedCompletionDate,
                            SubmittedByName = combo.u.Name,
                            StatusName = s.StatusName
                        })
                    .ToList();
            }

            ViewBag.Role = user.Role.ToLower();         // This is for controlling toggle visibility
            ViewBag.ShowingMine = showOnlyMine;         // And this is to determine the toggle state

            return View("ViewProposals", proposals);
        }


        public IActionResult Details(int id)
        {
            var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
            if (proposal == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == proposal.SubmittedBy);
            var projectLevel = _context.ProjectLevels
                                .FirstOrDefault(pl => pl.LevelId == proposal.ProjectLevelId)?.LevelName ?? "Unknown";

            var model = new MyProposalViewModel
            {
                Id = proposal.Id,
                Title = proposal.Title,
                Synopsis = proposal.Synopsis,
                Method = proposal.Method,
                ProjectLevel = projectLevel,
                Resources = proposal.Resources,
                EthicalConsiderations = proposal.EthicalConsiderations,
                Outcomes = proposal.Outcomes,
                Milestones = proposal.Milestones,
                EstimatedCompletionDate = proposal.EstimatedCompletionDate,
                SubmittedByName = user?.Name ?? "Unknown"
            };

            return View(model);
        }
        

        public IActionResult Edit(int id)
        {
            var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
            if (proposal == null)
            {
                return NotFound();
            }

            ViewBag.ProjectLevels = _context.ProjectLevels
                .Select(pl => new SelectListItem
                {
                    Value = pl.LevelId.ToString(),
                    Text = pl.LevelName
                })
                .ToList();

            ViewBag.CurrentLevelName = _context.ProjectLevels
                .FirstOrDefault(pl => pl.LevelId == proposal.ProjectLevelId)?.LevelName ?? "Unknown";

            return View(proposal);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Proposal updatedProposal)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedProposal);
            }

            var existingProposal = _context.Proposals.FirstOrDefault(p => p.Id == updatedProposal.Id);
            if (existingProposal == null)
            {
                return NotFound();
            }

            // Store old values first
            var originalClone = new Proposal
            {
                Title = existingProposal.Title,
                Synopsis = existingProposal.Synopsis,
                Method = existingProposal.Method,
                ProjectLevelId = existingProposal.ProjectLevelId,
                Resources = existingProposal.Resources,
                EthicalConsiderations = existingProposal.EthicalConsiderations,
                Outcomes = existingProposal.Outcomes,
                Milestones = existingProposal.Milestones,
                EstimatedCompletionDate = existingProposal.EstimatedCompletionDate,
                SubmittedBy = existingProposal.SubmittedBy
            };

            // Then update
            existingProposal.Title = updatedProposal.Title;
            existingProposal.Synopsis = updatedProposal.Synopsis;
            existingProposal.Method = updatedProposal.Method;
            existingProposal.ProjectLevelId = updatedProposal.ProjectLevelId;
            existingProposal.Resources = updatedProposal.Resources;
            existingProposal.EthicalConsiderations = updatedProposal.EthicalConsiderations;
            existingProposal.Outcomes = updatedProposal.Outcomes;
            existingProposal.Milestones = updatedProposal.Milestones;
            existingProposal.EstimatedCompletionDate = updatedProposal.EstimatedCompletionDate;
            existingProposal.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            var email = User.Identity?.Name;
            var userId = _context.Users.FirstOrDefault(u => u.Email == email)?.UserId ?? 0;

            _context.ProposalLogs.Add(new ProposalLog
            {
                ProposalId = existingProposal.Id,
                StatusId = existingProposal.StatusId,
                ChangedBy = userId,
                Action = "modified",
                Timestamp = DateTime.Now
            });
            _context.SaveChanges();

            var submitter = _context.Users.FirstOrDefault(u => u.UserId == existingProposal.SubmittedBy);
            var level = _context.ProjectLevels.FirstOrDefault(pl => pl.LevelId == existingProposal.ProjectLevelId);

            // Finally, use old states and pass through ViewBag
            ViewBag.Original = originalClone;
            ViewBag.OriginalLevel = _context.ProjectLevels.FirstOrDefault(pl => pl.LevelId == originalClone.ProjectLevelId)?.LevelName ?? "Unknown";
            ViewBag.SubmitterName = submitter?.Name ?? "Unknown";
            ViewBag.LevelName = level?.LevelName ?? "Unknown";

            return View("EditSuccess", existingProposal);
        }

        public IActionResult EditSuccess(int id)
        {
            var updated = _context.Proposals.FirstOrDefault(p => p.Id == id);
            if (updated == null) return NotFound();

            var originalLog = _context.ProposalLogs
                .Where(log => log.ProposalId == id && log.Action == "submitted")
                .OrderBy(log => log.Timestamp)
                .FirstOrDefault();

            var original = originalLog != null
                ? _context.Proposals.AsNoTracking().FirstOrDefault(p => p.Id == id)
                : updated;

            var user = _context.Users.FirstOrDefault(u => u.UserId == updated.SubmittedBy);
            var level = _context.ProjectLevels.FirstOrDefault(pl => pl.LevelId == updated.ProjectLevelId);
            var origLevel = _context.ProjectLevels.FirstOrDefault(pl => pl.LevelId == original.ProjectLevelId);

            ViewBag.SubmitterName = user?.Name ?? "Unknown";
            ViewBag.LevelName = level?.LevelName ?? "Unknown";
            ViewBag.Original = original;
            ViewBag.OriginalLevel = origLevel?.LevelName ?? "Unknown";

            return View("EditSuccess", updated);
        }

        public IActionResult Search(
            List<string> StatusFilters,
            List<string> ProjectLevelFilters,
            DateTime? StartDate,
            DateTime? EndDate,
            string SearchKeyword,
            string SortBy,
            int page = 1,
            int pageSize = 10)
        {
            ViewBag.ProjectLevels = _context.ProjectLevels
                .Select(pl => new { pl.LevelId, pl.LevelName })
                .ToList();

            ViewBag.Statuses = _context.Statuses
                .Select(s => new { s.StatusId, s.StatusName })
                .ToList();

            var query = _context.Proposals
                .Join(_context.ProjectLevels,
                    p => p.ProjectLevelId,
                    pl => pl.LevelId,
                    (p, pl) => new { p, pl })
                .Join(_context.Users,
                    combo => combo.p.SubmittedBy,
                    u => u.UserId,
                    (combo, u) => new { combo.p, combo.pl, u })
                .Join(_context.Statuses,
                    combo => combo.p.StatusId,
                    s => s.StatusId,
                    (combo, s) => new MyProposalViewModel
                    {
                        Id = combo.p.Id,
                        Title = combo.p.Title,
                        Synopsis = combo.p.Synopsis,
                        Method = combo.p.Method,
                        ProjectLevel = combo.pl.LevelName,
                        Resources = combo.p.Resources,
                        EthicalConsiderations = combo.p.EthicalConsiderations,
                        Outcomes = combo.p.Outcomes,
                        Milestones = combo.p.Milestones,
                        EstimatedCompletionDate = combo.p.EstimatedCompletionDate,
                        SubmittedByName = combo.u.Name,
                        StatusName = s.StatusName
                    });

            if (ProjectLevelFilters != null && ProjectLevelFilters.Any())
                query = query.Where(p => ProjectLevelFilters.Contains(p.ProjectLevel));

            // Status Filters
            if (StatusFilters != null && StatusFilters.Any())
                query = query.Where(p => StatusFilters.Contains(p.StatusName));

            // Date Filters
            if (StartDate.HasValue)
                query = query.Where(p => p.EstimatedCompletionDate >= StartDate.Value);
            if (EndDate.HasValue)
                query = query.Where(p => p.EstimatedCompletionDate <= EndDate.Value);

        
            // Search Keyword
            if (!string.IsNullOrEmpty(SearchKeyword))
                query = query.Where(p => p.Title.Contains(SearchKeyword));

            // Sort
            switch (SortBy)
            {
                case "Title":
                    query = query.OrderBy(p => p.Title);
                    break;
                default:
                    query = query.OrderByDescending(p => p.EstimatedCompletionDate);
                    break;
            }

            // Pagination
            int totalCount = query.Count();
            var pagedProposals = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new ProposalSearchViewModel
            {
                Proposals = pagedProposals,
                TotalCount = totalCount,
                PageIndex = page,
                PageSize = pageSize,
                StatusFilters = StatusFilters ?? new List<string>(),
                ProjectLevelFilters = ProjectLevelFilters ?? new List<string>(),
                StartDate = StartDate,
                EndDate = EndDate,
                SearchKeyword = SearchKeyword,
                SortBy = SortBy
            };

            return View(viewModel);
        }
    }
}
