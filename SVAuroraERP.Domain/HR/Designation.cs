namespace SVAuroraERP.Domain.HR
{
    [Table("tDesignation")]
    public class Designation
    {
        [Column("PK_DesignationID"), Key] public int DesignationID { get; set; }
        [Column("DesignationName")] public string? DesignationName { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }

    }
    [Table("V_Designation")]
    public class VDesignation
    {
        [Column("PK_DesignationID"), Key] public int DesignationID { get; set; }
        [Column("DesignationName")] public string? DesignationName { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDateIST")] public DateTime LastUpdatedDateIST { get; set; }
    }
}
