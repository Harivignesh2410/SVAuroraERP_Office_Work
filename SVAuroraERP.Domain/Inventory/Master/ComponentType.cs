namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tComponentType")]
    public class ComponentType
    {
        [Column("PK_ComponentTypeID"), Key] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeCode")] public string? ComponentTypeCode { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }

    [Table("V_ComponentType")]
    public class VComponentType
    {
        [Column("PK_ComponentTypeID"), Key] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeCode")] public string? ComponentTypeCode { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
    [Table("V_ComponentExceptType")]
    public class VComponentExceptType
    {
        [Column("PK_ComponentTypeID"), Key] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeCode")] public string? ComponentTypeCode { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}