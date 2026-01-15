namespace SVAuroraERP.Domain.Production
{
    public class PendingApprovalFilter
    {
        public int ProcessTypeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? sStartDate { get; set; }
        public string? sEndDate { get; set; }
        public string? SearchInWord { get; set; }
    }
}
