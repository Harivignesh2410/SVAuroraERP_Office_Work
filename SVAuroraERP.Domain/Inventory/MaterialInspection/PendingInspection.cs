// Added on 2025.01.21 by Harivignesh (US 44)
namespace SVAuroraERP.Domain.Inventory.MaterialInspection
{
    [Table("tPendingInwardInspection")]
    public class PendingInspection
    {
        [Column("PK_PendingInwardInspectionID"), Key] public int PendingInwardInspectionID { get; set; }
        [Column("FK_PurchaseTransID")] public int PurchaseTransID { get; set; }
        [Column("BatchNo")] public string? BatchNo { get; set; }
        [Column("BatchQuantity")] public decimal BatchQuantity { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("IsAutoBatch")] public bool IsAutoBatch { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
        [NotMapped] public string? StatusFlag { get; set; }
        [Column("PendingQuantity")] public decimal PendingQuantity { get; set; }
        [Column("LessQuantity")] public decimal LessQuantity { get; set; }
        [Column("ExcessQuantity")] public decimal ExcessQuantity { get; set; }
    }
    [Table("V_PendingInwardInspection")]
    public class VPendingInwardInspection
    {
        [Column("PK_PurchaseEntryID")] public int PurchaseEntryID { get; set; }
        [Column("PK_PendingInwardInspectionID"), Key] public int PendingInwardInspectionID { get; set; }
        [Column("FK_PurchaseTransID")] public int PurchaseTransID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("ItemCode")] public string? ItemCode { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("HSNCode")] public string? HSNCode { get; set; }
        [Column("Quantity")] public decimal Quantity { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }
        [Column("UnitName")] public string? UnitName { get; set; }
        [Column("BatchNo")] public string? BatchNo { get; set; }
        [Column("BatchQuantity")] public decimal BatchQuantity { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public string? StatusFlag { get; set; }
        [Column("FK_PurchaseStatusID")] public byte PurchaseStatusID { get; set; }
        [Column("PurchaseStatus")] public string? PurchaseStatus { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [Column("FK_SupplierID")] public int SupplierID { get; set; }
        [Column("SupplierName")] public string? SupplierName { get; set; }
        [Column("PurchaseInvoiceNo")] public string PurchaseInvoiceNo { get; set; }
        [Column("PurchaseInvoiceDate")] public DateTime PurchaseInvoiceDate { get; set; }
        [Column("sPurchaseInvoiceDate")] public string? sPurchaseInvoiceDate { get; set; }
        [Column("PurchaseInvoiceAmount")] public decimal PurchaseInvoiceAmount { get; set; }
        [Column("IsAutoBatch")] public bool IsAutoBatch { get; set; }
        [Column("PendingQuantity")] public decimal PendingQuantity { get; set; }
        [Column("LessQuantity")] public decimal LessQuantity { get; set; }
        [Column("ExcessQuantity")] public decimal ExcessQuantity { get; set; }

    }

    public class SearchPendingInwardFilter
    {

        public int ItemID { get; set; }
        public int ComponentTypeID { get; set; }
        public string? SearchInWord { get; set; }
    }
    public class FilterForBatchStock
    {
        public int SizeID { get; set; }
        public int ColorID { get; set; }
        public int ComponentTypeID { get; set; }
        public int RackLocationID { get; set; }
        public int WareHouseID { get; set; }
        public string? SearchInWord { get; set; }
        public int ReportTypeID { get; set; }

    }

    public class FilterRawMaterialData
    {
        public int SizeID { get; set; }
        public int ColorID { get; set; }
        public int ComponentTypeID { get; set; }
        public List<string> SelectedColumns { get; set; }
    }


    public class NumberPlateStockReportFilter
    {
        public int SizeID { get; set; }
        public int ColorID { get; set; }

        public int BlankPlateID { get; set; }
        public int HologramPlateID { get; set; }
        public int LaserMarkingPlateID { get; set; }

        public List<string>? SelectedColumns { get; set; } = new List<string>();
    }



    public class NumberPlateStockReportData
    {
        public string? SizeName { get; set; }
        public string? ColorName { get; set; }
        public decimal BlankPlate { get; set; }
        public decimal HologramPlate { get; set; }
        public decimal LaserMarkingPlate { get; set; }
        public decimal Packing { get; set; }
        public string? UnitName { get; set; }    // new
    }


}
