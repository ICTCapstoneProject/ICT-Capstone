using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        // POST: /Proposal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Proposal proposal)
        {
            if (ModelState.IsValid)
            {
                proposal.ProjectLevelId = 1; // Default to "New Proposal"
                proposal.CreatedAt = DateTime.Now;

                _context.Proposals.Add(proposal);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(proposal);
        }
    }
}
