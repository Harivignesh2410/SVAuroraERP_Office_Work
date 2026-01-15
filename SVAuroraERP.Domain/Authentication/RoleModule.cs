namespace SVAuroraERP.Domain.Authentication
{
    [Table("tRoleModule")]
    public class RoleModule
    {
        [Column("PK_RoleModuleID"), Key] public int RoleModuleID { get; set; }
        [Column("FK_RoleID")] public int RoleID { get; set; }
        [Column("FK_ModuleID")] public byte ModuleID { get; set; }
        [Column("IsEnabled")] public bool IsEnabled { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped, JsonIgnore] public string? StatusFlag { get; set; }
    }
}