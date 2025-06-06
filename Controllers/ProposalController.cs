using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FSSA.Models;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FSSA.DTOs;
using ProjectManagerMvc.Services;

namespace FSSA.DTOs
{
    public class FinancialResourceDto
    {
        public string Title { get; set; }
        public double Cost { get; set; }
    }

    public class CoResearcherDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class AttachmentDto
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string TypeName { get; set; }
        public int Id { get; set; }
    }

    public class CommentDto
    {
        public int CommentId { get; set; }
        public string Author { get; set; }
        public DateTime Timestamp { get; set; }
        public string CommentText { get; set; }
    }
}

namespace FSSA.Controllers
{
    public class ProposalController : Controller
    {
        private readonly ProjectManagerContext _context;
        private const int MethodImageTypeId = 1;
        private readonly IWebHostEnvironment _environment;
        private readonly INotificationService _notificationService;

        public ProposalController(ProjectManagerContext context, IWebHostEnvironment environment, INotificationService notificationService)
        {
            _context = context;
            _environment = environment;
            _notificationService = notificationService;
        }

        private string BuildAttachmentFileName(int proposalId, string proposalTitle, string attachmentType, string ext)
        {
            // Remove/replace unsafe characters
            var safeTitle = string.Concat(proposalTitle.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
            var safeType = string.Concat(attachmentType.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
            return $"{proposalId} {safeTitle} {safeType} Attachment{ext}";
        }

        // GET: /Proposal/Create
        public IActionResult Create()
        {
            ViewBag.ProjectLevels = _context.ProjectLevels
                .Select(pl => new SelectListItem
                {
                    Value = pl.LevelId.ToString(),
                    Text = pl.LevelName
                }).ToList();

            var researcherRoleId = _context.Roles.FirstOrDefault(r => r.RoleName.ToLower() == "researcher")?.RoleId;

            if (researcherRoleId != null)
            {
                ViewBag.Researchers = _context.UserRoles
                    .Where(ur => ur.RoleId == researcherRoleId)
                    .Include(ur => ur.User)
                    .Select(ur => new SelectListItem
                    {
                        Value = ur.User.UserId.ToString(),
                        Text = ur.User.Name + " (#" + ur.User.UserId + ")"
                    })
                    .OrderBy(r => r.Text)
                    .ToList();
            }
            else
            {
                ViewBag.Researchers = new List<SelectListItem>();
            }

            return View();
        }



        // POST: /Proposal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
         Proposal proposal,
         [FromForm] IFormFile? SynopsisAttachment,
         [FromForm] IFormFile? MethodImage,
         [FromForm] IFormFile? EthicsAttachment,
         [FromForm] List<int> CoResearchers,
         [FromForm] List<string> ResourceTitles,
         [FromForm] List<decimal> ResourceCosts,
         [FromForm] int ResearcherId
     )
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelStateEntry in ModelState)
                {
                    foreach (var error in modelStateEntry.Value.Errors)
                    {
                        Console.WriteLine($"Model error in {modelStateEntry.Key}: {error.ErrorMessage}");
                    }
                }
                ViewBag.ProjectLevels = _context.ProjectLevels
                    .Select(pl => new SelectListItem
                    {
                        Value = pl.LevelId.ToString(),
                        Text = pl.LevelName
                    }).ToList();

                var researcherRoleId = _context.Roles.FirstOrDefault(r => r.RoleName.ToLower() == "researcher")?.RoleId;

                ViewBag.Researchers = (researcherRoleId != null)
                    ? _context.UserRoles
                        .Where(ur => ur.RoleId == researcherRoleId)
                        .Include(ur => ur.User)
                        .Select(ur => new SelectListItem
                        {
                            Value = ur.User.UserId.ToString(),
                            Text = ur.User.Name + " (#" + ur.User.UserId + ")"
                        })
                        .OrderBy(r => r.Text)
                        .ToList()
                    : new List<SelectListItem>();

                return View(proposal);
            }

