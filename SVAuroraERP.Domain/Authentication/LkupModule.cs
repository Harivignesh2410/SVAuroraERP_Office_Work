namespace SVAuroraERP.Domain.Authentication
{
    [Table("LkupModule")]
    public class LkupModule
    {
        [Column("PK_ModuleID"), Key] public byte ModuleID { get; set; }
        [Column("FK_ApplicationID")] public byte ApplicationID { get; set; }
        [Column("ModuleName")] public string? ModuleName { get; set; }
        [Column("OrdinalNo")] public byte? OrdinalNo { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
    }
}