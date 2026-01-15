//Added on 2025/04/17 by Harivignesh
namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tBox")]
    public class Box
    {
        [Column("PK_BoxID"), Key] public int BoxID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("BoxName")] public string? BoxName { get; set; }
        [Column("MaxCapacity")] public int MaxCapacity { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        //Added on 2025.04.19 by Harivignesh
        [Column("InnerBoxCount")] public byte? InnerBoxCount { get; set; }
        [Column("InnerBoxQuantity")] public byte? InnerBoxQuantity { get; set; }
    }

    [Table("V_Box")]
    public class VBox
    {
        [Column("PK_BoxID"), Key] public int BoxID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("BoxName")] public string? BoxName { get; set; }
        [Column("MaxCapacity")] public int MaxCapacity { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        //Added on 2025.04.19 by Harivignesh
        [Column("InnerBoxCount")] public byte? InnerBoxCount { get; set; }
        [Column("InnerBoxQuantity")] public byte? InnerBoxQuantity { get; set; }
    }
}
