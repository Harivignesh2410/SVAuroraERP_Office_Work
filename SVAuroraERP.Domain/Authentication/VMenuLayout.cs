namespace SVAuroraERP.Domain.Authentication
{
    [Table("V_MenuLayout")]
    public class VMenuLayout
    {
        [Column("PK_PageControlID"), Key] public byte PageControlID { get; set; }
        [Column("FK_MenuControlID")] public byte MenuControlID { get; set; }
        [Column("PageName")] public string? PageName { get; set; }
        [Column("PageURL")] public string? PageURL { get; set; }
        [Column("PageIcon")] public string? PageIcon { get; set; }
        [Column("MenuName")] public string? MenuName { get; set; }
        [Column("MenuIcon")] public string? MenuIcon { get; set; }
        [Column("MenuDisplayName")] public string? MenuDisplayName { get; set; }
        [Column("ModuleOrdinalNo")] public byte ModuleOrdinalNo { get; set; }
        [Column("FK_ModuleID")] public byte ModuleID { get; set; }
        [Column("ModuleName")] public string? ModuleName { get; set; }
        [Column("MenuGroupOrdinalNo")] public byte MenuGroupOrdinalNo { get; set; }
        [Column("FK_MenuGroupID")] public byte MenuGroupID { get; set; }
        [Column("MenuGroupName")] public string? MenuGroupName { get; set; }
        [Column("MenuOrdinalNo")] public byte MenuOrdinalNo { get; set; }
        [Column("PageOrdinalNo")] public byte PageOrdinalNo { get; set; }
    }
}