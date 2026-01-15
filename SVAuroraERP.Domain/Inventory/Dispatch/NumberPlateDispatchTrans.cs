//Added on 2025.04.29 by Harivigneshs;
namespace SVAuroraERP.Domain.Inventory.Dispatch
{
    [Table("tNumberPlateDispatchTrans")]
    public class NumberPlateDispatchTrans
    {
        [Column("PK_NumberPlateDispatchTransID"), Key] public int NumberPlateDispatchTransID { get; set; }
        [Column("FK_NumberPlateDispatchID")] public int NumberPlateDispatchID { get; set; }
        [Column("FK_PackingID")] public int PackingID { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
    }

    [Table("V_NumberPlateDispatchTrans")]
    public class VNumberPlateDispatchTrans
    {
        [Column("PK_NumberPlateDispatchTransID"), Key] public int NumberPlateDispatchTransID { get; set; }
        [Column("FK_NumberPlateDispatchID")] public int NumberPlateDispatchID { get; set; }
        [Column("FK_PackingID")] public int PackingID { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("PackingNo")] public string? PackingNo { get; set; }
        [Column("DispatchNo")] public string? DispatchNo { get; set; }
        [Column("DispatchDate")] public string? DispatchDate { get; set; }
        [Column("BoxName")] public string? BoxName { get; set; }
        [Column("PackingDate")] public string? PackingDate { get; set; }
        [Column("BoxCount")] public int BoxCount { get; set; }

        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("TotalQuantity")] public decimal TotalQuantity { get; set; }

        [Column("PcsPerBox")] public decimal PcsPerBox { get; set; }
        [NotMapped] public List<VPackingTrans>? PackingTrans { get; set; }

    }

}