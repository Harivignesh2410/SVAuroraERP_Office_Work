namespace SVAuroraERP.Domain.Inventory.Dispatch
{
    [Table("tMapInventoryandHSRPColor")]
    public class MapPlateColor
    {
        [Column("PK_MapInventoryandHSRPColorID"), Key] public int MapInventoryandHSRPColorID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("FK_HSRPPlateColorID")] public int HSRPPlateColorID { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_MapInventoryandHSRPColor")]
    public class VMapPlateColor
    {
        [Column("PK_MapInventoryandHSRPColorID"), Key] public int MapInventoryandHSRPColorID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }
        [Column("FK_HSRPPlateColorID")] public int HSRPPlateColorID { get; set; }
        [Column("VehiclePlateColorName")] public string? VehiclePlateColorName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}
