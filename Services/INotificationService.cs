using FSSA.Models;

namespace ProjectManagerMvc.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string message, int? proposalId = null, string notificationType = "General");
        Task CreateNotificationForRoleAsync(string roleName, string message, int? proposalId = null, string notificationType = "General", int? excludeUserId = null);
        Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}