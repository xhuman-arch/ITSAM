using System.ComponentModel.DataAnnotations;

namespace ITServiceAssetManagement.Models
{
    public class TicketCreateViewModel
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string Priority { get; set; } = "MEDIUM";

        [Display(Name = "Asset Terkait (opsional)")]
        public int? AssetId { get; set; }

        [Display(Name = "Lampiran Foto/Dokumen (opsional)")]
        public IFormFile? AttachmentFile { get; set; }
    }

    public class TicketAssignViewModel
    {
        public int TicketId { get; set; }

        [Required]
        [Display(Name = "Tugaskan ke (IT_SUPPORT)")]
        public string AssignedToId { get; set; } = string.Empty;
    }

    public class TroubleshootingCreateViewModel
    {
        public int TicketId { get; set; }

        [Required]
        [Display(Name = "Tindakan yang Dilakukan")]
        public string ActionTaken { get; set; } = string.Empty;

        public string? Notes { get; set; }

        [Required]
        public string Result { get; set; } = "PENDING";

        [Display(Name = "Lampiran Bukti Perbaikan (opsional)")]
        public IFormFile? AttachmentFile { get; set; }
    }

    public class ReplacementRequestCreateViewModel
    {
        public int? TicketId { get; set; }

        [Required]
        [Display(Name = "Asset yang Diganti")]
        public int AssetId { get; set; }

        [Required]
        [Display(Name = "Alasan Penggantian")]
        public string Reason { get; set; } = string.Empty;
    }

    public class ReplacementProcessViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Status { get; set; } = "APPROVED"; // APPROVED, REJECTED, COMPLETED
    }
}
