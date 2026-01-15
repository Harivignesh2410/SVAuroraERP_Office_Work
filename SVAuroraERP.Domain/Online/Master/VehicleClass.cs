//Added on 2025.05.05 by Harivignesh (US-49)
namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tVehicleClass")]
    public class VehicleClass
    {
        [Column("PK_VehicleClassID"), Key] public int VehicleClassID { get; set; }
        [Column("VehicleClassCode")] public string? VehicleClassCode { get; set; }
        [Column("VehicleClassName")] public string? VehicleClassName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_VehicleClass")]
    public class VVehicleClass
    {
        [Column("PK_VehicleClassID"), Key] public int VehicleClassID { get; set; }
        [Column("VehicleClassCode")] public string? VehicleClassCode { get; set; }
        [Column("VehicleClassName")] public string? VehicleClassName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}
