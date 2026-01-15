//Added on 2025.05.31  by Harivignesh
using System.Data;

namespace SVAuroraERP.Domain.Inventory.Production
{
    [Table("tHydrolicPressure")]
    public class HydrolicPressure
    {
        [Column("PK_HydrolicPressureID"),Key]public int HydrolicPressureID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("FK_RackLocationID")] public int RackLocationID { get; set; }
        [Column("FK_OperatorID")] public int OperatorID { get; set; }
        [Column("StartTime")] public TimeOnly StartTime { get; set; }
        [NotMapped] public string? sStartTime { get; set; }
        [Column("EndTime")] public TimeOnly EndTime { get; set; }
        [NotMapped] public string? sEndTime { get; set; }
        [Column("ProductionDate")] public DateOnly ProductionDate { get; set; }
        [NotMapped] public string? sProductionDate { get; set; }
        [Column("ProductionQty")] public int ProductionQty { get; set; }
        [Column("WastageQty")] public int WastageQty { get; set; }
        [Column("OtherWastageQty")] public decimal OtherWastageQty { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        //Added on 2025.06.02 by Harivignesh
        [NotMapped] public List<HydrolicConsumption> HydrolicConsumption { get; set; }
        [Column("FK_BatchStockID")] public int BatchStockID { get; set; }
    }
    [Table("V_HydrolicPressure")]
    public class VHydrolicPressure
    {
        [Column("PK_HydrolicPressureID"), Key] public int HydrolicPressureID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("FK_RackLocationID")] public int RackLocationID { get; set; }
        [Column("FK_OperatorID")] public int OperatorID { get; set; }
        [Column("StartTime")] public TimeOnly StartTime { get; set; }
        [Column("sStartTime")] public string? sStartTime { get; set; }
        [Column("EndTime")] public TimeOnly EndTime { get; set; }
        [Column("sEndTime")] public string? sEndTime { get; set; }
        [Column("TotalTime")] public int TotalTime { get; set; }
        [Column("ProductionDate")] public DateOnly ProductionDate { get; set; }
        [Column("sProductionDate")] public string? sProductionDate { get; set; }
        [Column("ProductionQty")] public int ProductionQty { get; set; }
        [Column("WastageQty")] public int WastageQty { get; set; }
        [Column("OtherWastageQty")] public decimal OtherWastageQty { get; set; }
        [Column("FK_BatchStockID")] public int BatchStockID { get; set; }
        [Column("BatchNo")] public string? BatchNo { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("RackLocationName")] public string? RackLocationName { get; set; }
        [Column("WareHouseName")] public string? WareHouseName { get; set; }
        [Column("OperatorName")] public string? OperatorName { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("ProdConsumedQty")] public decimal ProdConsumedQty { get; set; }
        [Column("ProdBalanceQty")] public decimal ProdBalanceQty { get; set; }
        [Column("ProdWastageQty")] public decimal ProdWastageQty { get; set; }
        [Column("ProbableProductionQuantity")] public decimal ProbableProductionQuantity { get; set; }
        // Added on 2025/06/14 by Harivignesh
        [Column("FK_StatusID")] public byte StatusID { get; set; }
        [Column("Status")] public string? Status { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
    [Table("V_HydrolicPressureCompleted")]
    public class VHydrolicPressureCompleted
    {
        [Column("PK_HydrolicPressureID"), Key] public int HydrolicPressureID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("FK_RackLocationID")] public int RackLocationID { get; set; }
        [Column("FK_OperatorID")] public int OperatorID { get; set; }
        [Column("StartTime")] public TimeOnly StartTime { get; set; }
        [Column("sStartTime")] public string? sStartTime { get; set; }
        [Column("EndTime")] public TimeOnly EndTime { get; set; }
        [Column("sEndTime")] public string? sEndTime { get; set; }
        [Column("TotalTime")] public int TotalTime { get; set; }
        [Column("ProductionDate")] public DateOnly ProductionDate { get; set; }
        [Column("sProductionDate")] public string? sProductionDate { get; set; }
        [Column("ProductionQty")] public int ProductionQty { get; set; }
        [Column("WastageQty")] public int WastageQty { get; set; }
        [Column("OtherWastageQty")] public decimal OtherWastageQty { get; set; }
        [Column("FK_BatchStockID")] public int BatchStockID { get; set; }
        [Column("BatchNo")] public string? BatchNo { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("RackLocationName")] public string? RackLocationName { get; set; }
        [Column("WareHouseName")] public string? WareHouseName { get; set; }
        [Column("OperatorName")] public string? OperatorName { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("ProdConsumedQty")] public decimal ProdConsumedQty { get; set; }
        [Column("ProdBalanceQty")] public decimal ProdBalanceQty { get; set; }
        [Column("ProdWastageQty")] public decimal ProdWastageQty { get; set; }
        [Column("ProbableProductionQuantity")] public decimal ProbableProductionQuantity { get; set; }
        // Added on 2025/06/14 by Harivignesh
        [Column("FK_StatusID")] public byte StatusID { get; set; }
        [Column("Status")] public string? Status { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
    public class FullHydraulicDataResult
    {
        public DataTable StockRequest { get; set; }
        public DataTable StockRequestTrans { get; set; }
        public DataTable HydrolicPressure { get; set; }
        public DataTable HydrolicConsumption { get; set; }
    }
    public class HydraulicDataResponse
    {
        public List<VStockRequest> StockRequest { get; set; }
        public List<VStockRequestTrans> StockRequestTrans { get; set; }
        public List<VHydrolicPressure> HydrolicPressure { get; set; }
        public List<HydrolicConsumption> HydrolicConsumption { get; set; }
    }
    [Table("V_HydrolicPressureBatchStock")]
    public class HydrolicPressureBatchStock
    {
        // V_HydrolicPressure
        [Column("PK_HydrolicPressureID"), Key] public int HydrolicPressureID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("FK_RackLocationID")] public int RackLocationID { get; set; }
        [Column("FK_OperatorID")] public int OperatorID { get; set; }

        [Column("sStartTime")] public string? sStartTime { get; set; }
        [Column("StartTime")] public TimeOnly StartTime { get; set; }          
        [Column("sEndTime")] public string? sEndTime { get; set; }
        [Column("TotalTime")] public int TotalTime { get; set; }
        [Column("EndTime")] public TimeOnly EndTime { get; set; }   

        [Column("ProductionDate")] public DateTime ProductionDate { get; set; }
        [Column("sProductionDate")] public string? sProductionDate { get; set; }

        [Column("ProductionQty")] public int ProductionQty { get; set; }
        [Column("WastageQty")] public int WastageQty { get; set; }
        [Column("OtherWastageQty")] public decimal OtherWastageQty { get; set; }

        [Column("BatchNo")] public string? BatchNo { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("RackLocationName")] public string? RackLocationName { get; set; }
        [Column("WareHouseName")] public string? WareHouseName { get; set; }
        [Column("OperatorName")] public string? OperatorName { get; set; }

        [Column("FK_BatchStockID")] public int BatchStockID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }

        [Column("ProdConsumedQty")] public decimal ProdConsumedQty { get; set; }
        [Column("ProdBalanceQty")] public decimal ProdBalanceQty { get; set; }
        [Column("ProdWastageQty")] public decimal ProdWastageQty { get; set; }

        [Column("Status")] public string? Status { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }

        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }

