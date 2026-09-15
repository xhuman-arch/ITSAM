using ITServiceAssetManagement.Data;
using ITServiceAssetManagement.Models;
using ITServiceAssetManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace ITServiceAssetManagement.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task NotifyAsync(string userId, string title, string message, string? linkUrl = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                LinkUrl = linkUrl,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task NotifyRoleAsync(string role, string title, string message, string? linkUrl = null)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);

            foreach (var user in users)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Title = title,
                    Message = message,
                    LinkUrl = linkUrl,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
