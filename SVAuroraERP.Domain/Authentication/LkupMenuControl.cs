namespace SVAuroraERP.Domain.Authentication
{
    [Table("LkupMenuControl")]
    public class LkupMenuControl
    {
        [Column("PK_MenuControlID"), Key] public Byte MenuControlID { get; set; }
        [Column("FK_ModuleID")] public Byte ModuleID { get; set; }
        [Column("MenuName")] public string MenuName { get; set; } = string.Empty;
        [Column("MenuDisplayName")] public string MenuDisplayName { get; set; } = string.Empty;
        [Column("MenuIcon")] public string MenuIcon { get; set; } = string.Empty;
        [Column("OrdinalNo")] public Byte OrdinalNo { get; set; }
        [Column("FK_MenuGroupID")] public Byte MenuGroupID { get; set; }
        public List<LkupPageControl>? PageControlList { get; set; } = new List<LkupPageControl>();
    }
}