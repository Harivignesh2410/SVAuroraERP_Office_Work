namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tHSRPReplacementReason")]
    public class HSRPReplacementReason
    {
        [Column("PK_HSRPReplacementReasonID"), Key] public int HSRPReplacementReasonID { get; set; }
        [Column("Code")] public string? Code { get; set; }
        [Column("ReplacementReasonName")] public string? ReplacementReasonName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
    }

    [Table("V_HSRPReplacementReason")]
    public class VHSRPReplacementReason
    {
        [Column("PK_HSRPReplacementReasonID"),Key] public int HSRPReplacementReasonID { get; set; }
        [Column("Code")] public string? Code { get; set; }
        [Column("ReplacementReasonName")] public string? ReplacementReasonName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}