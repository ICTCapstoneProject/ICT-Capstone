using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSSA.Models;
using System.Linq;

namespace FSSA.Controllers
{

    [Authorize(Roles = "Committee Chair")]
    public class ProjectLevelController : Controller
    {
        private readonly ProjectManagerContext _context;

        public ProjectLevelController(ProjectManagerContext context)
        {
            _context = context;
        }

        // Returns all project levels for display/modification
        public IActionResult Index()
        {
            var levels = _context.ProjectLevels.OrderBy(l => l.LevelId).ToList();
            return PartialView("_ProjectLevelModal", levels);
        }

        [HttpPost]
        public IActionResult Add(string newLevelName, string chairConfirmation)
        {
            if (string.IsNullOrWhiteSpace(newLevelName) || chairConfirmation != "ChairPrivileges")
            {
                // Always go to Denied on mistakes
                return RedirectToAction("Denied", new { reason = 
                    string.IsNullOrWhiteSpace(newLevelName) ? 
                    "You must enter a project level name." : 
                    "You must type the indicated passkey to authorise changes." });
            }

            _context.ProjectLevels.Add(new ProjectLevel { LevelName = newLevelName });
            _context.SaveChanges();
            return RedirectToAction("Success", new { levelName = newLevelName });
        }

        [HttpPost]
        public IActionResult Edit(int levelId, string newName, string chairConfirmation)
        {
            if (string.IsNullOrWhiteSpace(newName) || chairConfirmation != "ChairPrivileges")
            {
                return RedirectToAction("Denied", new { reason =
                    string.IsNullOrWhiteSpace(newName) ?
                    "You must enter a project level name." :
                    "You must type 'ChairPrivileges' to authorise changes." });
            }

            var level = _context.ProjectLevels.FirstOrDefault(l => l.LevelId == levelId);
            if (level != null)
            {
                level.LevelName = newName;
                _context.SaveChanges();
            }
            return RedirectToAction("Success", new { levelName = newName, actionType = "edit" });
        }

        // success get
        public IActionResult Success(string levelName, string actionType = null)
        {
            ViewBag.ActionType = actionType;
            return View(model: levelName);
        }

        // denied get
        public IActionResult Denied(string reason)
        {
            ViewBag.Reason = reason;
            return View();
        }
    }
}