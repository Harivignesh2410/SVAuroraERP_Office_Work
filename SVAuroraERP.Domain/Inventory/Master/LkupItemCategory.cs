namespace SVAuroraERP.Domain.Inventory.Master
{
    //Added on 2025.04.05
    [Table("LkupItemCategory")]
    public class LkupItemCategory
    {
        [Column("PK_ItemCategoryID"), Key] public byte ItemCategoryID { get; set; }
        [Column("ItemCategoryName")] public string ItemCategoryName { get; set; } = string.Empty;
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
}