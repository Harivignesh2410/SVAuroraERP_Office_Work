namespace SVAuroraERP.Domain.Inventory.Production
{
    [Table("tProductionConsumption")]
    public class ProductionConsumption
    {
        [Column("PK_ProductionConsumptionID"), Key] public int ProductionConsumptionID { get; set; }
        [Column("FK_ProductionInwardID")] public int ProductionInwardID { get; set; }
        [Column("FK_StockRequestTransID")] public int StockRequestTransID { get; set; }
        [Column("ActualConsumedQty")] public decimal ActualConsumedQty { get; set; }
        [Column("WastageQty")] public decimal WastageQty { get; set; }
        [Column("WastagePercentage")] public decimal WastagePercentage { get; set; }
        [Column("BalanceQty")] public decimal BalanceQty { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
    }
}
