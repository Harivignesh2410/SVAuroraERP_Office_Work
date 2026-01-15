namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tTax")]
    public class Tax
    {
        [Column("PK_TaxID"), Key] public int TaxID { get; set; }
        [Column("TaxCode")] public string? TaxCode { get; set; }
        [Column("TaxName")] public string? TaxName { get; set; }
        [Column("TaxPercentage")] public decimal TaxPercentage { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }
    [Table("V_Tax")]
    public class VTax
    {
        [Column("PK_TaxID"), Key] public int TaxID { get; set; }
        [Column("TaxCode")] public string? TaxCode { get; set; }
        [Column("TaxName")] public string? TaxName { get; set; }
        [Column("TaxPercentage")] public decimal TaxPercentage { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDateIST")] public DateTime LastUpdatedDateIST { get; set; }
    }
}