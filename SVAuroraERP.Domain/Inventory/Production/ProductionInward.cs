namespace SVAuroraERP.Domain.Inventory.Production
{
    [Table("tProductionInward")]
    public class ProductionInward
    {
        [Column("PK_ProductionInwardID"), Key] public int ProductionInwardID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestID { get; set; }
        [Column("FK_OutputComponentTypeID")] public int OutputComponentTypeID { get; set; }
        [Column("ExpectedProductionQty")] public decimal ExpectedProductionQty { get; set; }
        [Column("ActualProductionQty")] public decimal ActualProductionQty { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("FK_RackLocationID")] public int RackLocationID { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public List<ProductionConsumption> ProductionConsumption { get; set; }
        //Added on 2025/05/20 by Harivignesh
        [Column("FK_OperatorID")] public int OperatorID { get; set; }
    }
}
