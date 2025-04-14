using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FSSA.Models;

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
                proposal.StatusId = 1;             // Default to "New Proposal" status from the db
                proposal.SubmittedBy = 1;          // Hardcoded for now using the guest I made in the db
                proposal.CreatedAt = DateTime.Now;
                proposal.UpdatedAt = DateTime.Now;

                _context.Proposals.Add(proposal);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
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
    }
}
