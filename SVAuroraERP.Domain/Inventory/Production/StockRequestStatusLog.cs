namespace SVAuroraERP.Domain.Inventory.Production
{
    //Added on 2025.03.12
    [Table("tStockRequestStatusLog")]
    public class StockRequestStatusLog
    {
        [Column("PK_StockRequestStatusLogID"), Key] public int StockRequestStatusLogID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestId { get; set; }
        [Column("FK_StatusID")] public short StatusID { get; set; }
    }
}