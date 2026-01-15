namespace SVAuroraERP.Domain.Dealer
{
    [Table("tDealerTimeSlot")]
    public class TimeSlot
    {
        [Column("PK_TimeSlotID"), Key] public int TimeSlotID { get; set; }
        [Column("SlotName")] public string? SlotName { get; set; }
        [Column("StartTime")] public TimeSpan StartTime { get; set; }
        [Column("EndTime")] public TimeSpan EndTime { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_DealerTimeSlot")]
    public class VTimeSlot
    {
        [Column("PK_TimeSlotID"), Key] public int TimeSlotID { get; set; }
        [Column("SlotName")] public string? SlotName { get; set; }
        [Column("StartTime")] public TimeSpan StartTime { get; set; }
        [Column("EndTime")] public TimeSpan EndTime { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}

