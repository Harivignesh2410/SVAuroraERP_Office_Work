namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tHSRPReplacementDocument")]
    public class HSRPReplacementDocument
    {
        [Column("PK_HSRPReplacementDocumentID"),Key] public int HSRPReplacementDocumentID { get; set; }
        [Column("FK_ReplacementReasonID")] public int ReplacementReasonID { get; set; }
        [Column("Code")] public string? Code { get; set; }
        [Column("ReplacementDocumentName")] public string? ReplacementDocumentName { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
    }

    [Table("V_HSRPReplacementDocument")]
    public class VHSRPReplacementDocument
    {
        [Column("PK_HSRPReplacementDocumentID"),Key] public int HSRPReplacementDocumentID { get; set; }
        [Column("FK_ReplacementReasonID")] public int ReplacementReasonID { get; set; }
        [Column("ReplacementReasonCode")] public string? ReplacementReasonCode { get; set; }
        [Column("ReplacementReasonName")] public string? ReplacementReasonName { get; set; }
        [Column("Code")] public string? Code { get; set; }
        [Column("ReplacementDocumentName")] public string? ReplacementDocumentName { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}