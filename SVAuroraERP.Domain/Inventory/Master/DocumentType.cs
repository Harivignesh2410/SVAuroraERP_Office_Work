namespace SVAuroraERP.Domain.Master
{
    [Table("tDocumentType")]
    public class DocumentType
    {
        [Column("PK_DocumentTypeID"), Key] public int DocumentTypeID { get; set; }
        [Column("FK_DocumentGroupID")] public int DocumentGroupID { get; set; }
        [Column("DocumentTypeCode")] public string? DocumentTypeCode { get; set; }
        [Column("DocumentTypeName")] public string? DocumentTypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }

    }
    [Table("V_DocumentType")]
    public class VDocumentType
    {
        [Column("PK_DocumentTypeID"), Key] public int DocumentTypeID { get; set; }
        [Column("FK_DocumentGroupID")] public int DocumentGroupID { get; set; }
        [Column("DocumentGroupName")] public string? DocumentGroupName { get; set; }
        [Column("DocumentTypeCode")] public string? DocumentTypeCode { get; set; }
        [Column("DocumentTypeName")] public string? DocumentTypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }

    }
}
