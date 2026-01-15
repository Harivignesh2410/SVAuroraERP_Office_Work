namespace SVAuroraERP.Domain.Authentication
{

    [Table("tAppLauncher")]
    public class AppLauncher
    {
        [Column("PK_AppLauncherID"), Key] public int AppLauncherID { get; set; }
        [Column("FK_UserID")] public int UserID { get; set; }
        [Column("FK_PageControlID")] public byte PageControlID { get; set; }
        [Column("OrdinalNo")] public byte OrdinalNo { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }

    [Table("V_Applauncher")]
    public class VApplauncher
    {
        [Column("PK_AppLauncherID"), Key] public int AppLauncherID { get; set; }
        [Column("FK_UserID")] public int UserID { get; set; }
        [Column("FK_PageControlID")] public byte PageControlID { get; set; }
        [Column("OrdinalNo")] public byte OrdinalNo { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("PageName")] public string? PageName { get; set; }
        [Column("PageIcon")] public string? PageIcon { get; set; }
        [Column("MenuDisplayName")] public string? MenuDisplayName { get; set; }
        [Column("PageURL")] public string? PageURL { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }
}