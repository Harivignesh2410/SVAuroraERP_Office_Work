namespace SVAuroraERP.Domain.Authentication
{
    [Table("trole")]
    public class Role
    {
        [Column("pk_roleid"), Key] public int RoleID { get; set; }
        [Column("FK_ApplicationID")] public byte? ApplicationID { get; set; } 
        [Column("rolename"), MaxLength(150)] public string RoleName { get; set; } = string.Empty;
        [Column("description"), MaxLength(255)] public string? Description { get; set; } = string.Empty;
        [Column("isactive"), DefaultValue(true)] public bool IsActive { get; set; }
        [Column("isdeleted"), DefaultValue(false)] public bool IsDeleted { get; set; }
        [Column("lastupdatedby")] public int LastUpdatedBy { get; set; }
        [Column("lastupdatedate")] public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;
        [NotMapped] public long LoginAuditID { get; set; }
        [NotMapped] public List<byte>? RoleModuleIDs { get; set; }
    }

    [Table("V_Role")]
    public class VRole
    {
        [Column("PK_RoleID"), Key] public int RoleID { get; set; }

        [Column("RoleName")] public string RoleName { get; set; } = string.Empty;
        [Column("Description")] public string? Description { get; set; } = string.Empty;
        [Column("IsActive"), DefaultValue(true)] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateDate")] public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdateDateIST")] public DateTime LastUpdateDateIST { get; set; }
        [Column("RoleModuleIDs")] public string? RoleModuleIDs { get; set; }
        [Column("FK_ApplicationID")] public byte? ApplicationID { get; set; }
        [Column("ApplicationName")] public string? ApplicationName { get; set; }
        [Column("Colorcode")] public string? Colorcode { get; set; }
    }
}