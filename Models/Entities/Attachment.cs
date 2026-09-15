namespace ITServiceAssetManagement.Models.Entities
{
    public class Attachment
    {
        public int Id { get; set; }

        // Attachment bisa terkait ke Ticket (bukti kerusakan) atau Troubleshooting (bukti perbaikan)
        public int? TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public int? TroubleshootingId { get; set; }
        public Troubleshooting? Troubleshooting { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty; // path relatif di wwwroot/uploads

        public string UploadedById { get; set; } = string.Empty;
        public Models.ApplicationUser? UploadedBy { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
