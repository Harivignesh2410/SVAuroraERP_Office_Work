namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tOtherCharges")]
    public class OtherCharges
    {
        [Column("PK_OtherChargesID"), Key] public int OtherChargesID { get; set; }
        [Column("OtherChargesDescription")] public string? OtherChargesDescription { get; set; }
        [Column("Type")] public byte Type { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }

    [Table("V_OtherCharges")]
    public class VOtherCharges
    {
        [Column("PK_OtherChargesID"), Key] public int OtherChargesID { get; set; }
        [Column("OtherChargesDescription")] public string? OtherChargesDescription { get; set; }
        [Column("Type")] public byte Type { get; set; }
        [Column("TypeName")] public string? TypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDateIST")] public DateTime LastUpdatedDateIST { get; set; }
    }
}