            var identity = User.Identity;
            if (identity == null || !identity.IsAuthenticated)
                return Unauthorized();

            var email = identity.Name;
            var userId = _context.Users.FirstOrDefault(u => u.Email == email)?.UserId;
            if (userId == null)
                return Unauthorized();

            proposal.StatusId = 1;
            proposal.SubmittedBy = userId.Value;
            proposal.CreatedAt = DateTime.Now;
            proposal.UpdatedAt = DateTime.Now;

            proposal.LeadResearcherId = ResearcherId;

            _context.Proposals.Add(proposal);
            await _context.SaveChangesAsync();

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // Helper method to save attachment
            void SaveAttachment(IFormFile file, int typeId, string typeLabel)
            {
                if (file != null && file.Length > 0)
                {
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = BuildAttachmentFileName(proposal.Id, proposal.Title, typeLabel, ext);
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    _context.Attachments.Add(new Attachment
                    {
                        ProposalId = proposal.Id,
                        FileName = fileName,
                        FileUrl = "/uploads/" + fileName,
                        TypeId = typeId
                    });
                }
            }

            // Save each one with the correct type and label
            SaveAttachment(SynopsisAttachment, 1, "Synopsis");
            SaveAttachment(MethodImage, 2, "Method");
            SaveAttachment(EthicsAttachment, 3, "Ethics");

            foreach (var coResearcherId in CoResearchers.Distinct())
            {
                _context.ProposalResearchers.Add(new ProposalResearcher
                {
                    ProposalId = proposal.Id,
                    UserId = coResearcherId
                });
            }

