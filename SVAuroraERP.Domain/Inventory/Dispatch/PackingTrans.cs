//Added on 2025/04/18 by Harivignesh
namespace SVAuroraERP.Domain.Inventory.Dispatch
{
    [Table("tPackingTrans")]
    public class PackingTrans
    {
        [Column("PK_PackingTransID"), Key] public int PackingTransID { get; set; }
        [Column("FK_PackingID")] public int PackingID { get; set; }
        [Column("StartingLaserNo")] public string? StartingLaserNo { get; set; }
        [Column("EndingLaserNo ")] public string? EndingLaserNo { get; set; }
        [Column("InnerBoxNo")] public string? InnerBoxNo { get; set; }
        [Column("Quantity")] public decimal Quantity { get; set; }
        [Column("LaserNoPrefix")] public string? LaserNoPrefix { get; set; }
    }
    [Table("V_PackingTrans")]
    public class VPackingTrans
    {
        [Column("PK_PackingTransID"), Key] public int PackingTransID { get; set; }
        [Column("FK_PackingID")] public int PackingID { get; set; }
        [Column("StartingLaserNo")] public string? StartingLaserNo { get; set; }
        [Column("EndingLaserNo ")] public string? EndingLaserNo { get; set; }
        [Column("InnerBoxNo")] public string? InnerBoxNo { get; set; }
        [Column("Quantity")] public decimal Quantity { get; set; }
        [Column("LaserNoPrefix")] public string? LaserNoPrefix { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("PackingNo")] public string? PackingNo { get; set; }
    }
}
