namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tHSRPPlateDimension")]
    public class HSRPPlateDimension
    {
        [Column("PK_HSRPPlateDimensionID"), Key] public int HSRPPlateDimensionID { get; set; }
        [Column("FK_VehiclePlateSizeID")] public int VehiclePlateSizeID { get; set; }
        [Column("FK_VehiclePlateColorID")] public int VehiclePlateColorID { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_HSRPPlateDimension")]
    public class VHSRPPlateDimension
    {
        [Column("PK_HSRPPlateDimensionID"), Key] public int HSRPPlateDimensionID { get; set; }
        [Column("FK_VehiclePlateSizeID")] public int VehiclePlateSizeID { get; set; }
        [Column("VehiclePlateColorName")] public string?   VehiclePlateColorName { get; set; }
        [Column("FK_VehiclePlateColorID")] public int VehiclePlateColorID { get; set; }
        [Column("VehiclePlateSizeName")] public string? VehiclePlateSizeName { get; set; }
        [Column("Dimension")] public string? Dimension { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}