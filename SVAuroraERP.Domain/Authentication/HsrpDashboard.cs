namespace SVAuroraERP.Domain.Authentication
{
    public class HsrpDashboard
    {
        [Column("FK_OrderStatusID")]public int OrderStatusID { get; set; }

        [Column("Description")]public string? Description { get; set; }

        [Column("OrdinalNo")]public int OrdinalNo { get; set; }

        [Column("OrderCount")]public int OrderCount { get; set; }
    }

    public class SummaryCount
    {
        [Column("OrderType")]public string? OrderType { get; set; }

        [Column("TotalOrders")] public int TotalOrders { get; set; }
    }

    public class HsrpDashboardRequest : DataResponse
    {
        public int? UserID { get; set; }
    }
    public class HsrpDashboardDataSet
    {
        public DataTable OEMOrders { get; set; }
        public DataTable OnlineOrders { get; set; }
        public DataTable SummaryCounts { get; set; }
    }


}
