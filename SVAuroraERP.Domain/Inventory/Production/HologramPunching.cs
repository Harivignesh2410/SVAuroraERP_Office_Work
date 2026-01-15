using System.Data;

namespace SVAuroraERP.Domain.Inventory.Production
{
    [Table("tHologramPunching")]
    public class HologramPunching
    {
        [Column("PK_HologramPunchingID"), Key] public int HologramPunchingID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestID { get; set; }
        [Column("FK_InputBatchStockID")] public int InputBatchStockID { get; set; }
        [Column("FK_OutputBatchStockID")] public int OutputBatchStockID { get; set; }
        [Column("FK_HologramPlateID")] public int HologramPlateID { get; set; }
        [Column("FK_RackLocationID")] public int RackLocationID { get; set; }
        [Column("FK_MachineID")] public int MachineID { get; set; }
        [Column("FK_OperatorID")] public int OperatorID { get; set; }
        [Column("StartTime")] public TimeOnly StartTime { get; set; }
        [NotMapped] public string? sStartTime { get; set; }
        [Column("EndTime")] public TimeOnly EndTime { get; set; }
        [NotMapped] public string? sEndTime { get; set; }
        [Column("ProductionDate")] public DateOnly ProductionDate { get; set; }
        [NotMapped] public string? sProductionDate { get; set; }
        [Column("HologramFinishedQty")] public decimal HologramFinishedQty { get; set; }
        [Column("RejectedPlateQty")] public decimal RejectedPlateQty { get; set; }
        [Column("HologramWastageQty")] public decimal HologramWastageQty { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public List<HologramConsumption> HologramConsumption { get; set; }
        [Column("FK_StatusID")] public int StatusID { get; set; }
    }
    [Table("tHologramConsumption")]
    public class HologramConsumption
    {
        [Column("PK_HologramConsumptionID"), Key] public int HologramConsumptionID { get; set; }
        [Column("FK_HologramPunchingID")] public int HologramPunchingID { get; set; }
        [Column("FK_BatchStockID")] public int BatchStockID { get; set; }
        [Column("ActualConsumedQty")] public decimal ActualConsumedQty { get; set; }
        [Column("WastageQty")] public decimal WastageQty { get; set; }
        [Column("WastagePercentage")] public decimal WastagePercentage { get; set; }
        [Column("BalanceQty")] public decimal BalanceQty { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
    }
    [Table("V_HologramPunching")]
    public class VHologramPunching
    {
        [Column("PK_HologramPunchingID"), Key] public int HologramPunchingID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestID { get; set; }
        [Column("FK_InputBatchStockID")] public int InputBatchStockID { get; set; }
        [Column("FK_OutputBatchStockID")] public int OutputBatchStockID { get; set; }
        [Column("FK_HologramPlateID")] public int HologramPlateID { get; set; }
        [Column("FK_RackLocationID")] public int RackLocationID { get; set; }
        [Column("FK_MachineID")] public int MachineID { get; set; }
        [Column("FK_OperatorID")] public int OperatorID { get; set; }
        [Column("StartTime")] public TimeOnly StartTime { get; set; }
        [Column("sStartTime")] public string? sStartTime { get; set; }
        [Column("EndTime")] public TimeOnly EndTime { get; set; }
        [Column("sEndTime")] public string? sEndTime { get; set; }
        [Column("TotalTime")] public int TotalTime { get; set; }
        [Column("ProductionDate")] public DateOnly ProductionDate { get; set; }
        [Column("sProductionDate")] public string? sProductionDate { get; set; }
        [Column("HologramFinishedQty")] public decimal HologramFinishedQty { get; set; }
        [Column("RejectedPlateQty")] public decimal RejectedPlateQty { get; set; }
        [Column("HologramWastageQty")] public decimal HologramWastageQty { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("BatchNo")] public string? BatchNo { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("RackLocationName")] public string? RackLocationName { get; set; }
        [Column("WareHouseName")] public string? WareHouseName { get; set; }
        [Column("OperatorName")] public string? OperatorName { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("MachineName")] public string? MachineName { get; set; }
        [Column("FK_StatusID")] public int StatusID { get; set; }
        [Column("BatchStockStatus")] public string? StockStatus { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
    }
    [Table("V_HologramPunchingCompleted")]
    public class VHologramPunchingCompleted
    {
        [Column("PK_HologramPunchingID"), Key] public int HologramPunchingID { get; set; }
        [Column("FK_StockRequestID")] public int StockRequestID { get; set; }
        [Column("FK_InputBatchStockID")] public int InputBatchStockID { get; set; }
        [Column("FK_OutputBatchStockID")] public int? OutputBatchStockID { get; set; }
        [Column("FK_HologramPlateID")] public int HologramPlateID { get; set; }
        [Column("FK_RackLocationID")] public int RackLocationID { get; set; }
        [Column("FK_MachineID")] public int MachineID { get; set; }
        [Column("FK_OperatorID")] public int OperatorID { get; set; }
        [Column("StartTime")] public TimeOnly StartTime { get; set; }
        [Column("sStartTime")] public string? sStartTime { get; set; }
        [Column("EndTime")] public TimeOnly EndTime { get; set; }
        [Column("sEndTime")] public string? sEndTime { get; set; }
        [Column("TotalTime")] public int TotalTime { get; set; }
        [Column("ProductionDate")] public DateOnly ProductionDate { get; set; }
        [Column("sProductionDate")] public string? sProductionDate { get; set; }
        [Column("HologramFinishedQty")] public decimal HologramFinishedQty { get; set; }
        [Column("RejectedPlateQty")] public decimal RejectedPlateQty { get; set; }
        [Column("HologramWastageQty")] public decimal HologramWastageQty { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("BatchNo")] public string? BatchNo { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("RackLocationName")] public string? RackLocationName { get; set; }
        [Column("WareHouseName")] public string? WareHouseName { get; set; }
        [Column("OperatorName")] public string? OperatorName { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("MachineName")] public string? MachineName { get; set; }
        [Column("FK_StatusID")] public byte StatusID { get; set; }
        [Column("BatchStockStatus")] public string? StockStatus { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("ProdConsumedQty")] public decimal ProdConsumedQty { get; set; }
        [Column("ProdBalanceQty")] public decimal ProdBalanceQty { get; set; }
        [Column("ProdWastageQty")] public decimal ProdWastageQty { get; set; }
        [Column("ProbableProductionQuantity")] public decimal ProbableProductionQuantity { get; set; }
        [Column("OBFK_StatusID")] public byte OBStatusID { get; set; }
        [Column("Status")] public string? Status { get; set; }
        [Column("OBColorCode")] public string? OBColorCode { get; set; }
    }
    public class FullHologramDataResult
    {
        public DataTable StockRequests { get; set; }
        public DataTable VStockRequestTrans { get; set; }
        public DataTable HologramPunching { get; set; }
        public DataTable BatchStock { get; set; }
    }
    public class HologramDataResponse
    {
        public List<VStockRequest> StockRequests { get; set; }
        public List<VStockRequestTrans> VStockRequestTrans { get; set; }
        public List<VHologramPunching> HologramPunching { get; set; }
        public List<BatchStock> BatchStock { get; set; }
    }

    public class HolgoramGetDataID
    {
        public int BatchStockID { get; set; }
        public int StockRequestID { get; set; }
    }
    

}
