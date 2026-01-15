//Added on 2025/10/30 by Harivignesh

using SVAuroraERP.Domain.Inventory.Master;

namespace SVAuroraERP.Domain.Inventory.ScrapManagement
{
    [Table("tScrapEntry")]
    public class ScrapEntry
    {
        [Column("PK_ScrapeEntryID"), Key] public int ScrapEntryID { get; set; }
        [Column("ScrapEntryNo")] public string? ScrapEntryNo { get; set; }
        [Column("ScrapDate")] public DateOnly? ScrapDate { get; set; }
        [NotMapped] public string? sScrapDate { get; set; }
        [Column("TotalSoldQty")] public decimal TotalSoldQty { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public List<ScrapEntryTrans>? ScrapEntryTransList { get; set; }
    }

    [Table("V_ScrapEntry")]
    public class VScrapEntry
    {
        [Column("PK_ScrapeEntryID"), Key] public int ScrapEntryID { get; set; }
        [Column("ScrapEntryNo")] public string? ScrapEntryNo { get; set; }
        [Column("sScrapDate")] public string? sScrapDate { get; set; }
        [Column("TotalSoldQty")] public decimal TotalSoldQty { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [Column("ComponentSizeList")] public string? ComponentSizeList { get; set; }
    }

    public class ScrapData
    {
        public int FK_ComponentTypeID { get; set; }
        public string ComponentTypeName { get; set; }
        public int FK_SizeID { get; set; }
        public string SizeName { get; set; }
        public decimal ProdWastageQty { get; set; }
        public decimal PerPlate { get; set; }
        public decimal WastageQtyInKG { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal SoldQty { get; set; }
        public decimal TransSoldQty { get; set; }
        public int ScrapEntryTransID { get; set; }
    }

    public class ScrapDataParameter
    {
        public int ALUMINUMCOILID { get; set; }
        public int BLANKPLATEID { get; set; }
        public int HOLOGRAMPLATEID { get; set; }
        public int SCRAPENTRYID { get; set; }
    }


    public class ScrapDataFilterParameter
    {
        public int SizeID { get; set; }
        public int ComponentTypeID { get; set; }

        public int AluminumCoil { get; set; }
        public int BlankPlate { get; set; }
        public int HologramPlate { get; set; }

        public List<string>? SelectedColumns { get; set; } = new List<string>();
    }


    public class ScrapStockData
    {
        public string? ComponentTypeName { get; set; }
        public string? SizeName { get; set; }
        public decimal TotalScrap { get; set; }
        public decimal SoldQty { get; set; }
        public decimal BalanceQty { get; set; }
    }
}