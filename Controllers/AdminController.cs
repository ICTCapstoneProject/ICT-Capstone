using FSSA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ProjectManagerContext _context;

    public AdminController(ProjectManagerContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var users = _context.Users
            .Select(u => new
            {
                u.UserId,
                u.Name,
                u.Email,
                Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            }).ToList();

        return View(users);
    }

    public IActionResult Create()
    {
        ViewBag.Roles = new MultiSelectList(_context.Roles, "RoleId", "RoleName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(User user, List<int> selectedRoles)
    {
        user.CreatedAt = DateTime.Now;
        _context.Users.Add(user);
        _context.SaveChanges();

        foreach (var roleId in selectedRoles)
        {
            _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = roleId });
        }

        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}