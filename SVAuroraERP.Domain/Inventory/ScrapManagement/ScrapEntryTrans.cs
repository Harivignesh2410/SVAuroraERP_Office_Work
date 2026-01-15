//Added on 2025/10/31 by Harivignesh
namespace SVAuroraERP.Domain.Inventory.ScrapManagement
{

    [Table("tscrapEntryTrans")]
    public class ScrapEntryTrans
    {
        [Column("PK_ScrapEntryTransID"), Key] public int ScrapEntryTransID { get; set; }
        [Column("FK_ScrapEntryID")] public int ScrapEntryID { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("SoldQty")] public decimal SoldQty { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("V_ScrapEntryTrans")]
    public class VScrapEntryTrans
    {
        [Column("PK_ScrapEntryTransID"), Key] public int ScrapEntryTransID { get; set; }
        [Column("FK_ScrapEntryID")] public int ScrapEntryID { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("SoldQty")] public decimal SoldQty { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
    }
}
