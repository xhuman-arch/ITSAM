namespace ITServiceAssetManagement.Models.Entities
{
    public class TicketAssignment
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string AssignedToId { get; set; } = string.Empty;
        public ApplicationUser? AssignedTo { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "ASSIGNED"; // ASSIGNED, IN_PROGRESS, DONE
    }
}
