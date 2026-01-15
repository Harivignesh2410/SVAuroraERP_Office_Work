namespace SVAuroraERP.Domain.Dealer
{
    [Table("tDealerSlotConfig")]
    public class DealerSlotConfig
    {
        [Column("PK_ConfigID"), Key] public int ConfigID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("FK_TimeSlotID")] public int TimeSlotID { get; set; }
        [Column("SlotDate")] public DateTime SlotDate { get; set; }
        [NotMapped] public string sSlotDate { get; set; } = string.Empty;
        [Column("MaxCapacity")] public int MaxCapacity { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("V_DealerSlotConfig")]
    public class VDealerSlotConfig
    {
        [Column("PK_ConfigID"), Key] public int ConfigID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("DealerName")] public string? DealerName { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("FK_TimeSlotID")] public int TimeSlotID { get; set; }
        [Column("SlotName")] public string? SlotName { get; set; }
        [Column("StartTime")] public TimeSpan StartTime { get; set; }
        [Column("EndTime")] public TimeSpan EndTime { get; set; }
        [Column("SlotDate")] public DateTime SlotDate { get; set; }
        [Column("sSlotDate")] public string sSlotDate { get; set; } = string.Empty;
        [Column("MaxCapacity")] public int MaxCapacity { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("OEMName")] public string? OEMName { get; set; }
    }
    
    public class DealerSlotConfigDataTableRequest : DataTableRequest
    {
        public int? OEMID { get; set; }
        public int? DealerID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}