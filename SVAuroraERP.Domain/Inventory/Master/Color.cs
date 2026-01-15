namespace SVAuroraERP.Domain.Inventory.Master
{
    //Added on 2025.01.04
    [Table("tColor")]
    public class Color
    {
        [Column("PK_ColorID"), Key] public int ColorID { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }

    [Table("V_Color")]
    public class VColor
    {
        [Column("PK_ColorID"), Key] public int ColorID { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}