namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tUnit")]
    public class Unit
    {
        [Column("PK_UnitID"), Key] public int UnitID { get; set; }
        [Column("UnitCode")] public string? UnitCode { get; set; }
        [Column("UnitName")] public string? UnitName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }

    [Table("V_Unit")]
    public class VUnit
    {
        [Column("PK_UnitID"), Key] public int UnitID { get; set; }
        [Column("UnitCode")] public string? UnitCode { get; set; }
        [Column("UnitName")] public string? UnitName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        [Column("LastUpdatedDateIST")] public DateTime LastUpdatedDateIST { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}