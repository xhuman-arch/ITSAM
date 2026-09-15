namespace ITServiceAssetManagement.Models.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string EntityName { get; set; } = string.Empty; // "Ticket", "Asset", "ReplacementRequest"

        public int EntityId { get; set; }

        public string Action { get; set; } = string.Empty; // "CREATE", "UPDATE", "DELETE", "ASSIGN", "STATUS_CHANGE"

        public string Description { get; set; } = string.Empty;

        public string PerformedById { get; set; } = string.Empty;
        public ApplicationUser? PerformedBy { get; set; }

        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    }
}
