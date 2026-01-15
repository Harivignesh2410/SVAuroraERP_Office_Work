namespace SVAuroraERP.Domain.Authentication
{
    [Table("tRoleConfiguration")]
    public class RoleConfiguration
    {
        [Column("PK_RoleConfigurationID"), Key] public int RoleConfigurationID { get; set; }
        [Column("FK_RoleID")] public int RoleID { get; set; }
        [Column("FK_PageControlID")] public byte PageControlID { get; set; }
        [Column("IsAccess")] public bool IsAccess { get; set; }
        [Column("IsAdd")] public bool IsAdd { get; set; }
        [Column("IsEdit")] public bool IsEdit { get; set; }
        [Column("IsDelete")] public bool IsDelete { get; set; }
        [Column("IsView")] public bool IsView { get; set; }
        [Column("IsExport")] public bool IsExport { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy"), JsonIgnore] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate"), JsonIgnore] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public string? StatusFlag { get; set; }
    }

    [Table("V_RoleConfiguration")]
    public class VRoleConfiguration
    {
        [Column("PK_RoleConfigurationID"), Key] public int RoleConfigurationID { get; set; }
        [Column("FK_RoleID")] public int RoleID { get; set; }
        [Column("FK_MenuControlID")] public byte MenuControlID { get; set; }
        [Column("FK_PageControlID")] public byte PageControlID { get; set; }
        [Column("MenuName")] public string? MenuName { get; set; }
        [Column("PageName")] public string? PageName { get; set; }
        [Column("IsAccess")] public bool IsAccess { get; set; }
        [Column("IsAdd")] public bool IsAdd { get; set; }
        [Column("IsEdit")] public bool IsEdit { get; set; }
        [Column("IsDelete")] public bool IsDelete { get; set; }
        [Column("IsView")] public bool IsView { get; set; }
        [Column("IsExport")] public bool IsExport { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }

        [Column("ModuleOrdinalNo")] public byte ModuleOrdinalNo { get; set; }
        [Column("FK_ModuleID")] public byte ModuleID { get; set; }
        [Column("ModuleName")] public string? ModuleName { get; set; }
        [Column("MenuGroupOrdinalNo")] public byte MenuGroupOrdinalNo { get; set; }
        [Column("FK_MenuGroupID")] public byte MenuGroupID { get; set; }
        [Column("MenuGroupName")] public string? MenuGroupName { get; set; }
        [Column("MenuOrdinalNo")] public byte MenuOrdinalNo { get; set; }
        [Column("PageOrdinalNo")] public byte PageOrdinalNo { get; set; }
        [Column("MenuIcon")] public string? MenuIcon { get; set; }
        [Column("MenuDisplayName")] public string? MenuDisplayName { get; set; }
        [Column("PageURL")] public string? PageURL { get; set; }
        [Column("PageIcon")] public string? PageIcon { get; set; }
    }
}