        // V_BatchStock (aliased with BS_)
        [Column("BS_PK_BatchStockID")] public int BSPKBatchStockID { get; set; }
        [Column("BS_FK_ItemID")] public int BSItemID { get; set; }
        [Column("BS_BatchNo")] public string? BSBatchNo { get; set; }

        [Column("BatchQuantity")] public decimal BatchQuantity { get; set; }
        [Column("ConsumedQty")] public decimal ConsumedQty { get; set; }
        [Column("BalanceQty")] public decimal BalanceQty { get; set; }

        [Column("BS_LastUpdatedDate")] public DateTime BSLastUpdatedDate { get; set; }
        [Column("RackLocationID")] public int BSRackLocationID { get; set; }
        [Column("BS_ItemName")] public string? BSItemName { get; set; }

        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }

        [Column("BS_FK_SizeID")] public int BSSizeID { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }

        [Column("BS_FK_ColorID")] public int BSColorID { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }

        [Column("WareHouseID")] public int WareHouseID { get; set; }
        [Column("BS_WareHouseName")] public string? BSWareHouseName { get; set; }
        [Column("BS_RackLocationName")] public string? BSRackLocationName { get; set; }

        [Column("StockRequestID")] public int BSStockRequestID { get; set; }
        [Column("FK_UnitID")] public int UnitID { get; set; }
        [Column("UnitName")] public string? UnitName { get; set; }

        [Column("ProductionQuantity")] public decimal ProductionQuantity { get; set; }
        [Column("PerPlate")] public decimal PerPlate { get; set; }

        [Column("BS_ProdConsumedQty")] public decimal BSProdConsumedQty { get; set; }
        [Column("BS_ProdWastageQty")] public decimal BSProdWastageQty { get; set; }
        [Column("BS_ProdBalanceQty")] public decimal BSProdBalanceQty { get; set; }

        [Column("ProbableProdConsumedQty")] public decimal ProbableProdConsumedQty { get; set; }
        [Column("FK_StatusID")] public byte StatusID { get; set; }
        [Column("BS_Status")] public string? BSStatus { get; set; }
        [Column("BS_ColorCode")] public string? BSColorCode { get; set; }
    }
}
