namespace SVAuroraERP.Domain.Inventory.Production
{
    [Table("tStockRequest")]
    public class StockRequest
    {
        [Column("PK_StockRequestID"), Key] public int StockRequestID { get; set; }
        [Column("RequestNo")] public string RequestNo { get; set; } = string.Empty;
        [Column("RequestDate")] public DateTime RequestDate { get; set; }
        [NotMapped] public string sRequestDate { get; set; } = string.Empty;
        [Column("RequestedBy")] public int RequestedBy { get; set; }
        [Column("FK_ProcessTypeID")] public byte ProcessTypeID { get; set; }
        [Column("FK_StatusID")] public byte StatusID { get; set; }
        [Column("ApprovedBy")] public int? ApprovedBy { get; set; }
        [Column("ApprovedDate")] public DateTime? ApprovedDate { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("Narration")] public string? Narration { get; set; }
        [NotMapped] public List<StockRequestTrans>? StockRequestTrans { get; set; }
    }

    //Added on 2025.03.13
    [Table("V_StockRequest")]
    public class VStockRequest
    {
        [Column("PK_StockRequestID"), Key] public int StockRequestID { get; set; }
        [Column("RequestNo")] public string RequestNo { get; set; } = string.Empty;
        [Column("RequestDate")] public DateTime RequestDate { get; set; }
        [Column("sRequestDate")] public string sRequestDate { get; set; } = string.Empty;
        [Column("RequestedBy")] public int RequestedBy { get; set; }
        [Column("RequestedByName")] public string? RequestedByName { get; set; }
        [Column("FK_ProcessTypeID")] public byte ProcessTypeID { get; set; }
        [Column("ProcessTypeName")] public string? ProcessTypeName { get; set; }
        [Column("FK_StatusID")] public byte StatusID { get; set; }
        [Column("StockRequestStatus")] public string? StockRequestStatus { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("ApprovedBy")] public int? ApprovedBy { get; set; }
        [Column("ApprovedByName")] public string? ApprovedByName { get; set; }
        [Column("ApprovedDate")] public DateTime? ApprovedDate { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("Narration")] public string? Narration { get; set; }
        [NotMapped] public List<VStockRequestTrans>? VStockRequestTrans { get; set; }
        [Column("FK_OutputComponentTypeID")] public int OutputComponentTypeID { get; set; }
        //Added on 2025.03.16
        [Column("sApprovedDate")] public string? sApprovedDate { get; set; }
    }

    public class ApprovalRequest
    {
        public int StockRequestID { get; set; }
        public byte StatusID { get; set; }
        public int LastUpdatedBy { get; set; }
        public string? Narration { get; set; }
    }
}