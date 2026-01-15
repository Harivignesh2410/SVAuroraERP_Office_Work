namespace SVAuroraERP.Domain.Inventory.Production
{
    [Table("tProductionConfiguration")]
    public class ProductionConfiguration
    {
        [Column("PK_ProductionConfigurationID"), Key] public int ProductionConfigurationID { get; set; }
        [Column("FK_ProcessTypeID")] public byte ProcessTypeID { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("V_ProductionConfiguration")]
    public class VProductionConfiguration
    {
        [Column("PK_ProductionConfigurationID"), Key] public int ProductionConfigurationID { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("FK_ProcessTypeID")] public byte ProcessTypeID { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("ComponentTypeCode")] public string? ComponentTypeCode { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [Column("ProcessTypeName")] public string? ProcessTypeName { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}