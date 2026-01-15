namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tOEMPricing")]

    public class OEMPricing
    {
        [Column("PK_OEMPricingID"), Key] public int OEMPricingID { get; set; }
        [Column("FK_HSRPPartNumberID")] public int HSRPPartNumberID { get; set; }
        [Column("FK_VehiclePlateSizeFrontID")] public int VehiclePlateSizeFrontID { get; set; }
        [Column("FK_VehiclePlateSizeRearID")] public int VehiclePlateSizeRearID { get; set; }
        [Column("Rivets")] public byte? Rivets { get; set; }
        [Column("SnapLock")] public byte? SnapLock { get; set; }
        [Column("Rate")] public decimal? Rate { get; set; }
        [Column("CourierCharges")] public decimal? CourierCharges { get; set; }
        [Column("TotalAmount")] public decimal? TotalAmount { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_OEMPricing")]
    public class VOEMPricing
    {
        [Column("PK_OEMPricingID"), Key] public int OEMPricingID { get; set; }
        [Column("FK_HSRPPartNumberID")] public int HSRPPartNumberID { get; set; }
        [Column("FK_VehiclePlateSizeFrontID")] public int VehiclePlateSizeFrontID { get; set; }
        [Column("FK_VehiclePlateSizeRearID")] public int VehiclePlateSizeRearID { get; set; }
        [Column("Rivets")] public byte Rivets { get; set; }
        [Column("SnapLock")] public byte SnapLock { get; set; }
        [Column("Rate")] public decimal Rate { get; set; }
        [Column("CourierCharges")] public decimal CourierCharges { get; set; }
        [Column("TotalAmount")] public decimal TotalAmount { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
        [Column("LastUpdatedDateIST")] public DateTime? LastUpdatedDateIST { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("PartNumber")] public string? PartNumber { get; set; }
        [Column("VehiclePlateSizeCodeRear")] public string? VehiclePlateSizeCodeRear { get; set; }
        [Column("VehiclePlateSizeNameRear")] public string? VehiclePlateSizeNameRear { get; set; }
        [Column("VehiclePlateSizeCodeFront")] public string? VehiclePlateSizeCodeFront { get; set; }
        [Column("VehiclePlateSizeNameFront")] public string? VehiclePlateSizeNameFront { get; set; }
        [Column("FK_OEMID")] public int? OEMID { get; set; }
        [Column("OEMName")] public string? OEMName { get; set; }
    }
}
