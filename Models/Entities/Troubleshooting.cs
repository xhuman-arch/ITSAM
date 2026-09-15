namespace ITServiceAssetManagement.Models.Entities
{
    public class Troubleshooting
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public string? Notes { get; set; }

        public string? ActionTaken { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string PerformedById { get; set; } = string.Empty;
        public ApplicationUser? PerformedBy { get; set; }

        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

        public string Result { get; set; } = "PENDING"; // PENDING, SUCCESS, FAILED

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
