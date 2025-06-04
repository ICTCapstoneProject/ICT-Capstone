using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FSSA.Models;
using ProjectManagerMvc.Services;

namespace FSSA.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ProjectManagerContext _context;
        private readonly INotificationService _notificationService;

        public NotificationController(ProjectManagerContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.Name == null)
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (user == null)
                return NotFound();

            var notifications = await _notificationService.GetUserNotificationsAsync(user.UserId);
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            if (User.Identity?.Name == null)
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (user == null)
                return NotFound();

            await _notificationService.MarkAllAsReadAsync(user.UserId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            if (User.Identity?.Name == null)
                return Json(new { count = 0 });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (user == null)
                return Json(new { count = 0 });

            var count = await _notificationService.GetUnreadCountAsync(user.UserId);
            return Json(new { count });
        }
    }
}