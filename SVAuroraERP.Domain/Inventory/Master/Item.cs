namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tItem")]
    public class Item
    {
        [Column("PK_ItemID"), Key] public int ItemID { get; set; }
        [Column("ItemCode")] public string? ItemCode { get; set; }
        [Column("HSNCode ")] public string? HSNCode { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("Price")] public decimal Price { get; set; }
        [Column("FK_UnitID")] public int UnitID { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsStockRequired")] public bool IsStockRequired { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }

        //Added on 2025.01.05
        [Column("FK_CategoryID")] public int? ItemCategoryID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_ComponentTypeID")] public int? ComponentTypeID { get; set; }
    }


    [Table("V_Item")]
    public class VItem
    {
        [Column("PK_ItemID"), Key] public int ItemID { get; set; }
        [Column("ItemCode")] public string? ItemCode { get; set; }
        [Column("HSNCode ")] public string? HSNCode { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("Price")] public decimal Price { get; set; }
        [Column("FK_UnitID ")] public int UnitID { get; set; }
        [Column("UnitName")] public string? UnitName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsStockRequired")] public bool IsStockRequired { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        [Column("LastUpdatedDateIST")] public DateTime LastUpdatedDateIST { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }

        //Added on 2025.01.05
        [Column("FK_CategoryID")] public int ItemCategoryID { get; set; }
        [Column("CategoryName")] public string? ItemCategoryName { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("FK_ComponentTypeID")] public int? ComponentTypeID { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
    }
}