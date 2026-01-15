using System.Data;

namespace SVAuroraERP.Domain.Inventory.Production
{
    [Table("tLaserNoMarking")]
    public class LaserNoMarking
    {
        [Key, Column("PK_LaserNoMarkingID")] public int LaserNoMarkingID { get; set; }
        [Column("FK_InputBatchStockID")] public int InputBatchStockID { get; set; }
        [Column("FK_OutputBatchStockID")] public int OutputBatchStockID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("FK_RackLocationID")] public int RackLocationID { get; set; }
        [Column("FK_MachineID")] public int MachineID { get; set; }
        [Column("FK_OperatorID")] public int OperatorID { get; set; }
        [Column("StartTime")] public TimeOnly StartTime { get; set; }
        [NotMapped] public string? sStartTime { get; set; }
        [Column("EndTime")] public TimeOnly EndTime { get; set; }
        [NotMapped] public string? sEndTime { get; set; }
        [Column("ProductionDate")] public DateOnly ProductionDate { get; set; }
        [NotMapped] public string? sProductionDate { get; set; }
        [Column("StartingNo")] public int StartingNo { get; set; }
        [Column("EndingNo")] public int EndingNo { get; set; }
        [Column("NoOfPlate")] public int NoOfPlate { get; set; }
        [Column("RejectedPlate")] public int RejectedPlate { get; set; }
        [Column("StartingLaserNo")] public string? StartingLaserNo { get; set; }
        [Column("EndingLaserNo")] public string? EndingLaserNo { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("FK_StatusID")] public int StatusID { get; set; }
        [NotMapped] public LaserNoConsumption? LaserNoConsumption { get; set; }
    }
    [Table("tLaserNoConsumption")]
    public class LaserNoConsumption
    {
        [Key, Column("PK_LaserNoConsumptionID")] public int LaserNoConsumptionID { get; set; }
        [Column("FK_LaserNoMarkingID")] public int LaserNoMarkingID { get; set; }
        [Column("FK_BatchStockID")] public int BatchStockID { get; set; }
        [Column("ActualConsumedQty")] public int ActualConsumedQty { get; set; }
        [Column("WastageQty")] public int WastageQty { get; set; }
        [Column("WastagePercentage")] public decimal WastagePercentage { get; set; }
        [Column("BalanceQty")] public int BalanceQty { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
    }
    [Table("V_LaserNoMarking")]
    public class VLaserNoMarking
    {
        [Column("PK_LaserNoMarkingID"),Key] public int LaserNoMarkingID { get; set; }
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
        [Column("StartingNo")] public int StartingNo { get; set; }
        [Column("EndingNo")] public int EndingNo { get; set; }
        [Column("StartingLaserNo")] public string? StartingLaserNo { get; set; }
        [Column("EndingLaserNo")] public string? EndingLaserNo { get; set; }
        [Column("RejectedPlate")] public int RejectedPlate { get; set; }
        [Column("NoOfPlate")] public int NoOfPlate { get; set; }
        [Column("BatchNo")] public string? BatchNo { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("RackLocationName")] public string? RackLocationName { get; set; }
        [Column("WareHouseName")] public string? WareHouseName { get; set; }
        [Column("OperatorName")] public string? OperatorName { get; set; }
        [Column("FK_InputBatchStockID")] public int InputBatchStockID { get; set; }
        [Column("FK_OutputBatchID")] public int OutputBatchID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("FK_MachineID")] public int MachineID { get; set; }
        [Column("MachineName")] public string? MachineName { get; set; }
        [Column("FK_StatusID")] public byte StatusID { get; set; }
        [Column("BatchStockStatus")] public string? BatchStockStatus { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
    }

    public class FullLaserDataResult
    {
        public DataTable LaserNoMarking { get; set; }
        public DataTable VHologramPunchingCompleted { get; set; }
    }
    public class LaserDataResponse
    {
        public List<VLaserNoMarking> LaserNoMarking { get; set; }
        public List<VHologramPunchingCompleted> VHologramPunchingCompleted { get; set; }
    }
    public class UpdateResult
    {
        public bool IsSuccess { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
