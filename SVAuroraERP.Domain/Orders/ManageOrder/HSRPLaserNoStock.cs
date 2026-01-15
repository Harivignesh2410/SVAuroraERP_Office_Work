namespace SVAuroraERP.Domain.Orders.ManageOrder
{
    internal class HSRPLaserNoStock
    {
    }
    [Table("V_HSrpLaserNoStock")]
    public class VHSrpLaserNoStock
    {
        [Column("PK_HSRPLaserNoStockID"), Key] public int HSRPLaserNoStockID { get; set; }
        [Column("FK_NumberPlateDispatchTransID")] public int NumberPlateDispatchTransID { get; set; }
        [Column("DispatchNo")] public string? DispatchNo { get; set; }
        [Column("Fk_StockStatusID")] public byte StockStatusID { get; set; }
        [Column("LaserNoStatus")] public string? LaserNoStatus { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("SerialNo")] public string? SerialNo { get; set; }
        [Column("FK_EmbossingStationID")] public int EmbossingStationID { get; set; }
        [Column("EmbossingStationName")] public string? EmbossingStationName { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("Dimension")] public string? Dimension { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
        [Column("StockInsertedDate")] public string? StockInsertedDate { get; set; }
    }

    [Table("LkupHSRPLaserNoStatus")]
    public class HSrpLaserNoStatus
    {
        [Column("PK_HSRPLaserNoStatusID"), Key] public byte HSRPLaserNoStatusID { get; set; }
        [Column("LaserNoStatus")] public string? LaserNoStatus { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
    }
    [Table("V_HSRPLaserNoStockLog")]
    public class VHSRPLaserNoStockLog
    {
        [Column("PK_HSRPLaserNoStockLogID"), Key] public int HSRPLaserNoStockLogID { get; set; }
        [Column("FK_HSRPLaserNoStockID")] public int HSRPLaserNoStockID { get; set; }
        [Column("FK_HSRPLaserNoStatusID")] public byte HSRPLaserNoStatusID { get; set; }
        [Column("LaserNoStatus")] public string? LaserNoStatus { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("LkupHSRPOrderRectificationReason")]
    public class LkupHSRPOrderRectificationReason
    {
        [Column("PK_HSRPOrderRectificationReasoniID"), Key] public byte HSRPOrderRectificationReasoniID { get; set; }
        [Column("HSRPOrderRectificationReason")] public string? HSRPOrderRectificationReason { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    public class HSRPLaserNoStockFilterData: DataTableRequest
    {
        public string? sStatingDate { get; set; }
        public string? sEndingDate { get; set; }
        public int? EmbossingStationID { get; set; }
        public int? SizeID { get; set; }
        public int? ColorID { get; set; }
        public byte? StockStatusID { get; set; }
    }

    public class LaserNoData
    {
        public int PK_HSRPLaserNoStockID { get; set; }
        public string? Dimension { get; set; } 
        public string? SerialNo { get; set; }
    }

    public class FullLaserNoDataResult
    {
        public DataTable FrontLaserNoData { get; set; }
        public DataTable RearLaserNoData { get; set; }
    }
    public class LaserNoDataResponse
    {
        public List<LaserNoData> FrontLaserNoData { get; set; }
        public List<LaserNoData> RearLaserNoData { get; set; }
        public List<VHologramPunching> HologramPunching { get; set; }
        public List<BatchStock> BatchStock { get; set; }
    }

}

