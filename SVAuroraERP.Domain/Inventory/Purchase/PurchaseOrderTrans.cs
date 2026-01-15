namespace SVAuroraERP.Domain.Inventory.Purchase
{
    [Table("tPurchaseOrderTrans")]
    public class PurchaseOrderTrans
    {
        [Column("PK_PurchaseOrderTransID"), Key] public int PurchaseOrderTransID { get; set; }
        [Column("FK_PurchaseOrderID")] public int PurchaseOrderID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("Quantity")] public decimal? Quantity { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public string? StatusFlag { get; set; } //I - Insert, U - Update, D - Delete
    }
    [Table("V_PurchaseOrderTrans")]
    public class VPurchaseOrderTrans
    {
        [Column("PK_PurchaseOrderTransID"), Key] public int PurchaseOrderTransID { get; set; }
        [Column("FK_PurchaseOrderID")] public int PurchaseOrderID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("ItemCode")] public string? ItemCode { get; set; }
        [Column("HSNCode")] public string? HSNCode { get; set; }
        [Column("UnitName")] public string? UnitName { get; set; }
        [Column("Quantity")] public decimal? Quantity { get; set; }
        [NotMapped] public string? StatusFlag { get; set; }
    }

}