            for (int i = 0; i < ResourceTitles.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(ResourceTitles[i]))
                {
                    _context.FinancialResources.Add(new FinancialResource
                    {
                        ProposalId = proposal.Id,
                        Title = ResourceTitles[i],
                        Cost = i < ResourceCosts.Count ? ResourceCosts[i] : 0
                    });
                }
            }

            _context.ProposalLogs.Add(new ProposalLog
            {
                ProposalId = proposal.Id,
                StatusId = proposal.StatusId,
                ChangedBy = proposal.SubmittedBy,
                Action = "submitted",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
            // Send notification
            var submitter = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            var submitterName = submitter?.Name ?? "Unknown User";
            await _notificationService.CreateNotificationForRoleAsync(
                "Committee Chair",
                $"Proposal '{proposal.Title}' was created by {submitterName} and requires approval.",
                proposal.Id,
                "ProposalSubmission",
                userId.Value
            );
            await _notificationService.CreateNotificationForRoleAsync(
                "Ethics Committee",
                $"Proposal '{proposal.Title}' was created by {submitterName} and requires approval.",
                proposal.Id,
                "ProposalSubmission",
                userId.Value
            );

            return RedirectToAction("Success", new { id = proposal.Id });
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

            // Fetch all attachments joined with their type names
            var attachments = _context.Attachments
                .Where(a => a.ProposalId == id)
                .Join(_context.AttachmentTypes,
                    a => a.TypeId,
                    t => t.TypeId,
                    (a, t) => new AttachmentDto
                    {
                        FileName = a.FileName,
                        FileUrl = a.FileUrl,
                        TypeName = t.TypeName
                    })
                .ToList();

            // Financial resources
            var financialResources = _context.FinancialResources
                .Where(fr => fr.ProposalId == id)
                .Select(fr => new FinancialResourceDto
                {
                    Title = fr.Title,
                    Cost = (double)fr.Cost
                }).ToList();

            // Co-researchers
            var coResearchers = _context.ProposalResearchers
                .Where(pr => pr.ProposalId == id)
                .Join(_context.Users,
                    pr => pr.UserId,
                    u => u.UserId,
                    (pr, u) => new CoResearcherDto
                    {
                        Id = u.UserId.ToString(),
                        Name = u.Name
                    })
                .ToList();
            var leadResearcher = _context.Users.FirstOrDefault(u => u.UserId == proposal.LeadResearcherId);

            ViewBag.SubmitterName = user?.Name ?? "Unknown User";
            ViewBag.LevelName = level?.LevelName ?? "Unknown Level";
            ViewBag.LeadResearcherName = leadResearcher?.Name ?? "Unknown";
            ViewBag.Attachments = attachments;
            ViewBag.FinancialResources = financialResources;
            ViewBag.CoResearchers = coResearchers;

            return View(proposal);
        }


        public IActionResult ViewProposals(bool showOnlyMine = false)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return Unauthorized();

            bool canToggle = User.IsInRole("researcher") || User.IsInRole("manager") || User.IsInRole("assistant director");

            var baseQuery = _context.Proposals
                .Where(p => !showOnlyMine || p.SubmittedBy == user.UserId)
                .Include(p => p.Attachments)
                .Join(_context.ProjectLevels,
                    p => p.ProjectLevelId,
                    pl => pl.LevelId,
                    (p, pl) => new { p, ProjectLevelName = pl.LevelName })
                .Join(_context.Users,
                    combo => combo.p.SubmittedBy,
                    u => u.UserId,
                    (combo, u) => new { combo.p, combo.ProjectLevelName, SubmitterName = u.Name })
                .Join(_context.Statuses,
                    combo => combo.p.StatusId,
                    s => s.StatusId,
                    (combo, s) => new { combo.p, combo.ProjectLevelName, combo.SubmitterName, StatusName = s.StatusName });

            var proposals = baseQuery
                .AsEnumerable()
                .Select(x => new MyProposalViewModel
                {
                    Id = x.p.Id,
                    Title = x.p.Title,
                    Synopsis = x.p.Synopsis,
                    Method = x.p.Method,
                    ProjectLevel = x.ProjectLevelName,
                    PhysicalResources = x.p.PhysicalResources,
                    EthicalConsiderations = x.p.EthicalConsiderations,
                    Outcomes = x.p.Outcomes,
                    Milestones = x.p.Milestones,
                    EstimatedCompletionDate = x.p.EstimatedCompletionDate,
                    SubmittedByName = x.SubmitterName,
                    StatusName = x.StatusName,
                    Attachments = x.p.Attachments.ToList(),
                    FinancialResources = _context.FinancialResources
                        .Where(fr => fr.ProposalId == x.p.Id)
                        .Select(fr => new FinancialResourceDto
                        {
                            Title = fr.Title,
                            Cost = (double)fr.Cost
                        }).ToList(),
                    LeadResearcherName = _context.Users
                        .FirstOrDefault(u => u.UserId == x.p.LeadResearcherId)?.Name ?? "Unknown",
                    CoResearchers = _context.ProposalResearchers
                        .Where(pr => pr.ProposalId == x.p.Id)
                        .Join(_context.Users,
                            pr => pr.UserId,
                            u => u.UserId,
                            (pr, u) => new CoResearcherDto
                            {
                                Id = u.UserId.ToString(),
                                Name = u.Name
                            }).ToList()
                }).ToList();

            ViewBag.canToggle = canToggle;
            ViewBag.ShowingMine = showOnlyMine;

            return View("ViewProposals", proposals);
        }


        public IActionResult Details(int id)
        {
            var proposal = _context.Proposals
                .Include(p => p.Attachments)
                .FirstOrDefault(p => p.Id == id);

            if (proposal == null)
            {
                return NotFound();
            }

            var submitter = _context.Users.FirstOrDefault(u => u.UserId == proposal.SubmittedBy);
            var leadResearcher = _context.Users.FirstOrDefault(u => u.UserId == proposal.LeadResearcherId);
            var currentUser = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            var userRoleIds = _context.UserRoles
                .Where(ur => ur.UserId == currentUser.UserId)
                .Select(ur => ur.RoleId)
                .ToList();

            // Pass current user data
            ViewBag.CurrentUserId = currentUser.UserId;
            ViewBag.CurrentUserRoleIds = userRoleIds;
            ViewBag.SubmitterId = proposal.SubmittedBy;

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
                Attachments = proposal.Attachments.ToList()
            };

            return View(model);
        }


        public IActionResult Edit(int id)
        {
            var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
            if (proposal == null)
                return NotFound();

            // Add ProjectLevels
            ViewBag.ProjectLevels = _context.ProjectLevels
                .Select(pl => new SelectListItem
                {
                    Value = pl.LevelId.ToString(),
                    Text = pl.LevelName
                }).ToList();

            // Add Researchers list
            var researcherRoleId = _context.Roles.FirstOrDefault(r => r.RoleName.ToLower() == "researcher")?.RoleId;
            if (researcherRoleId != null)
            {
                ViewBag.Researchers = _context.UserRoles
                    .Where(ur => ur.RoleId == researcherRoleId)
                    .Include(ur => ur.User)
                    .Select(ur => new SelectListItem
                    {
                        Value = ur.User.UserId.ToString(),
                        Text = ur.User.Name + " (#" + ur.User.UserId + ")"
                    })
                    .OrderBy(r => r.Text)
                    .ToList();
            }

            // Add OriginalProposal
            ViewBag.OriginalProposal = new Proposal
            {
                Title = proposal.Title,
                Synopsis = proposal.Synopsis,
                Method = proposal.Method,
                ProjectLevelId = proposal.ProjectLevelId,
                PhysicalResources = proposal.PhysicalResources,
                EthicalConsiderations = proposal.EthicalConsiderations,
                Outcomes = proposal.Outcomes,
                Milestones = proposal.Milestones,
                EstimatedCompletionDate = proposal.EstimatedCompletionDate,
                LeadResearcherId = proposal.LeadResearcherId
            };

            // Financial Resources
            ViewBag.OriginalFinancialResources = _context.FinancialResources
                .Where(fr => fr.ProposalId == id)
                .Select(fr => new FinancialResourceDto
                {
                    Title = fr.Title,
                    Cost = (double)fr.Cost
                }).ToList();

            // Co-Researchers
            ViewBag.OriginalCoResearchers = _context.ProposalResearchers
                .Where(pr => pr.ProposalId == id)
                .Join(_context.Users,
                    pr => pr.UserId,
                    u => u.UserId,
                    (pr, u) => new CoResearcherDto
                    {
                        Id = u.UserId.ToString(),
                        Name = u.Name
                    }).ToList();

            // Attachments
            ViewBag.OriginalAttachments = _context.Attachments
                .Where(a => a.ProposalId == id)
                .Join(_context.AttachmentTypes,
                    a => a.TypeId,
                    t => t.TypeId,
                    (a, t) => new AttachmentDto
                    {
                        FileName = a.FileName,
                        FileUrl = a.FileUrl,
                        TypeName = t.TypeName
                    }).ToList();

            // Project Level Name
            ViewBag.OriginalProjectLevel = _context.ProjectLevels
                .Where(pl => pl.LevelId == proposal.ProjectLevelId)
                .Select(pl => pl.LevelName)
                .FirstOrDefault() ?? "N/A";

            // Lead Researcher Name
            ViewBag.OriginalLeadResearcher = _context.Users
                .Where(u => u.UserId == proposal.LeadResearcherId)
                .Select(u => u.Name)
                .FirstOrDefault() ?? "N/A";

            return View(proposal);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,Synopsis,Method,ProjectLevelId,LeadResearcherId,PhysicalResources,EthicalConsiderations,Outcomes,Milestones,EstimatedCompletionDate,StatusId")] Proposal model,
            int[] CoResearchers,
            string[] ResourceTitles,
            decimal[] ResourceCosts,
            IFormFile? SynopsisAttachment,
            IFormFile? MethodImage,
            IFormFile? EthicsAttachment,
            bool RemoveSynopsisAttachment = false,
            bool RemoveMethodImage = false,
            bool RemoveEthicsAttachment = false
        )
        {

            if (id != model.Id)
                return NotFound();

            // To avoid null reference nonsense
            CoResearchers ??= Array.Empty<int>();
            ResourceTitles ??= Array.Empty<string>();
            ResourceCosts ??= Array.Empty<decimal>();

            // Get original proposal for side-by-side comparison
            var originalProposal = await _context.Proposals.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            if (originalProposal == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Validation error in {entry.Key}: {error.ErrorMessage}");
                    }
                }
                // Inline ViewBag assignment (copy-paste from Edit GET for ModelState errors)
                ViewBag.ProjectLevels = _context.ProjectLevels
            .Select(pl => new SelectListItem
            {
                Value = pl.LevelId.ToString(),
                Text = pl.LevelName
            }).ToList();

                var researcherRoleId = _context.Roles.FirstOrDefault(r => r.RoleName.ToLower() == "researcher")?.RoleId;
                ViewBag.Researchers = (researcherRoleId != null)
                    ? _context.UserRoles
                        .Where(ur => ur.RoleId == researcherRoleId)
                        .Include(ur => ur.User)
                        .Select(ur => new SelectListItem
                        {
                            Value = ur.User.UserId.ToString(),
                            Text = ur.User.Name + " (#" + ur.User.UserId + ")"
                        })
                        .OrderBy(r => r.Text)
                        .ToList()
                    : new List<SelectListItem>();

                ViewBag.OriginalProposal = new Proposal
                {
                    Title = originalProposal.Title,
                    Synopsis = originalProposal.Synopsis,
                    Method = originalProposal.Method,
                    ProjectLevelId = originalProposal.ProjectLevelId,
                    PhysicalResources = originalProposal.PhysicalResources,
                    EthicalConsiderations = originalProposal.EthicalConsiderations,
                    Outcomes = originalProposal.Outcomes,
                    Milestones = originalProposal.Milestones,
                    EstimatedCompletionDate = originalProposal.EstimatedCompletionDate,
                    LeadResearcherId = originalProposal.LeadResearcherId
                };
                ViewBag.OriginalFinancialResources = _context.FinancialResources
                    .Where(fr => fr.ProposalId == id)
                    .Select(fr => new FinancialResourceDto
                    {
                        Title = fr.Title,
                        Cost = (double)fr.Cost
                    }).ToList();

                ViewBag.OriginalCoResearchers = _context.ProposalResearchers
                    .Where(pr => pr.ProposalId == id)
                    .Join(_context.Users,
                        pr => pr.UserId,
                        u => u.UserId,
                        (pr, u) => new CoResearcherDto
                        {
                            Id = u.UserId.ToString(),
                            Name = u.Name
                        }).ToList();

                ViewBag.OriginalAttachments = _context.Attachments
                    .Where(a => a.ProposalId == id)
                    .Join(_context.AttachmentTypes,
                        a => a.TypeId,
                        t => t.TypeId,
                        (a, t) => new AttachmentDto
                        {
                            FileName = a.FileName,
                            FileUrl = a.FileUrl,
                            TypeName = t.TypeName
                        }).ToList();

                ViewBag.OriginalProjectLevel = _context.ProjectLevels
                    .Where(pl => pl.LevelId == originalProposal.ProjectLevelId)
                    .Select(pl => pl.LevelName)
                    .FirstOrDefault() ?? "N/A";

                ViewBag.OriginalLeadResearcher = _context.Users
                    .Where(u => u.UserId == originalProposal.LeadResearcherId)
                    .Select(u => u.Name)
                    .FirstOrDefault() ?? "N/A";

                return View(model);
            }

            // Get proposal for update
            var proposal = await _context.Proposals
                .Include(p => p.Attachments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proposal == null)
                return NotFound();

            // Update proposal fields
            proposal.Title = model.Title;
            proposal.Synopsis = model.Synopsis;
            proposal.Method = model.Method;
            proposal.ProjectLevelId = model.ProjectLevelId;
            proposal.LeadResearcherId = model.LeadResearcherId;
            proposal.PhysicalResources = model.PhysicalResources;
            proposal.EthicalConsiderations = model.EthicalConsiderations;
            proposal.Outcomes = model.Outcomes;
            proposal.Milestones = model.Milestones;
            proposal.EstimatedCompletionDate = model.EstimatedCompletionDate;
            proposal.StatusId = 1;

            // Update ProposalResearchers (Co-Researchers)
            var existingProposalResearchers = _context.ProposalResearchers.Where(pr => pr.ProposalId == proposal.Id);
            _context.ProposalResearchers.RemoveRange(existingProposalResearchers);
            foreach (var coResearcherId in CoResearchers.Distinct())
            {
                _context.ProposalResearchers.Add(new ProposalResearcher
                {
                    ProposalId = proposal.Id,
                    UserId = coResearcherId
                });
            }

            // Update Financial Resources
            var oldResources = _context.FinancialResources.Where(fr => fr.ProposalId == proposal.Id);
            _context.FinancialResources.RemoveRange(oldResources);
            for (int i = 0; i < ResourceTitles.Length && i < ResourceCosts.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(ResourceTitles[i]))
                {
                    _context.FinancialResources.Add(new FinancialResource
                    {
                        ProposalId = proposal.Id,
                        Title = ResourceTitles[i],
                        Cost = ResourceCosts[i]
                    });
                }
            }

            //  Update Attachments (helper for each type)
            async Task SaveAttachmentAsync(IFormFile file, int typeId, string typeLabel)
            {
                if (file == null || file.Length == 0)
                    return;

                var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var ext = Path.GetExtension(file.FileName);
                var fileName = BuildAttachmentFileName(proposal.Id, proposal.Title, typeLabel, ext);
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var old = proposal.Attachments.FirstOrDefault(a => a.TypeId == typeId);
                if (old != null)
                    _context.Attachments.Remove(old);

                proposal.Attachments.Add(new Attachment
                {
                    ProposalId = proposal.Id,
                    TypeId = typeId,
                    FileName = fileName,
                    FileUrl = "/uploads/" + fileName
                });
            }
            void RemoveAttachmentOfType(int typeId)
            {
                var old = proposal.Attachments.FirstOrDefault(a => a.TypeId == typeId);
                if (old != null)
                    _context.Attachments.Remove(old);
            }

            // Remove if toggled
            if (RemoveSynopsisAttachment) RemoveAttachmentOfType(1);
            if (RemoveMethodImage) RemoveAttachmentOfType(2);
            if (RemoveEthicsAttachment) RemoveAttachmentOfType(3);

            // Save new files if uploaded
            await SaveAttachmentAsync(SynopsisAttachment, 1, "Synopsis");
            await SaveAttachmentAsync(MethodImage, 2, "Method");
            await SaveAttachmentAsync(EthicsAttachment, 3, "Ethics");

            // --- Log this edit action
            var userId = GetCurrentUserId();
            _context.ProposalLogs.Add(new ProposalLog
            {
                ProposalId = proposal.Id,
                StatusId = proposal.StatusId,
                ChangedBy = userId,
                Action = "modified",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            // Notify the chair
            await _notificationService.CreateNotificationForRoleAsync(
                "Committee Chair",
                $"Proposal '{proposal.Title}' has been modified and requires re-approval.",
                proposal.Id,
                "ProposalModification",
                userId
            );

            //  Set up ViewBags for EditSuccess (side-by-side compare)
            await PopulateEditSuccessViewBags(originalProposal, proposal);

            // Show comparison of changes
            return View("EditSuccess", proposal);
        }

        // Helper: Get the current user's UserId
        private int GetCurrentUserId()
        {
            var email = User.Identity?.Name;
            return _context.Users.FirstOrDefault(u => u.Email == email)?.UserId ?? 0;
        }


        private async Task PopulateEditSuccessViewBags(Proposal orig, Proposal updated)
        {
            // Original values
            ViewBag.OrigProjectLevelName = await _context.ProjectLevels
                .Where(x => x.LevelId == orig.ProjectLevelId).Select(x => x.LevelName).FirstOrDefaultAsync() ?? "N/A";
            ViewBag.OrigLeadResearcherName = await _context.Users
                .Where(x => x.UserId == orig.LeadResearcherId).Select(x => x.Name).FirstOrDefaultAsync() ?? "N/A";
            ViewBag.OrigCoResearcherNames = await _context.ProposalResearchers
                .Where(pr => pr.ProposalId == orig.Id)
                .Join(_context.Users, pr => pr.UserId, u => u.UserId, (pr, u) => u.Name)
                .ToListAsync();

            ViewBag.OrigFinancialResources = await _context.FinancialResources
                .Where(x => x.ProposalId == orig.Id)
                .Select(fr => new FinancialResourceDto
                {
                    Title = fr.Title,
                    Cost = (double)fr.Cost
                }).ToListAsync();

            ViewBag.OrigAttachments = await _context.Attachments
                .Where(a => a.ProposalId == orig.Id)
                .Join(_context.AttachmentTypes, a => a.TypeId, t => t.TypeId,
                    (a, t) => new AttachmentDto
                    {
                        FileName = a.FileName,
                        FileUrl = a.FileUrl,
                        TypeName = t.TypeName
                    }).ToListAsync();

            // Updated values
            ViewBag.UpdatedProjectLevelName = await _context.ProjectLevels
                .Where(x => x.LevelId == updated.ProjectLevelId).Select(x => x.LevelName).FirstOrDefaultAsync() ?? "N/A";
            ViewBag.UpdatedLeadResearcherName = await _context.Users
                .Where(x => x.UserId == updated.LeadResearcherId).Select(x => x.Name).FirstOrDefaultAsync() ?? "N/A";
            ViewBag.UpdatedCoResearcherNames = await _context.ProposalResearchers
                .Where(pr => pr.ProposalId == updated.Id)
                .Join(_context.Users, pr => pr.UserId, u => u.UserId, (pr, u) => u.Name)
                .ToListAsync();

            ViewBag.UpdatedFinancialResources = await _context.FinancialResources
                .Where(x => x.ProposalId == updated.Id)
                .Select(fr => new FinancialResourceDto
                {
                    Title = fr.Title,
                    Cost = (double)fr.Cost
                }).ToListAsync();

            ViewBag.UpdatedAttachments = await _context.Attachments
                .Where(a => a.ProposalId == updated.Id)
                .Join(_context.AttachmentTypes, a => a.TypeId, t => t.TypeId,
                    (a, t) => new AttachmentDto
                    {
                        FileName = a.FileName,
                        FileUrl = a.FileUrl,
                        TypeName = t.TypeName
                    }).ToListAsync();

            // Human readable submitter
            ViewBag.SubmitterName = await _context.Users
                .Where(x => x.UserId == updated.LeadResearcherId).Select(x => x.Name).FirstOrDefaultAsync() ?? "N/A";
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
                    (combo, s) => new { combo.p, combo.pl, combo.u, s })
                .ToList()
                .Select(entry => new MyProposalViewModel
                {
                    Id = entry.p.Id,
                    Title = entry.p.Title,
                    Synopsis = entry.p.Synopsis,
                    Method = entry.p.Method,
                    MethodImage = _context.Attachments
                    .Where(a => a.ProposalId == entry.p.Id && a.TypeId == MethodImageTypeId)
                    .Select(a => a.FileUrl)
                    .FirstOrDefault(),
                    ProjectLevel = entry.pl.LevelName,
                    PhysicalResources = entry.p.PhysicalResources,
                    FinancialResources = _context.FinancialResources
                    .Where(fr => fr.ProposalId == entry.p.Id)
                    .Select(fr => new FinancialResourceDto
                    {
                        Title = fr.Title,
                        Cost = (double)fr.Cost
                    }).ToList(),
                    EthicalConsiderations = entry.p.EthicalConsiderations,
                    Outcomes = entry.p.Outcomes,
                    Milestones = entry.p.Milestones,
                    EstimatedCompletionDate = entry.p.EstimatedCompletionDate,
                    SubmittedByName = entry.u.Name,
                    StatusName = entry.s.StatusName,
                    CoResearchers = _context.ProposalResearchers
                    .Where(pr => pr.ProposalId == entry.p.Id)
                    .Join(_context.Users,
                        pr => pr.UserId,
                        u => u.UserId,
                        (pr, u) => new CoResearcherDto
                        {
                            Id = u.UserId.ToString(),
                            Name = u.Name
                        }).ToList(),
                    Attachments = _context.Attachments
                        .Where(a => a.ProposalId == entry.p.Id)
                        .ToList()
                })
                .AsQueryable();

            if (ProjectLevelFilters != null && ProjectLevelFilters.Any())
                query = query.Where(p => ProjectLevelFilters.Contains(p.ProjectLevel));

            if (StatusFilters != null && StatusFilters.Any())
                query = query.Where(p => StatusFilters.Contains(p.StatusName));

            if (StartDate.HasValue)
                query = query.Where(p => p.EstimatedCompletionDate >= StartDate.Value);
            if (EndDate.HasValue)
                query = query.Where(p => p.EstimatedCompletionDate <= EndDate.Value);

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                // If the input is all digits, search by id
                if (int.TryParse(SearchKeyword, out int searchId))
                {
                    query = query.Where(p => p.Id == searchId);
                }
                else
                {
                    // Case insensitive for non-integers
                    var lowered = SearchKeyword.ToLower();
                    query = query.Where(p =>
                        p.Title != null && p.Title.ToLower().Contains(lowered)
                        || p.Id.ToString().Contains(lowered) // And on top of that, allow typing an ID within another string
                    );
                }
            }

            query = SortBy == "Title" ? query.OrderBy(p => p.Title) : query.OrderByDescending(p => p.EstimatedCompletionDate);

            var totalCount = query.Count();
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

        public IActionResult Summary(int id)
        {
            var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
            if (proposal == null)
                return NotFound();

            var user = _context.Users.FirstOrDefault(u => u.UserId == proposal.SubmittedBy);
            var projectLevel = _context.ProjectLevels.FirstOrDefault(pl => pl.LevelId == proposal.ProjectLevelId)?.LevelName ?? "Unknown";
            var statusName = _context.Statuses.FirstOrDefault(s => s.StatusId == proposal.StatusId)?.StatusName ?? "Unknown";

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

            var attachments = _context.Attachments
                .Where(a => a.ProposalId == proposal.Id)
                .ToList();

            // Comment logic
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
                MethodImage = _context.Attachments
                    .Where(a => a.ProposalId == proposal.Id && a.TypeId == MethodImageTypeId)
                    .Select(a => a.FileUrl)
                    .FirstOrDefault(),
                ProjectLevel = projectLevel,
                PhysicalResources = proposal.PhysicalResources,
                FinancialResources = financialResources,
                EthicalConsiderations = proposal.EthicalConsiderations,
                Outcomes = proposal.Outcomes,
                Milestones = proposal.Milestones,
                EstimatedCompletionDate = proposal.EstimatedCompletionDate,
                SubmittedByName = user?.Name ?? "Unknown",
                StatusName = statusName,
                CoResearchers = coResearchers,
                Attachments = attachments,
                Comments = comments
            };

            return View("Summary", model);
        }
        

        // Comment Post method
        [HttpPost]
        public IActionResult AddComment(int ProposalId, string CommentText)
        {
            if (string.IsNullOrWhiteSpace(CommentText))
            {
                // Redirect back with error message if needed
                return RedirectToAction("Summary", new { id = ProposalId });
            }

            var comment = new Comment
            {
                ProposalId = ProposalId,
                CommentText = CommentText,
                Commenter = GetCurrentUserId(), 
                Timestamp = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Summary", new { id = ProposalId });
        }


    }
}