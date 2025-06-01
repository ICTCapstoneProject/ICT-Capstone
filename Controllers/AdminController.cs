using FSSA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;

[Authorize(Roles = "Admin,Committee Chair")]
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

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Roles = new MultiSelectList(_context.Roles, "RoleId", "RoleName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UserCreateViewModel model)
    {


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

        foreach (var roleId in model.SelectedRoles)
        {
            _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = roleId });
        }

        _context.SaveChanges();
        return RedirectToAction("CreateSuccess");
    }

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
    public IActionResult Edit(UserEditViewModel model)
    {
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

        user.Name = model.Name;
        user.Email = model.Email;

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
    public IActionResult DeleteConfirmed(UserDeleteViewModel model)
    {
        if (model.AdminConfirmation != "AdminPrivileges")
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
            if (user == null) return NotFound();

            ModelState.AddModelError("AdminConfirmation", "Incorrect override key. Try Again.");
            return View("Delete", new UserDeleteViewModel { UserId = user.UserId });
        }

        var userToDelete = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
        if (userToDelete == null) return NotFound();

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