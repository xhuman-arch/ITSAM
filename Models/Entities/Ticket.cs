using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITServiceAssetManagement.Models.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "OPEN"; // OPEN, IN_PROGRESS, RESOLVED, CLOSED

        public string Priority { get; set; } = "MEDIUM"; // LOW, MEDIUM, HIGH

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // SLA: batas waktu penyelesaian, dihitung otomatis dari Priority saat tiket dibuat
        public DateTime? DueDate { get; set; }

        // Requester (Employee yang membuat tiket)
        [Required]
        public string RequesterId { get; set; } = string.Empty;
        public ApplicationUser? Requester { get; set; }

        // Asset terkait (opsional)
        public int? AssetId { get; set; }
        public Asset? Asset { get; set; }

        // Navigation
        public ICollection<TicketAssignment> TicketAssignments { get; set; } = new List<TicketAssignment>();
        public ICollection<Troubleshooting> Troubleshootings { get; set; } = new List<Troubleshooting>();
        public ICollection<ReplacementRequest> ReplacementRequests { get; set; } = new List<ReplacementRequest>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        // Tidak disimpan ke DB — dihitung on-the-fly untuk ditampilkan di UI
        [NotMapped]
        public bool IsOverdue => DueDate.HasValue
            && DateTime.UtcNow > DueDate.Value
            && Status != "RESOLVED"
            && Status != "CLOSED";
    }
}
