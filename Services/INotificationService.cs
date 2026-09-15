namespace ITServiceAssetManagement.Services
{
    public interface INotificationService
    {
        Task NotifyAsync(string userId, string title, string message, string? linkUrl = null);
        Task NotifyRoleAsync(string role, string title, string message, string? linkUrl = null);
    }
}
