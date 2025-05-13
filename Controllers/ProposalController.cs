using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FSSA.Models;
using System;
using System.Linq;
using System.Security.Claims;

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


        public IActionResult MyProposals()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !string.Equals(user.Role, "researcher", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            var proposals = _context.Proposals
                .Where(p => p.SubmittedBy == user.UserId)
                .Join(_context.ProjectLevels,
                    p => p.ProjectLevelId,
                    pl => pl.LevelId,
                    (p, pl) => new MyProposalViewModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Synopsis = p.Synopsis,
                        ProjectLevel = pl.LevelName,
                        EstimatedCompletionDate = p.EstimatedCompletionDate
                    })
                .ToList();

            return View(proposals);
        }


        public IActionResult Details(int id)
        {
            var proposal = _context.Proposals.FirstOrDefault(p => p.Id == id);
            if (proposal == null)
            {
                return NotFound();
            }

            var projectLevel = _context.ProjectLevels.FirstOrDefault(pl => pl.LevelId == proposal.ProjectLevelId)?.LevelName ?? "Unknown";

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
                EstimatedCompletionDate = proposal.EstimatedCompletionDate        
            };

            return View(model);
        }

    }
}
