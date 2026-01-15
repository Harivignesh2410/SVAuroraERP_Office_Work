//Added on 2025/04/18 by Harivignesh
namespace SVAuroraERP.Domain.Inventory.Dispatch
{
    //Added on 2025/04/21 by Harivignesh
    [Table("tPacking")]
    public class Packing
    {
        [Column("PK_PackingID"), Key] public int PackingID { get; set; }
        [Column("PackingNo")] public string? PackingNo { get; set; }
        [Column("PackingDate")] public DateTime PackingDate { get; set; }
        [NotMapped] public string? sPackingDate { get; set; }
        [Column("FK_BOXID")] public int BOXID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public List<PackingTrans> PackingTrans { get; set; }
        //Added on 2025.04.29 by Harivignesh
        [Column("StatusID")] public byte StatusID { get; set; }
        [Column("FK_AllotedToID")] public int? AllotedToID { get; set; }
    }

    [Table("v_StockPacking")]
    public class VStockPacking
    {
        [Column("PK_BatchStockID"), Key] public int BatchStockID { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("FK_BOXID")] public int BOXID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("BatchNo")] public string? BatchNo { get; set; }
        [Column("LaserNoPrefix")] public string? LaserNoPrefix { get; set; }
        [Column("StartingNo")] public int StartingNo { get; set; }
        [Column("EndingNo")] public int EndingNo { get; set; }
        [Column("PlateCount")] public int PlateCount { get; set; }
        [Column("StockRequestID")] public int StockRequestID { get; set; }
    }

    public class PackingFilter
    {
        public int BoxID { get; set; }
        public int SizeID { get; set; }
        public int ColorID { get; set; }
    }
    public class AvailableLaserNoDto
    {
        public int BatchStockID { get; set; }
        public int StartingNo { get; set; }
        public int EndingNo { get; set; }
        public int PlateCount { get; set; }
        public string? BatchNo { get; set; }
        public string? ItemName { get; set; }
        public string? StartLaserNo { get; set; } 
        public string? EndLaserNo { get; set; }   
    }


    //public class PackingData
    //{
    //    public int  PK_PackingID { get; set; }
    //    public int Fk_BoxID { get; set; }
    //    public int FK_ColorID { get; set; }
    //    public int LaserNoPrefix { get; set; }
    //    public int StartingNo { get; set; }
    //    public int EndingNo { get; set; }
    //    public int LastUpdatedBy { get; set; }
    //}
    //Added on 2025/04/21 by Harivignesh
    [Table("V_Packing")]
    public class VPacking
    {
        [Column("PK_PackingID"), Key] public int PackingID { get; set; }
        [Column("PackingNo")] public string? PackingNo { get; set; }
        [Column("PackingDate")] public string? PackingDate { get; set; }
        [Column("FK_BOXID")] public int BOXID { get; set; }
        [Column("BoxName")] public string? BoxName { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public List<VPackingTrans> PackingTrans { get; set; }
        [Column("BoxCount")] public int BoxCount { get; set; }
        [Column("TotalQuantity")] public decimal TotalQuantity { get; set; }
        [Column("PcsPerBox")] public decimal PcsPerBox { get; set; }
        [Column("StatusID")] public byte StatusID { get; set; }
        [Column("StatusName")] public string? StatusName { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("FK_AllotedToID")] public int? AllotedToID { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
    }
}
