namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tRackLocation")]
    public class RackLocation
    {
        [Column("PK_RackLocationID"), Key] public int RackLocationID { get; set; }
        [Column("FK_WareHouseID")] public int WareHouseID { get; set; }
        [Column("RackLocationCode")] public string? RackLocationCode { get; set; }
        [Column("RackLocationName")] public string? RackLocationName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        // [NotMapped]public List<RackLocationSizeCapacity> RackLocationSizeCapacity { get; set; }

    }
    [Table("V_RackLocation")]
    public class VRackLocation
    {
        [Column("PK_RackLocationID"), Key] public int RackLocationID { get; set; }
        [Column("FK_WareHouseID")] public int WareHouseID { get; set; }
        [Column("WareHouseName")] public string? WareHouseName { get; set; }
        [Column("RackLocationCode")] public string? RackLocationCode { get; set; }
        [Column("RackLocationName")] public string? RackLocationName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [NotMapped] public List<RackLocationSizeCapacity> RackLocationSizeCapacity { get; set; }
    }
}
