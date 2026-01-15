namespace SVAuroraERP.Domain.Inventory.Master
{
    [Table("tRackLocationSizeCapacity")]
    public class RackLocationSizeCapacity
    {
        [Column("PK_RackLocationSizeCapacityID"), Key] public int RackLocationSizeCapacityID { get; set; }
        [Column("FK_RackLocationID")] public int RackLocationID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("Capacity")] public decimal Capacity { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public string? StatusFlag { get; set; }
    }
}
