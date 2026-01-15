namespace SVAuroraERP.Domain.Dealer
{
    [Table("tDealerWorkingDay")]
    public class DealerWorkingDay
    {
        [Column("PK_WorkingDayID"), Key] public int WorkingDayID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("DayOfWeek")] public byte DayOfWeek { get; set; } // 1=Monday, 7=Sunday
        [Column("IsWorking")] public bool IsWorking { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("V_DealerWorkingDay")]
    public class VDealerWorkingDay
    {
        [Column("PK_WorkingDayID"), Key] public int WorkingDayID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("DealerName")] public string? DealerName { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("DayOfWeek")] public byte DayOfWeek { get; set; }
        [Column("DayName")] public string? DayName { get; set; }
        [Column("IsWorking")] public bool IsWorking { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("OEMName")] public string? OEMName { get; set; }
        [Column("City")] public string? City { get; set; }
    }

    public class DealerWorkingDayDataTableRequest : DataTableRequest
    {
        public int? OEMID { get; set; }
        public int? DealerID { get; set; }
    }
}

