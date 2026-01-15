namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tSize")]
    public class Size
    {
        [Column("PK_SizeID"), Key] public int SizeID { get; set; }
        [Column("SizeCode")] public string? SizeCode { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }

    }
    [Table("V_Size")]
    public class VSize
    {
        [Column("PK_SizeID"), Key] public int SizeID { get; set; }
        [Column("SizeCode")] public string? SizeCode { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDateIST")] public DateTime LastUpdatedDateIST { get; set; }

    }
}
