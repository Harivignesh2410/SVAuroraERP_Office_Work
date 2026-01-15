namespace SVAuroraERP.Domain.Inventory.Dispatch
{
    [Table("tMapInventoryandHSRPSize")]
    public class MapPlateSize
    {
        [Column("PK_MapInventoryandHSRPSizeID"), Key] public int MapInventoryandHSRPSizeID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_HSRPPlateSizeID")] public int HSRPPlateSizeID { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }

    }
    [Table("V_MapInventoryandHSRPSize")]
    public class VMapPlateSize
    {
        [Column("PK_MapInventoryandHSRPSizeID"), Key] public int MapInventoryandHSRPSizeID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("FK_HSRPPlateSizeID")] public int HSRPPlateSizeID { get; set; }
        [Column("VehiclePlateSizeName")] public string? VehiclePlateSizeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }

    }
}
