namespace SVAuroraERP.Domain.Orders.OrdersDelivery
{
    [Table("V_ListDispatchedOrder")]
    public class VListDispatchOrder
    {
        [Column("PK_GenerateDeliveryID"), Key] public int GenerateDeliveryID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("FK_ModeOfTransportID")] public int ModeOfTransportID { get; set; }
        [Column("ModeOfTransport")] public string? ModeOfTransport { get; set; }
        [Column("FK_CourierID")] public int CourierID { get; set; }
        [Column("ConsignmentDetails")] public string? ConsignmentDetails { get; set; }
        [Column("CollectingPerson")] public string? CollectingPerson { get; set; }
        [Column("UploadImageUrl")] public string? UploadImageUrl { get; set; }
        [Column("ImageName")] public string? ImageName { get; set; }
        [Column("GenerateDate")] public DateTime GenerateDate { get; set; }
        [Column("sGenerateDate")] public string? sGenerateDate { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("CourierName")] public string? CourierName { get; set; }
      
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("TotalOrders")] public int TotalOrders { get; set; }
        [Column("EmbossingStationName")] public string? EmbossingStationName { get; set; }
    }

    [Table("V_ListDispatchOrderTrans")]
    public class VListDispatchOrderTrans
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
        [Column("DeliveredDate")] public DateTime DeliveredDate { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("FrontLaserSerialNo")] public string? FrontLaserSerialNo { get; set; }
        [Column("RearLaserSerialNo")] public string? RearLaserSerialNo { get; set; }
        [Column("FrontPlateDimension")] public string? FrontPlateDimension { get; set; }
        [Column("RearPlateDimension")] public string? RearPlateDimension { get; set; }
        [Column("RegNo")] public string? RegNo { get; set; }
        [Column("sRegDate")] public string? sRegDate { get; set; }
        [Column("RegDate")] public DateTime RegDate { get; set; }
        [Column("IsDelivered")] public bool IsDelivered { get; set; }



    }
}
