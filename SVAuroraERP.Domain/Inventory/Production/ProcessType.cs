namespace SVAuroraERP.Domain.Inventory.Production
{
    //Added on 2025.03.12
    [Table("LkupProcessType")]
    public class ProcessType
    {
        [Column("PK_ProcessTypeID"), Key] public byte ProcessTypeID { get; set; }
        [Column("ProcessTypeName")] public string? ProcessTypeName { get; set; } = string.Empty;
        [Column("ProcessDescription")] public string? ProcessDescription { get; set; }
        [Column("FK_OutputComponentTypeID")] public int? OutputComponentTypeID { get; set; }
        [Column("OrdinalNo")] public byte OrdinalNo { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("V_ProcessType")]
    public class VProcessType
    {
        [Column("PK_ProcessTypeID"), Key] public byte ProcessTypeID { get; set; }
        [Column("ProcessTypeName")] public string? ProcessTypeName { get; set; } = string.Empty;
        [Column("ProcessDescription")] public string? ProcessDescription { get; set; }
        [Column("FK_OutputComponentTypeID")] public int? OutputComponentTypeID { get; set; }
        [Column("OrdinalNo")] public byte OrdinalNo { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
    }
}