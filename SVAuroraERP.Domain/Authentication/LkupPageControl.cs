namespace SVAuroraERP.Domain.Authentication
{
    [Table("LkupPageControl")]
    public class LkupPageControl
    {
        [Column("PK_PageControlID"), Key] public Byte PageControlID { get; set; }
        [Column("FK_MenuControlID")] public Byte MenuControlID { get; set; }
        [Column("PageName")] public string PageName { get; set; } = string.Empty;
        [Column("PageIcon")] public string PageIcon { get; set; } = string.Empty;
        [Column("PageURL")] public string PageURL { get; set; } = string.Empty;
        [Column("OrdinalNo")] public Byte OrdinalNo { get; set; }
        [Column("IsVisible"), JsonIgnore] public bool IsVisible { get; set; }
    }
}