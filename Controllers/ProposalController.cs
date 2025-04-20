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
    }
}
