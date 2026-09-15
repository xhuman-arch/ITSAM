namespace ITServiceAssetManagement.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string entityName, int entityId, string action, string description, string performedById);
    }
}
