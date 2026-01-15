namespace SVAuroraERP.Domain.Orders.ManageOrder
{

    [Table("tRectifyLaserPlate")]
    public class RectifyLaserPlate
    {
        [Column("PK_RectifyLaserPlateID"), Key] public int RectifyLaserPlateID { get; set; }
        [Column("FK_HSRPOrderID")] public int HSRPOrderID { get; set; }
        [Column("FK_HSRPOrderRectificationReasonID")] public int HSRPOrderRectificationReasonID { get; set; }
        [Column("FK_FrontLaserNoPlateID")] public int? FrontLaserNoPlateID { get; set; }
        [Column("FK_RearLaserNoPlateID")] public int? RearLaserNoPlateID { get; set; }
        [Column("Remarks")] public string? Remarks { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    public class LaserNoUpdateRequest
    {
        public int HSRPOrderID { get; set; }
        public int? FrontLaserNoPlateID { get; set; }
        public int? RearLaserNoPlateID { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateOnly OrderDate { get; set; }
        public string sOrderDate { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineNumber { get; set; }
    }
}
