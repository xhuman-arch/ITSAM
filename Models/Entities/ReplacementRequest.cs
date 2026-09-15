namespace ITServiceAssetManagement.Models.Entities
{
    public class ReplacementRequest
    {
        public int Id { get; set; }

        public int? TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public int? AssetId { get; set; }
        public Asset? Asset { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string RequestedById { get; set; } = string.Empty;
        public ApplicationUser? RequestedBy { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "PENDING"; // PENDING, APPROVED, REJECTED, COMPLETED

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        // Diproses oleh Purchasing
        public string? ProcessedById { get; set; }
        public ApplicationUser? ProcessedBy { get; set; }

        public DateTime? ProcessedAt { get; set; }
    }
}
