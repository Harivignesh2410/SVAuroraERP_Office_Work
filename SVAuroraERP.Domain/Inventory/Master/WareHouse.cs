//Added by Harivignesh
namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tWareHouse")]
    public class WareHouse
    {
        [Column("PK_WareHouseID"), Key] public int WareHouseID { get; set; }
        [Column("WareHouseCode")] public string? WareHouseCode { get; set; }
        [Column("WareHouseName")] public string? WareHouseName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }
    [Table("V_WareHouse")]
    public class VWareHouse
    {
        [Column("PK_WareHouseID"), Key] public int WareHouseID { get; set; }
        [Column("WareHouseCode")] public string? WareHouseCode { get; set; }
        [Column("WareHouseName")] public string? WareHouseName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDateIST { get; set; }

    }
}
