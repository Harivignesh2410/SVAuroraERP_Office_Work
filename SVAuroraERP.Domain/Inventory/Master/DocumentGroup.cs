namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tDocumentGroup")]
    public class DocumentGroup
    {
        [Column("PK_DocumentGroupID"), Key] public int DocumentGroupID { get; set; }
        [Column("DocumentGroupCode")] public string? DocumentGroupCode { get; set; }
        [Column("DocumentGroupName")] public string? DocumentGroupName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }

    }
    [Table("V_DocumentGroup")]
    public class VDocumentGroup
    {
        [Column("PK_DocumentGroupID"), Key] public int DocumentGroupID { get; set; }
        [Column("DocumentGroupCode")] public string? DocumentGroupCode { get; set; }
        [Column("DocumentGroupName")] public string? DocumentGroupName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }

    }
}
