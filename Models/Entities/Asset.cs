using System.ComponentModel.DataAnnotations;

namespace ITServiceAssetManagement.Models.Entities
{
    public class Asset
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string AssetName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? AssetCode { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public string Status { get; set; } = "ACTIVE"; // ACTIVE, IN_REPAIR, RETIRED

        public DateTime? PurchaseDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<ReplacementRequest> ReplacementRequests { get; set; } = new List<ReplacementRequest>();
    }
}
