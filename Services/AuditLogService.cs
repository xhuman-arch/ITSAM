using ITServiceAssetManagement.Data;
using ITServiceAssetManagement.Models.Entities;

namespace ITServiceAssetManagement.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;

        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string entityName, int entityId, string action, string description, string performedById)
        {
            var log = new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                Description = description,
                PerformedById = performedById,
                PerformedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
