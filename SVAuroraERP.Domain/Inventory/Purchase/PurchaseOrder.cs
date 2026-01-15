namespace SVAuroraERP.Domain.Inventory.Purchase
{
    [Table("tPurchaseOrder")]
    public class PurchaseOrder
    {
        [Column("PK_PurchaseOrderID"), Key] public int PurchaseOrderID { get; set; }
        [Column("FK_SupplierID")] public int SupplierID { get; set; }
        [Column("PurchaseOrderNo")] public string? PurchaseOrderNo { get; set; }
        [Column("PurchaseOrderDate")] public DateTime PurchaseOrderDate { get; set; }
        [NotMapped] public string? sPurchaseOrderDate { get; set; }
        [Column("PurchaseOrderValue")] public decimal PurchaseOrderValue { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public List<PurchaseOrderTrans>? PurchaseOrderTransList { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
        //Added on 2025.02.04
        [Column("FK_PurchaseOrderStatusID")] public byte PurchaseOrderStatusID { get; set; }

    }
    [Table("V_PurchaseOrder")]
    public class VPurchaseOrder
    {
        [Column("PK_PurchaseOrderID"), Key] public int PurchaseOrderID { get; set; }
        [Column("FK_SupplierID")] public int SupplierID { get; set; }
        [Column("SupplierName")] public string? SupplierName { get; set; }
        [Column("PurchaseOrderNo")] public string? PurchaseOrderNo { get; set; }
        [Column("PurchaseOrderDate")] public DateTime PurchaseOrderDate { get; set; }
        [Column("sPurchaseOrderDate")] public string? sPurchaseOrderDate { get; set; }
        [Column("PurchaseOrderValue")] public decimal PurchaseOrderValue { get; set; }
        [Column("FK_PurchaseOrderStatusID")] public byte PurchaseOrderStatusID { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("PurchaseOrderStatus")] public string? PurchaseOrderStatus { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public List<VPurchaseOrderTrans>? PurchaseOrderTransList { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }

}
