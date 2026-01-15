namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tHSRPPartNumber")]
    public class HSRPPartNumber
    {
        [Column("PK_HSRPPartNumberID"), Key] public int HSRPPartNumberID { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("PartNumber")] public string? PartNumber { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_HSRPPartNumber")]
    public class VHSRPPartNumber
    {
        [Column("PK_HSRPPartNumberID"), Key] public int HSRPPartNumberID { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("OEMName")] public string? OEMName { get; set; }
        [Column("PartNumber")] public string? PartNumber { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}
