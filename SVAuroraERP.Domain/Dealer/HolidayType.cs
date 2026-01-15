namespace SVAuroraERP.Domain.Dealer
{
    [Table("tHolidayType")]
    public class HolidayType
    {
        [Column("PK_HolidayTypeID"), Key] public int HolidayTypeID { get; set; }
        [Column("TypeName")] public string? TypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_HolidayType")]
    public class VHolidayType
    {
        [Column("PK_HolidayTypeID"), Key] public int HolidayTypeID { get; set; }
        [Column("TypeName")] public string? TypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}

