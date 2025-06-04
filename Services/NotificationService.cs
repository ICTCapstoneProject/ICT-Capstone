using Microsoft.EntityFrameworkCore;
using FSSA.Models;

namespace ProjectManagerMvc.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ProjectManagerContext _context;

        public NotificationService(ProjectManagerContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(int userId, string message, int? proposalId = null, string notificationType = "General")
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                ProposalId = proposalId,
                NotificationType = notificationType,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationForRoleAsync(string roleName, string message, int? proposalId = null, string notificationType = "General", int? excludeUserId = null)
        {
            List<User> targetUsers;

            if (roleName.ToLower() == "all")
            {
                targetUsers = await _context.Users.ToListAsync();
            }
            else
            {
                targetUsers = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == roleName))
                    .ToListAsync();
            }

            foreach (var user in targetUsers)
            {
                if (excludeUserId.HasValue && user.UserId == excludeUserId.Value)
                    continue;

                await CreateNotificationAsync(user.UserId, message, proposalId, notificationType);
            }
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
        {
            var query = _context.Notifications.AsQueryable();
            query = query.Where(n => n.UserId == userId);
            
            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }
            
            return await query
                .Include(n => n.Proposal)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }
    }
}