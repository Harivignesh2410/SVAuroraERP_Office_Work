namespace SVAuroraERP.Domain.Inventory.Production
{
    //Added on 2025.03.12
    [Table("LkupStockRequestStatus")]
    public class StockRequestStatus
    {
        [Column("PK_StockRequestStatusID"), Key] public short StockRequestStatusID { get; set; }
        [Column("StockRequestStatus")] public string StockRequestStatusName { get; set; } = string.Empty;
        [Column("ColorCode")] public string ColorCode { get; set; } = string.Empty;
    }
}