namespace ITServiceAssetManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }

        public int UnassignedTickets { get; set; }
        public int MyAssignedTickets { get; set; }
        public int MyOpenTickets { get; set; }
        public int OverdueTickets { get; set; }

        public int TotalAssets { get; set; }
        public int ActiveAssets { get; set; }
        public int InRepairAssets { get; set; }
        public int RetiredAssets { get; set; }

        public int PendingReplacementRequests { get; set; }
        public int TotalReplacementRequests { get; set; }
    }
}
