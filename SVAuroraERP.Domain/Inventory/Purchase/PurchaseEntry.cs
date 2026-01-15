namespace SVAuroraERP.Domain.Inventory.Purchase
{
    [Table("tPurchaseEntry")]
    public class PurchaseEntry
    {
        [Column("PK_PurchaseEntryID"), Key] public int PurchaseEntryID { get; set; }
        [Column("PurchaseInvoiceNo")] public string? PurchaseInvoiceNo { get; set; }
        [Column("PurchaseInvoiceDate")] public DateTime PurchaseInvoiceDate { get; set; }
        [NotMapped] public string? sPurchaseInvoiceDate { get; set; }
        [Column("FK_SupplierID")] public int SupplierID { get; set; }
        [Column("GrossAmount")] public decimal GrossAmount { get; set; }
        [Column("RoundedOffPlus")] public decimal RoundedOffPlus { get; set; }
        [Column("RoundedOffMinus")] public decimal RoundedOffMinus { get; set; }
        [Column("FK_OtherChargesID")] public int? OtherChargesID { get; set; }
        [Column("OtherChargesAmount")] public decimal? OtherChargesAmount { get; set; }
        [Column("FK_TaxID1")] public int? TaxID1 { get; set; }
        [Column("TaxPercentage1")] public decimal? TaxPercentage1 { get; set; }
        [Column("TaxAmount1")] public decimal? TaxAmount1 { get; set; }
        [Column("FK_TaxID2")] public int? TaxID2 { get; set; }
        [Column("TaxPercentage2")] public decimal? TaxPercentage2 { get; set; }
        [Column("TaxAmount2")] public decimal? TaxAmount2 { get; set; }
        [Column("TaxAmount")] public decimal? TaxAmount { get; set; }
        [Column("PurchaseInvoiceAmount")] public decimal PurchaseInvoiceAmount { get; set; }
        [Column("Narration")] public string? Narration { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public List<PurchaseEntryTrans>? PurchaseEntryTransList { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
        //Added on 2025.02.05
        [Column("FK_PurchaseOrderID")] public int? PurchaseOrderID { get; set; }
        //Added on 2025.02.15 by Harvignesh
        [Column("FK_PurchaseStatusID")] public byte? PurchaseStatusID { get; set; }

    }

    [Table("V_PurchaseEntry")]
    public class VPurchaseEntry
    {
        [Column("PK_PurchaseEntryID"), Key] public int PurchaseEntryID { get; set; }
        [Column("PurchaseInvoiceNo")] public string PurchaseInvoiceNo { get; set; }
        [Column("PurchaseInvoiceDate")] public DateTime PurchaseInvoiceDate { get; set; }
        [Column("sPurchaseInvoiceDate")] public string? sPurchaseInvoiceDate { get; set; }
        [Column("FK_SupplierID")] public int SupplierID { get; set; }
        [Column("GrossAmount")] public decimal GrossAmount { get; set; }
        [Column("RoundedOffPlus")] public decimal RoundedOffPlus { get; set; }
        [Column("RoundedOffMinus")] public decimal RoundedOffMinus { get; set; }
        [Column("FK_OtherChargesID")] public int? OtherChargesID { get; set; }
        [Column("OtherChargesAmount")] public decimal OtherChargesAmount { get; set; }
        [Column("FK_TaxID1")] public int? TaxID1 { get; set; }
        [Column("TaxPercentage1")] public decimal? TaxPercentage1 { get; set; }
        [Column("TaxAmount1")] public decimal? TaxAmount1 { get; set; }
        [Column("FK_TaxID2")] public int? TaxID2 { get; set; }
        [Column("TaxPercentage2")] public decimal? TaxPercentage2 { get; set; }
        [Column("TaxAmount2")] public decimal? TaxAmount2 { get; set; }
        [Column("TaxAmount")] public decimal? TaxAmount { get; set; }
        [Column("PurchaseInvoiceAmount")] public decimal PurchaseInvoiceAmount { get; set; }
        [Column("Narration")] public string? Narration { get; set; }
        [Column("SupplierName")] public string? SupplierName { get; set; }
        [Column("Taxname1")] public string? Taxname1 { get; set; }
        [Column("TaxName2")] public string? TaxName2 { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDateIST")] public DateTime? LastUpdatedDateIST { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [Column("TotalQuantity")] public decimal? TotalQuantity { get; set; }
        [Column("TotalPcs")] public decimal? TotalPcs { get; set; }
        [Column("TotalOtherCharges")] public decimal? TotalOtherCharges { get; set; }
        [Column("TotalItemTax")] public decimal? TotalItemTax { get; set; }
        [NotMapped] public List<VPurchaseEntryTrans>? PurchaseEntryTransList { get; set; }
        [NotMapped] public List<VPendingInwardInspection>? PendingInwardInspectionList { get; set; }
        [Column("FK_PurchaseOrderID")] public int? PurchaseOrderID { get; set; }
        [Column("ComponentNames")] public string? ComponentNames { get; set; }
        //Added on 2025.02.15 by Harvignesh
        [Column("FK_PurchaseStatusID")] public byte? PurchaseStatusID { get; set; }
        [Column("PurchaseStatus")] public string? PurchaseStatus { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
    }

    public class SearchPurchaseEntryFilter
    {
        public int SupplierID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? sStartDate { get; set; }
        public string? sEndDate { get; set; }
        public string? SearchInWord { get; set; }
    }

}