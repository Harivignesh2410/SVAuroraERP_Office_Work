namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tDistrict")]
    public class District
    {
        [Column("PK_DistrictID"), Key] public int DistrictID { get; set; }
        [Column("DistrictCode")] public string? DistrictCode { get; set; }
        [Column("DistrictName")] public string? DistrictName { get; set; }
        [Column("FK_StateID")] public int StateID { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_District")]
    public class VDistrict
    {
        [Column("PK_DistrictID"), Key] public int DistrictID { get; set; }
        [Column("DistrictCode")] public string? DistrictCode { get; set; }
        [Column("DistrictName")] public string? DistrictName { get; set; }
        [Column("FK_StateID")] public int StateID { get; set; }
        [Column("StateName")] public string? StateName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}
