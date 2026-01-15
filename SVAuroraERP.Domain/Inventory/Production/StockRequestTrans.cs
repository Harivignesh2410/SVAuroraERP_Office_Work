namespace SVAuroraERP.Domain.Inventory.Production
{
    //Added on 2025.03.12
    [Table("tStockRequestTrans")]
    public class StockRequestTrans
    {
        [Column("PK_StockRequestTransID"), Key] public int StockRequestTransID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestID { get; set; }
        [Column("FK_BatchStockID")] public int BatchStockID { get; set; }
        [Column("Quantity")] public decimal Quantity { get; set; }
        [NotMapped] public string? StatusFlag { get; set; }
    }

    [Table("V_StockRequestTrans")]
    public class VStockRequestTrans
    {
        [Column("FK_BatchStockID")] public int BatchStockID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestID { get; set; }
        [Column("PK_StockRequestTransID"), Key] public int StockRequestTransID { get; set; }
        [Column("Quantity")] public decimal Quantity { get; set; }
        [Column("RequestNo")] public string RequestNo { get; set; } = string.Empty;
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [Column("RequestDate")] public DateTime? RequestDate { get; set; }
        [Column("sRequestDate")] public string? sRequestDate { get; set; }
        [Column("ApprovedDate")] public DateTime? ApprovedDate { get; set; }
        [Column("ApprovedBy")] public int? ApprovedBy { get; set; }
        [Column("BatchNo")] public string BatchNo { get; set; } = string.Empty;
        [Column("BalanceQty")] public decimal BalanceQty { get; set; }
        [Column("ConsumedQty")] public decimal ConsumedQty { get; set; }
        [Column("BatchQuantity")] public decimal BatchQuantity { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("ColorName")] public string ColorName { get; set; } = string.Empty;
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("SizeName")] public string SizeName { get; set; } = string.Empty;
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("ItemName")] public string ItemName { get; set; } = string.Empty;
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("FK_StatusID")] public byte StatusID { get; set; }
        [Column("StockRequestStatus")] public string? StockRequestStatus { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("RequestedByName")] public string? RequestedByName { get; set; }
        [Column("FK_ProcessTypeID")] public byte ProcessTypeID { get; set; }
        [Column("ProcessTypeName")] public string? ProcessTypeName { get; set; }

        //Added on 2025.03.16
        [Column("StockStatus")] public string? StockStatus { get; set; }
        //Added on 2025.05.28 by Harivignesh
        [Column("FK_UnitID")] public int UnitID { get; set; }
        [Column("UnitName")] public string? UnitName { get; set; }
        [Column("ProbableProductionQuantity")] public decimal ProbableProductionQuantity { get; set; }
        [Column("PerPlate")] public decimal PerPlate { get; set; }
        [Column("ProductionQuantity")] public decimal ProductionQuantity { get; set; }
        [Column("ProbableProdConsumedQty")] public decimal ProbableProdConsumedQty { get; set; }
        [Column("ProdWastageQty")] public decimal ProdWastageQty { get; set; }
    }
}