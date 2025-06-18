using FSSA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;

// Only users with Admin or Committee Chair roles can access this controller
[Authorize(Roles = "Admin,Committee Chair")]
public class AdminController : Controller
{
    private readonly ProjectManagerContext _context;

    public AdminController(ProjectManagerContext context)
    {
        _context = context;
    }

    // Display all current users to Admin panel, ordered by time created
    public IActionResult Index()
    {
        var users = _context.Users
            .OrderBy(u => u.CreatedAt)
            .Select(u => new
            {
                u.UserId,
                u.Name,
                u.Email,
                Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            }).ToList();

        return View(users);
    }

    [HttpGet]
    // Display user creation form
    public IActionResult Create()
    {
        ViewBag.Roles = new MultiSelectList(_context.Roles, "RoleId", "RoleName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // Handle user creation
    public IActionResult Create(UserCreateViewModel model)
    {

        // Confirmation key is required to creat new user
        if (model.AdminConfirmation != "AdminPrivileges")
        {
            ModelState.AddModelError("AdminConfirmation", "Incorrect override key. Try Again.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new MultiSelectList(_context.Roles, "RoleId", "RoleName", model.SelectedRoles);
            return View(model);
        }

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            PasswordHash = model.PasswordHash,
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        // Assign selected role to this user
        foreach (var roleId in model.SelectedRoles)
        {
            _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = roleId });
        }

        _context.SaveChanges();
        return RedirectToAction("CreateSuccess");
    }

    // Display user edit form
    public IActionResult Edit(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == id);
        if (user == null) return NotFound();

        var selectedRoleIds = _context.UserRoles
            .Where(ur => ur.UserId == id)
            .Select(ur => ur.RoleId)
            .ToList();

        var viewModel = new UserEditViewModel
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            PasswordHash = "",
            ExistingPasswordHash = user.PasswordHash,
            SelectedRoles = selectedRoleIds,
            AdminConfirmation = ""
        };

        ViewBag.Roles = new MultiSelectList(_context.Roles, "RoleId", "RoleName", selectedRoleIds);
        return View(viewModel);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    // Handle user edit submission
    public IActionResult Edit(UserEditViewModel model)
    {
        // Need confirmation key
        if (model.AdminConfirmation != "AdminPrivileges")
        {
            ModelState.AddModelError("AdminConfirmation", "Incorrect override key. Try Again.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new MultiSelectList(_context.Roles, "RoleId", "RoleName", model.SelectedRoles);
            return View(model);
        }

        var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
        if (user == null) return NotFound();

        // Update name and email
        user.Name = model.Name;
        user.Email = model.Email;

        // If password left blank, keep the current password, or if changed, update it
        user.PasswordHash = string.IsNullOrWhiteSpace(model.PasswordHash)
            ? model.ExistingPasswordHash
            : model.PasswordHash;

        var existingRoles = _context.UserRoles.Where(ur => ur.UserId == user.UserId);
        _context.UserRoles.RemoveRange(existingRoles);
        foreach (var roleId in model.SelectedRoles)
        {
            _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = roleId });
        }

        _context.SaveChanges();
        return RedirectToAction("EditSuccess");
    }

    // Display user delete page
    public IActionResult Delete(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == id);
        if (user == null) return NotFound();

        var viewModel = new UserDeleteViewModel
        {
            UserId = user.UserId,
            Name = user.Name
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    // Delete user
    public IActionResult DeleteConfirmed(UserDeleteViewModel model)
    {
        // Need confirmation key to delete a user
        if (model.AdminConfirmation != "AdminPrivileges")
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
            if (user == null) return NotFound();

            ModelState.AddModelError("AdminConfirmation", "Incorrect override key. Try Again.");
            return View("Delete", new UserDeleteViewModel { UserId = user.UserId });
        }

        var userToDelete = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
        if (userToDelete == null) return NotFound();

        // Remove user and their role
        var userRoles = _context.UserRoles.Where(ur => ur.UserId == model.UserId);
        _context.UserRoles.RemoveRange(userRoles);
        _context.Users.Remove(userToDelete);
        _context.SaveChanges();

        return RedirectToAction("DeleteSuccess");
    }
    
    public IActionResult CreateSuccess()
    {
        ViewBag.Action = "created";
        return View("Success");
    }

    public IActionResult EditSuccess()
    {
        ViewBag.Action = "edited";
        return View("Success");
    }

    public IActionResult DeleteSuccess()
    {
        ViewBag.Action = "deleted";
        return View("Success");
    }




}