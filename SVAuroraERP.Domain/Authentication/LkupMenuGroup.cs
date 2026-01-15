namespace SVAuroraERP.Domain.Authentication
{
    [Table("LkupMenuGroup")]
    public class LkupMenuGroup
    {
        [Column("PK_MenuGroupID"), Key] public byte MenuGroupID { get; set; }
        [Column("MenuGroupName")] public string? MenuGroupName { get; set; }
        [Column("OrdinalNo")] public byte? OrdinalNo { get; set; }
        public List<LkupMenuControl>? MenuControlList { get; set; } = new List<LkupMenuControl>();
    }
}