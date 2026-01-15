

namespace SVAuroraERP.Domain.Inventory.Production
{
    [Table("tProductionCalculation")]
    public class ProductionCalculation
    {
        [Column("PK_ProductionCalculationID"), Key] public int ProductionCalculationID { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_UnitID")] public int UnitID { get; set; }
        [Column("QuantityForOneUnit")] public decimal QuantityForOneUnit { get; set; }
        [Column("ProductionQuantity")] public decimal ProductionQuantity { get; set; }
        [Column("PerPlate")] public decimal PerPlate { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_ProductionCalculation")]
    public class VProductionCalculation
    {
        [Column("PK_ProductionCalculationID"), Key] public int ProductionCalculationID { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("FK_UnitID")] public int UnitID { get; set; }
        [Column("UnitName")] public string? UnitName { get; set; }
        [Column("QuantityForOneUnit")] public decimal QuantityForOneUnit { get; set; }
        [Column("ProductionQuantity")] public decimal ProductionQuantity { get; set; }
        [Column("PerPlate")] public decimal PerPlate { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
}
