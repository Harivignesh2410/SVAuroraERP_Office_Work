namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tVehiclePlateColor")]
    public class VehiclePlateColor
    {
        [Column("PK_VehiclePlateColorID"), Key] public int VehiclePlateColorID { get; set; }
        [Column("VehiclePlateColorCode")] public string? VehiclePlateColorCode { get; set; }
        [Column("VehiclePlateColorName")] public string? VehiclePlateColorName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_VehiclePlateColor")]
    public class VVehiclePlateColor
    {
        [Column("PK_VehiclePlateColorID"), Key] public int VehiclePlateColorID { get; set; }
        [Column("VehiclePlateColorCode")] public string? VehiclePlateColorCode { get; set; }
        [Column("VehiclePlateColorName")] public string? VehiclePlateColorName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}