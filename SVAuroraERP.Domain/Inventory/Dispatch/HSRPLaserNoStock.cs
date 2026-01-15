namespace SVAuroraERP.Domain.Inventory.Dispatch
{
    [Table("tHSRPLaserNoStock")]
    public class HSRPLaserNoStock
    {
        [Column("PK_HSRPLaserNoStockID"), Key] public int HSRPLaserNoStockID { get; set; }
        [Column("FK_NumberPlateDispatchTransID")] public int? NumberPlateDispatchTransID { get; set; }
        [Column("Fk_StockStatusID")] public int? StockStatusID { get; set; } = 1;
        [Column("SerialNo")] public string? SerialNo { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }

    }
    public class HSRPlaserStockRequest
    {
        public string? OrderIds { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
    public class CheckAvailableOrderLaserNoRequest
    {
        public string? OrderIds { get; set; }
    }
    public class LaserAvailabilitySummary
    {
        public int TotalOrders { get; set; }
        public int FrontLaserAvailable { get; set; }
        public int RearLaserAvailable { get; set; }
        public int BothLaserAvailable { get; set; }
        public int RejectedCount { get; set; }
        public string? RejectedReasons { get; set; }
    }
    public class LaserAvailabilityResult
    {
        public LaserAvailabilitySummary Summary { get; set; }
        public List<int> ValidOrderIds { get; set; } = new();
    }



}