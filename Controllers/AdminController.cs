using FSSA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;

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



        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new MultiSelectList(_context.Roles, "RoleId", "RoleName", selectedRoles);
            return View(user);
        }


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

    public IActionResult Edit(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == id);
        if (user == null) return NotFound();

        var selectedRoleIds = _context.UserRoles
            .Where(ur => ur.UserId == id)
            .Select(ur => ur.RoleId)
            .ToList();

        ViewBag.Roles = new MultiSelectList(_context.Roles, "RoleId", "RoleName", selectedRoleIds);
        return View(user);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(User updatedUser, List<int> selectedRoles)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
        if (user == null) return NotFound();

        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        if (!string.IsNullOrWhiteSpace(updatedUser.PasswordHash))
        {
            user.PasswordHash = updatedUser.PasswordHash;
        }



        var existingRoles = _context.UserRoles.Where(ur => ur.UserId == user.UserId);
        _context.UserRoles.RemoveRange(existingRoles);
        foreach (var roleId in selectedRoles)
        {
            _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = roleId });
        }

        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == id);
        if (user == null) return NotFound();

        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == id);
        if (user == null) return NotFound();
        var userRoles = _context.UserRoles.Where(ur => ur.UserId == id);
        _context.UserRoles.RemoveRange(userRoles);
        _context.Users.Remove(user);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }




}