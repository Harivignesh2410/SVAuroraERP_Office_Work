namespace SVAuroraERP.Domain.Purchase
{
    [Table("tPurchaseEntryTrans")]
    public class PurchaseEntryTrans
    {
        [Column("PK_PurchaseTransID"), Key] public int PurchaseTransID { get; set; }
        [Column("FK_PurchaseEntryID")] public int PurchaseEntryID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("Pcs")] public decimal Pcs { get; set; }
        [Column("Quantity")] public decimal? Quantity { get; set; }
        [Column("Rate")] public decimal Rate { get; set; }
        [Column("MaterialValue")] public decimal? MaterialValue { get; set; }
        [Column("FK_OtherChargesID1")] public int? OtherChargesID1 { get; set; }
        [Column("OtherChargesIDAmount1")] public decimal? OtherChargesIDAmount1 { get; set; }
        [Column("FK_OtherChargesID2")] public int? OtherChargesID2 { get; set; }
        [Column("OtherChargesIDAmount2")] public decimal? OtherChargesIDAmount2 { get; set; }
        [Column("FK_OtherChargesID3")] public int? OtherChargesID3 { get; set; }
        [Column("OtherChargesIDAmount3")] public decimal? OtherChargesIDAmount3 { get; set; }
        [Column("OtherChargesAmount")] public decimal? OtherChargesAmount { get; set; }
        [Column("TaxableChargesAmount")] public decimal? TaxableChargesAmount { get; set; }
        [Column("FK_TaxID1")] public int? TaxID1 { get; set; }
        [Column("TaxPercentage1")] public decimal? TaxPercentage1 { get; set; }
        [Column("TaxAmount1")] public decimal? TaxAmount1 { get; set; }
        [Column("FK_TaxID2")] public int? TaxID2 { get; set; }
        [Column("TaxPercentage2")] public decimal? TaxPercentage2 { get; set; }
        [Column("TaxAmount2")] public decimal? TaxAmount2 { get; set; }
        [Column("TaxAmount")] public decimal? TaxAmount { get; set; }
        [Column("SubTotal")] public decimal SubTotal { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public string? StatusFlag { get; set; } //I - Insert, U - Update, D - Delete
    }

    [Table("V_PurchaseEntryTrans")]
    public class VPurchaseEntryTrans
    {
        [Column("PK_PurchaseTransID"), Key] public int PurchaseTransID { get; set; }
        [Column("FK_PurchaseEntryID")] public int PurchaseEntryID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("ItemCode")] public string? ItemCode { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("HSNCode")] public string? HSNCode { get; set; }
        [Column("UnitName")] public string? UnitName { get; set; }
        [Column("Pcs")] public decimal? Pcs { get; set; }
        [Column("Quantity")] public decimal? Quantity { get; set; }
        [Column("Rate")] public decimal? Rate { get; set; }
        [Column("MaterialValue")] public decimal? MaterialValue { get; set; }
        [Column("FK_OtherChargesID1")] public int? OtherChargesID1 { get; set; }
        [Column("OtherChargesDescription1")] public string? OtherChargesDescription1 { get; set; }
        [Column("OtherChargesIDAmount1")] public decimal? OtherChargesIDAmount1 { get; set; }
        [Column("FK_OtherChargesID2")] public int? OtherChargesID2 { get; set; }
        [Column("OtherChargesDescription2")] public string? OtherChargesDescription2 { get; set; }
        [Column("OtherChargesIDAmount2")] public decimal? OtherChargesIDAmount2 { get; set; }
        [Column("FK_OtherChargesID3")] public int? OtherChargesID3 { get; set; }
        [Column("OtherChargesDescription3")] public string? OtherChargesDescription3 { get; set; }
        [Column("OtherChargesIDAmount3")] public decimal? OtherChargesIDAmount3 { get; set; }
        [Column("OtherChargesAmount")] public decimal? OtherChargesAmount { get; set; }
        [Column("TaxableChargesAmount")] public decimal? TaxableChargesAmount { get; set; }
        [Column("FK_TaxID1")] public int? TaxID1 { get; set; }
        [Column("TaxName1")] public string? TaxName1 { get; set; }
        [Column("TaxPercentage1")] public decimal? TaxPercentage1 { get; set; }
        [Column("TaxAmount1")] public decimal? TaxAmount1 { get; set; }
        [Column("FK_TaxID2")] public int? TaxID2 { get; set; }
        [Column("TaxName2")] public string? TaxName2 { get; set; }
        [Column("TaxPercentage2")] public decimal? TaxPercentage2 { get; set; }
        [Column("TaxAmount2")] public decimal? TaxAmount2 { get; set; }
        [Column("TaxAmount")] public decimal? TaxAmount { get; set; }
        [Column("SubTotal")] public decimal? SubTotal { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }

        // Added on 2025.01.21 by Harivignesh
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [NotMapped] public string? StatusFlag { get; set; }

    }
}
