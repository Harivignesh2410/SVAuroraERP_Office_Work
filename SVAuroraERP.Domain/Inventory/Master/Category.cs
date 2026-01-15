namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tCategory")]
    public class Category
    {
        [Column("PK_CategoryID"), Key] public int CategoryID { get; set; }
        [Column("CategoryCode")] public string? CategoryCode { get; set; }
        [Column("CategoryName")] public string? CategoryName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }

    [Table("V_Category")]
    public class VCategory
    {
        [Column("PK_CategoryID"), Key] public int CategoryID { get; set; }
        [Column("CategoryCode")] public string? CategoryCode { get; set; }
        [Column("CategoryName")] public string? CategoryName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}