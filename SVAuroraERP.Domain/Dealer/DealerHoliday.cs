namespace SVAuroraERP.Domain.Dealer
{
    [Table("tDealerHoliday")]
    public class DealerHoliday
    {
        [Column("PK_DealerHolidayID"), Key] public int DealerHolidayID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("HolidayDate")] public DateTime HolidayDate { get; set; }
        [NotMapped] public string? sHolidayDate { get; set; }
        [Column("Reason")] public string? Reason { get; set; }
        [Column("IsFullDay")] public bool IsFullDay { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("tDealerHolidayType")]
    public class DealerHolidayType
    {
        [Column("PK_DealerHolidayTypeID"), Key] public int DealerHolidayTypeID { get; set; }
        [Column("FK_DealerHolidayID")] public int DealerHolidayID { get; set; }
        [Column("FK_HolidayTypeID")] public int HolidayTypeID { get; set; }
        [Column("IsEnabled")] public bool IsEnabled { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("V_DealerHoliday")]
    public class VDealerHoliday
    {
        [Column("PK_DealerHolidayID"), Key] public int DealerHolidayID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("DealerName")] public string? DealerName { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("OEMName")] public string? OEMName { get; set; }
        [Column("HolidayDate")] public DateTime HolidayDate { get; set; }
        [Column("sHolidayDate")] public string sHolidayDate { get; set; } = string.Empty;
        [Column("Reason")] public string? Reason { get; set; }
        [Column("IsFullDay")] public bool IsFullDay { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [NotMapped] public string? HolidayTypes { get; set; }
        [NotMapped] public List<int>? HolidayTypeIDs { get; set; }
    }

    public class DealerHolidayDataTableRequest : DataTableRequest
    {
        public int? OEMID { get; set; }
        public int? DealerID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

