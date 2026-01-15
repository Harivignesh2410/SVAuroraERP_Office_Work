namespace SVAuroraERP.Domain.Orders.OrdersDelivery
{
    [Table("tGenerateDeliveryTrans")]
    public class GenerateDeliveryTrans
    {
        [Column("PK_GenerateDeliveryTransID"), Key] public int GenerateDeliveryTransID { get; set; }
        [Column("FK_GenerateDeliveryID")] public int? GenerateDeliveryID { get; set; }
        [Column("FK_OrderID")] public int OrderID { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_GenerateDeliveryTrans")]
    public class VGenerateDeliveryTrans
    {
        [Column("PK_GenerateDeliveryTransID"), Key] public int GenerateDeliveryTransID { get; set; }
        [Column("FK_GenerateDeliveryID")] public int GenerateDeliveryID { get; set; }
        [Column("FK_OrderID")] public int OrderID { get; set; }
        [Column("DealerPONo")] public string? DealerPONo { get; set; }
        [Column("OrderNo")] public string? OrderNo { get; set; }
        [Column("DealerSONo")] public string? DealerSONo { get; set; }
        [Column("Dealer")] public string? Dealer { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("sOrderDate")] public string? sOrderDate { get; set; }
        [Column("OrderDate")] public DateTime OrderDate { get; set; }
        [Column("sRegDate")] public string? sRegDate { get; set; }
        [Column("RegDate")] public DateTime RegDate { get; set; }
       // [Column("DeliveredDate")] public DateTime DeliveredDate { get; set; }
        [Column("PlateColor")] public string? PlateColor { get; set; }
        [Column("FrontPlateSize")] public string? FrontPlateSize { get; set; }
        [Column("RearPlateSize")] public string? RearPlateSize { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("FrontLaserSerialNo")] public string? FrontLaserSerialNo { get; set; }
        [Column("RearLaserSerialNo")] public string? RearLaserSerialNo { get; set; }
        [Column("RegNo")] public string? RegNo { get; set; }
        [Column("IsDelivered")] public bool IsDelivered { get; set; }
    

    }
    [Table("V_AcknowledgeDispatchedOrders")]
    public class VAcknowledgeDispatchedOrders
    {
        [Column("PK_GenerateDeliveryTransID"), Key] public int GenerateDeliveryTransID { get; set; }
        [Column("FK_GenerateDeliveryID")] public int GenerateDeliveryID { get; set; }
        [Column("FK_OrderID")] public int OrderID { get; set; }
        [Column("DealerPONo")] public string? DealerPONo { get; set; }
        [Column("OrderNo")] public string? OrderNo { get; set; }
        [Column("DealerSONo")] public string? DealerSONo { get; set; }
        [Column("FrontPlateDimension")] public string? FrontPlateDimension { get; set; }
        [Column("RearPlateDimension")] public string? RearPlateDimension { get; set; }
        [Column("Dealer")] public string? Dealer { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("sOrderDate")] public string? sOrderDate { get; set; }
        [Column("OrderDate")] public DateTime OrderDate { get; set; }
        [Column("sGenerateDate")] public string? sGenerateDate { get; set; }
        [Column("GenerateDate")] public DateTime GenerateDate { get; set; }
        [Column("sRegDate")] public string? sRegDate { get; set; }
        [Column("RegDate")] public DateTime RegDate { get; set; }
        [Column("DeliveredDate")] public DateTime DeliveredDate { get; set; }
        [Column("PlateColor")] public string? PlateColor { get; set; }
        [Column("FrontPlateSize")] public string? FrontPlateSize { get; set; }
        [Column("RearPlateSize")] public string? RearPlateSize { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("FrontLaserSerialNo")] public string? FrontLaserSerialNo { get; set; }
        [Column("RearLaserSerialNo")] public string? RearLaserSerialNo { get; set; }
        [Column("RegNo")] public string? RegNo { get; set; }
        [Column("IsDelivered")] public bool IsDelivered { get; set; }


    }
}
