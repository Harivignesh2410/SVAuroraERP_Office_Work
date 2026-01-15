namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tVehiclePlateSize")]
    public class VehiclePlateSize
    {
        [Column("PK_VehiclePlateSizeID"), Key] public int VehiclePlateSizeID { get; set; }
        [Column("VehiclePlateSizeCode")] public string? VehiclePlateSizeCode { get; set; }
        [Column("VehiclePlateSizeName")] public string? VehiclePlateSizeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_VehiclePlateSize")]
    public class VVehiclePlateSize
    {
        [Column("PK_VehiclePlateSizeID"), Key] public int VehiclePlateSizeID { get; set; }
        [Column("VehiclePlateSizeCode")] public string? VehiclePlateSizeCode { get; set; }
        [Column("VehiclePlateSizeName")] public string? VehiclePlateSizeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}