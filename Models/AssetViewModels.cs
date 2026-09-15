using System.ComponentModel.DataAnnotations;

namespace ITServiceAssetManagement.Models
{
    public class AssetFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Nama Asset")]
        public string AssetName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Kode Asset")]
        public string? AssetCode { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [Required]
        public string Status { get; set; } = "ACTIVE";

        [Display(Name = "Tanggal Pembelian")]
        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }
    }
}
